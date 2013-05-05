using System;
using System.Diagnostics.CodeAnalysis;
using log4net;
using StyleCop;
using StyleCop.CSharp;
using SharedAssemblies.Tools.StyleCopRules.Extensions;


namespace SharedAssemblies.Tools.StyleCopRules
{
	/// <summary>
	/// Custom length and width restrictions that differ from StyleCop's rules.
	/// </summary>
	[SourceAnalyzer(typeof(CsParser))]
	[SuppressMessage("SharedAssemblies.Tools.StyleCopRules.SafetyAndPerformanceAnalyzer",
		"ST5007:NoCatchSystemException",
		Justification = "Used for code analysis, not production code.  "
			+ "In this case, if something happens we don't want it to crash rest of analysis.")]
	public sealed class ReadabilityAnalyzer : SourceAnalyzer
	{
		// create logger for this analyzer in case of errors
		private static readonly ILog _log = LogManager.GetLogger(typeof(ReadabilityAnalyzer)); 
		
		private const int _commentStartLength = 2;
		private const double _probablyABoxedCommentThresholdPercentage = 0.9;
		private const string _commentBoxCharacters = "-=*/_+";
		private const int _maxCharactersFudgeFactor = 20;


		/// <summary>
		/// Property to get the maximum number of parameters per method.
		/// </summary>
		public int MaxParametersPerMethod { get; private set; }


		/// <summary>
		/// Property to store the max number of character per line.
		/// </summary>
		public int MaxCharactersPerLine { get; private set; }


		/// <summary>
		/// Property to store the max lines per method.
		/// </summary>
		public int MaxLinesPerMethod { get; private set; }


		/// <summary>
		/// Analyze the C# source code document.
		/// </summary>
		/// <param name="document">The code document.</param>
		public override void AnalyzeDocument(CodeDocument document)
		{
			try
			{
				Param.RequireNotNull(document, "document");
				var codeDocument = (CsDocument)document;

				if (codeDocument.RootElement != null && !codeDocument.RootElement.Generated)
				{
					// get the settings we need
					MaxCharactersPerLine = GetMaxCharactersPerLine(document);
					MaxLinesPerMethod = GetMaxLinesPerMethod(document);
					MaxParametersPerMethod = GetMaxParametersPerMethod(document);

					// check each element for method length
					codeDocument.WalkDocument(VisitElement, VisitStatement, null, null);

					// analyze the master document itself for line length
					CheckMasterDocument(codeDocument);
				}
			}
			catch (Exception ex)
			{
				_log.Error("An exception occurred in LengthAndWidthAnalyzer.AnalyzeDocument.", ex);
			}
		}


		/// <summary>
		/// Visits each element of the document.
		/// </summary>
		/// <param name="element">Current element.</param>
		/// <param name="parentElement">The parent element of the current element.</param>
		/// <param name="context">The walker context.</param>
		/// <returns>True if should continue.</returns>
		private bool VisitElement(CsElement element, CsElement parentElement, object context)
		{
			// compare starting line of this element to the line of the final token '}'
			if ((element.ElementType == ElementType.Method) ||
				(element.ElementType == ElementType.Indexer) ||
				(element.ElementType == ElementType.Destructor) ||
				(element.ElementType == ElementType.Accessor) ||
				(element.ElementType == ElementType.Constructor))
			{
				CheckMethodForMaxLength(element);
				CheckMethodForMaxParameters(element);
			}

			return true;
		}


		/// <summary>
		/// Checks a method element to see if it exceeds the maximum number of lines.
		/// </summary>
		/// <param name="element">The element representing the method.</param>
		private void CheckMethodForMaxLength(CsElement element)
		{
			// check for the max method length
			int startingLine = element.LineNumber;
			var lastToken = element.Tokens.LastOrDefault();

			if (lastToken != null
				&& (lastToken.LineNumber - startingLine > MaxLinesPerMethod))
			{
				AddViolation(element, element.LineNumber, "MethodCannotExceedMaxLines",
					element.FriendlyPluralTypeText, MaxLinesPerMethod);
			}			
		}


		/// <summary>
		/// Checks a method element to see if it exceed the maximum number of parameters.
		/// </summary>
		/// <param name="element">The element representing a method.</param>
		private void CheckMethodForMaxParameters(CsElement element)
		{
			// check for more than 7 parameters
			var parameterContainer = element as IParameterContainer;

			if (parameterContainer != null
				&& parameterContainer.Parameters.NullSafeCount() > MaxParametersPerMethod)
			{
				AddViolation(element, element.LineNumber, "MethodCannotExceedMaxParameters",
					element.FriendlyPluralTypeText, MaxParametersPerMethod);
			}			
		}


		/// <summary>
		/// Visits each statement of the document.
		/// </summary>
		/// <param name="statement">Current statement.</param>
		/// <param name="parentExpression">Parent expresion of the current statement.</param>
		/// <param name="parentStatement">Parent statement of the current statement.</param>
		/// <param name="parentElement">Parent element of the current statement.</param>
		/// <param name="context">The walker context.</param>
		/// <returns>True if should continue.</returns>
		private bool VisitStatement(Statement statement, Expression parentExpression,
			Statement parentStatement, CsElement parentElement, object context)
		{
			if (statement.StatementType == StatementType.VariableDeclaration)
			{
				var declarationStatement = statement as VariableDeclarationStatement;

				if(declarationStatement != null)
				{
					if(declarationStatement.Declarators.NullSafeCount() > 1)
					{
						AddViolation(parentElement, statement.LineNumber,
									 "OneDeclarationPerLine");						
					}
				}
			}

			return true;
		}


		/// <summary>
		/// Checks the master element for line length.
		/// </summary>
		/// <param name="element">The root element of the document.</param>
		private void CheckMasterDocument(CsDocument element)
		{
			if (element.Tokens != null)
			{
				int length = 0;

				// check each token in the master document itself
				foreach (var token in element.Tokens)
				{
					// if end of line or xml header then reset length
					if (token.CsTokenType == CsTokenType.EndOfLine
						|| token.CsTokenType == CsTokenType.XmlHeader)
					{
						length = 0;
					}

					else
					{
						// do not use c-style multi-line comments.
						if (token.CsTokenType == CsTokenType.MultiLineComment)
						{
							AddViolation(element.RootElement, token.LineNumber,
										 "NoCStyleCommentsAllowed");
						}

						// do not box comments or make "horizontal rule" comments.
						else if (token.CsTokenType == CsTokenType.SingleLineComment
							&& IsBoxedCommentText(token.Text))
						{
							AddViolation(element.RootElement, token.LineNumber,
										 "NoBoxedCommentsAllowed");
						}

						// check the length of any and all lines for right margin
						length = CheckTokenLength(element, token, length);
					}
				}
			}
		}


		/// <summary>
		/// Returns true if the text given is "boxed" text.
		/// </summary>
		/// <param name="text">Text of the comment.</param>
		/// <returns>True if comment text appears to be a "box".</returns>
		private bool IsBoxedCommentText(string text)
		{
			int whiteSpaceCount = 0;
			int boxCharacterCount = 0;

			for(int i = _commentStartLength; i < text.Length; i++)
			{
				if(char.IsWhiteSpace(text[i]))
				{
					++whiteSpaceCount;
				}

				else if(_commentBoxCharacters.IndexOf(text[i]) >= 0)
				{
					++boxCharacterCount;
				}
			}

			// if more than 90% of the non-whitespace text is boxing characters, probably a box.
			double nonSpaceChars = text.Length - _commentStartLength - whiteSpaceCount;
			return (boxCharacterCount / nonSpaceChars) > _probablyABoxedCommentThresholdPercentage;
		}


		/// <summary>
		/// Checks a token to for length and take into account new lines.
		/// </summary>
		/// <param name="element">C# Code document.</param>
		/// <param name="token">The code token to check.</param>
		/// <param name="length">Left-over length.</param>
		/// <returns>Value of length remaining after token processed.</returns>
		private int CheckTokenLength(CsDocument element, CsToken token, int length)
		{
			int previous = -1;

			// assuming 4-space tabs.
			string alternateText = token.Text.Replace("\t", "    ");

			do
			{
				// some tokens are new lines in of themselves, but some tokens
				// like attributes, have new lines embedded.
				int next = alternateText.IndexOf('\n', previous + 1);

				// if there's a new line, take the length from start to new line,
				// otherwise, take from start to full string length
				length += (next >= 0)
					? (next - 1 - previous)
					: (alternateText.Length - 1 - previous);

				if (length > MaxCharactersPerLine + _maxCharactersFudgeFactor)
				{
					AddViolation(element.RootElement, token.LineNumber,
								 "LineCannotExceedMaxLength", MaxCharactersPerLine);
				}

				// if we did find a new-line, reset the length.
				if (next >= 0)
				{
					length = 0;
				}
				previous = next;
			}
			while (previous >= 0);

			return length;
		}


		/// <summary>
		/// Gets the maximum number of lines per method setting from the XML config file.
		/// </summary>
		/// <param name="document">The code document.</param>
		/// <returns>The maximum number of lines of code per method.</returns>
		private int GetMaxLinesPerMethod(CodeDocument document)
		{
			var property = this.GetSetting<IntProperty>(document, "MaxLinesPerMethod");

			return property != null ? property.Value : 50;
		}


		/// <summary>
		/// Gets the maximum number of parameters per method setting from the XML config file.
		/// </summary>
		/// <param name="document">The code document.</param>
		/// <returns>The maximum number of parameters per method.</returns>
		private int GetMaxParametersPerMethod(CodeDocument document)
		{
			var property = this.GetSetting<IntProperty>(document, "MaxParametersPerMethod");

			return property != null ? property.Value : 7;
		}


		/// <summary>
		/// Gets the maximum number of characters per line from the XML config file.
		/// </summary>
		/// <param name="document">The code document.</param>
		/// <returns>The maximum number of characters per line of code.</returns>
		private int GetMaxCharactersPerLine(CodeDocument document)
		{
			var property = this.GetSetting<IntProperty>(document, "MaxCharactersPerLine");

			return property != null ? property.Value : 120;           
		}
	}
}