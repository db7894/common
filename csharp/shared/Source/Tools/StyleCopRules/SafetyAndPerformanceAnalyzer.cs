using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using log4net;
using StyleCop;
using StyleCop.CSharp;
using SharedAssemblies.Tools.StyleCopRules.Extensions;


namespace SharedAssemblies.Tools.StyleCopRules
{
	/// <summary>
	/// Just an empty shell analyzer to copy-and-paste because for some reason
	/// StyleCop doesn't like inheritence that is not directly from SourceAnalyzer.
	/// </summary>
	[SourceAnalyzer(typeof(CsParser))]
	public sealed class SafetyAndPerformanceAnalyzer : SourceAnalyzer
	{
		// create logger for this analyzer in case of errors
		private static readonly ILog _log = LogManager.GetLogger(typeof(SafetyAndPerformanceAnalyzer));

		private static readonly string[] _forbiddenExceptions
			= new string[]
			  	{
			  		"AppDomainUnloadedException",
			  		"CannotUnloadAppDomainException",
			  		"ContextMarshalException",
			  		"ExecutionEngineException",
			  		"InvalidCastException",
			  		"InternalBufferOverflowException",
			  		"NullReferenceException",
			  		"OutOfMemoryException",
			  		"AmbiguousMatchException",
			  		"ReflectionTypeLoadException",
			  		"StackOverflowException",
			  		"SynchronizationLockException",
			  		"ThreadAbortException",
			  		"ThreadInterruptedException",
			  		"ThreadStateException",
			  		"TypeInitializationException",
			  		"TypeLoadException",
			  		"TypeUnloadedException",
			  	};
	
		
		/// <summary>
		/// Analyze the C# source code document.
		/// </summary>
		/// <param name="document">The code document.</param>
		[SuppressMessage("SharedAssemblies.Tools.StyleCopRules.SafetyAndPerformanceAnalyzer",
			"ST5007:NoCatchSystemException",
			Justification = "Used for code analysis, not production code.  "
				+ "In this case, if something happens we don't want it to crash rest of analysis.")]
		public override void AnalyzeDocument(CodeDocument document)
		{
			try
			{
				Param.RequireNotNull(document, "document");
				var codeDocument = (CsDocument)document;

				if (codeDocument.RootElement != null)
				{
					codeDocument.WalkDocument(VisitElement, VisitStatement, VisitExpression, null);
				}
			} 
			catch (Exception ex)
			{
				_log.Error("An exception occurred in OrderingAnalyzer.AnalyzeDocument.", ex);
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
			switch (element.ElementType)
			{
				case ElementType.Method:
				case ElementType.Property:
					if (element.HasDeclaration() && element.Declaration.Tokens.NullSafeCount() > 0
						&& element.Declaration.Tokens.Any(t => t.CsTokenType == CsTokenType.New))
					{
						AddViolation(parentElement, element.LineNumber,
							"DoNotHideParentMember", element.FriendlyTypeText);						
					}
					break;
			}
			return true;
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
			switch(statement.StatementType)
			{
				case StatementType.Catch:
					CheckForValidCatch(statement as CatchStatement, parentElement);
					break;

				case StatementType.Throw:
					CheckForValidThrow(statement as ThrowStatement, parentElement);
					break;

				case StatementType.Goto:
					AddViolation(parentElement, statement.LineNumber, 
						"NoGotos");
					break;

				case StatementType.Unsafe:
					AddViolation(parentElement, statement.LineNumber,
						"NoUnsafeBlocks");
					break;
			}
			return true;
		}


		/// <summary>
		/// Visits each expression of the code document.
		/// </summary>
		/// <param name="expression">The current expression.</param>
		/// <param name="parentExpression">The parent expression of the current expression.</param>
		/// <param name="parentStatement">The parent statement of the current expression.</param>
		/// <param name="parentElement">The parent element of the current expression.</param>
		/// <param name="context">The walker context.</param>
		/// <returns>True if should continue.</returns>
		private bool VisitExpression(Expression expression, Expression parentExpression,
			Statement parentStatement, CsElement parentElement, object context)
		{
			switch(expression.ExpressionType)
			{
				case ExpressionType.MemberAccess:
					// make sure not a manual invoke of garbage collection
					if(expression.Text.EndsWith("GC.Collect"))
					{
						AddViolation(parentElement, expression.LineNumber,
							"DoNotManuallyGarbageCollect");
					}
					else if(expression.Text.EndsWith("Environment.Exit"))
					{
						AddViolation(parentElement, expression.LineNumber,
							   "DoNotExit");
					}
					else if(expression.Text.EndsWith("Thread.ResetAbort"))
					{
						AddViolation(parentElement, expression.LineNumber,
							"DoNotSupressThreadAbortException");
					}
					break;

				case ExpressionType.Assignment:
					CheckAssignmentInConditional(expression as AssignmentExpression,
					                             parentStatement, parentElement);
					break;
			}
			return true;
		}


		/// <summary>
		/// Checks a throw statement to make sure not throwing somethign we shouldn't.
		/// </summary>
		/// <param name="statement">The throw statement.</param>
		/// <param name="parentElement">The parent element.</param>
		private void CheckForValidThrow(ThrowStatement statement, CsElement parentElement)
		{
			// check the throw expression
			if (statement != null && statement.ChildExpressions.NullSafeCount() > 0)
			{
				foreach(var expression in statement.ChildExpressions)
				{
					var newExpression = expression as NewExpression;

					if (newExpression != null && newExpression.ChildExpressions.NullSafeCount() > 0)
					{
						foreach(var childExpression in newExpression.ChildExpressions)
						{
							var methodInvocation = childExpression as MethodInvocationExpression;

							if (methodInvocation != null)
							{
								var constructorCall = methodInvocation.Text.ClassNameFromConstructorCall();

								if (_forbiddenExceptions.Contains(constructorCall))
								{
									AddViolation(parentElement, statement.LineNumber,
										"DoNotManuallyThrowSystemException",
										constructorCall);										
								}
							}
						}
					}
				}
			}
		}


		/// <summary>
		/// Checks an assignment statement to see if its part of a conditional expression.
		/// </summary>
		/// <param name="expression">The assignment expression.</param>
		/// <param name="parentStatement">The parent expression.</param>
		/// <param name="parentElement">The parent statement.</param>
		private void CheckAssignmentInConditional(AssignmentExpression expression,
			Statement parentStatement, CsElement parentElement)
		{
			switch(parentStatement.StatementType)
			{
				case StatementType.DoWhile:
				case StatementType.Foreach:
				case StatementType.If:
				case StatementType.Switch:
				case StatementType.While:
					// these are easy, no assignment should ever be in these at all.
					AddViolation(parentElement, expression.LineNumber,
						"NoAssignmentInConditionals", expression.LeftHandSide.Text,
						parentStatement.FriendlyTypeText);
					break;

				default:
					// even if not one of above, there are some statements where only
					// part of the statement is a conditional (like the for) so we have
					// to check for only assignment in that sub-part.
					CheckForConditionalSubStatements(expression, parentStatement, parentElement);
					break;
			}
		}


		/// <summary>
		/// Checks an assignment statement to see if its part of a conditional expression.
		/// </summary>
		/// <param name="expression">The assignment expression.</param>
		/// <param name="parentStatement">The parent expression.</param>
		/// <param name="parentElement">The parent statement.</param>
		private void CheckForConditionalSubStatements(AssignmentExpression expression,
			Statement parentStatement, CsElement parentElement)
		{
			// our job is not done above, what if this is part of a for loop?
			// in those cases, we need to examine only the conditional part as 
			// assignments in the first and last parts of a for are fine.
			// This is true for ternaries as well.  So for all else, climb up the
			// parentage to look for conditionals
			var parent = expression.Parent as Expression;

			while (parent != null)
			{
				switch (parent.ExpressionType)
				{
					case ExpressionType.Relational:
					case ExpressionType.Conditional:
						// if the assignment is part of a conditional or relational, stop checking.
						AddViolation(parentElement, expression.LineNumber,
									 "NoAssignmentInConditionals", 
									 expression.LeftHandSide.Text,
									 string.Format("{0} in {1}", parent.FriendlyTypeText,
					             						parentStatement.FriendlyTypeText));
						return;

					case ExpressionType.ObjectInitializer:
						// if the assignment is part of an object initializer, stop checking.
						return;
				}

				parent = parent.Parent as Expression;
			}
		}


		/// <summary> 
		/// Check a catch statement to make sure it's well formed.
		/// </summary>
		/// <param name="catchStatement">The catch expression.</param>
		/// <param name="parentElement">The parent of the catch.</param>
		private void CheckForValidCatch(CatchStatement catchStatement, CsElement parentElement)
		{
			if (catchStatement != null)
			{
				// check catch block for possible defects
				if(IsEmptyCatch(catchStatement))
				{
					// should not be doing assignment anywhere in these
					AddViolation(parentElement, catchStatement.LineNumber,
								 "NoEmptyCatchBlocks");
				}
				else
				{
					CheckForSpecificCatches(catchStatement, parentElement);
					CheckForImpropperRethrow(catchStatement, parentElement);					
				}
			}
		}


		/// <summary> 
		/// Check a catch statement to make sure it's not empty.
		/// </summary>
		/// <param name="catchStatement">The catch expression.</param>
		/// <returns>True if the catch block has no statements.</returns>
		private bool IsEmptyCatch(CatchStatement catchStatement)
		{
			// check for a catch with an empty block of statements
			if (catchStatement.EmbeddedStatement != null)
			{
				if((catchStatement.EmbeddedStatement.ChildStatements.NullSafeCount() == 0) 
					|| catchStatement.EmbeddedStatement.ChildStatements.All(
						s => s.StatementType == StatementType.Empty))
				{
					return true;
				}
			}

			return false;
		}


		/// <summary> 
		/// Check a catch statement to make sure it's too broad in scope.
		/// </summary>
		/// <param name="catchStatement">The catch expression.</param>
		/// <param name="parentElement">The parent of the catch.</param>
		private void CheckForSpecificCatches(CatchStatement catchStatement, CsElement parentElement)
		{
			// check for a catch with an empty block of statements
			if (catchStatement.CatchExpression != null)
			{
				string typeName = GetExceptionTypeName(catchStatement);

				if (string.Equals(typeName.ClassNameWithoutNamespace(), "Exception")
					&& !ThrowsException(catchStatement.ChildStatements))
				{
					AddViolation(parentElement, catchStatement.LineNumber,
					             "NoCatchSystemException");
				}
				else if(string.Equals(typeName.ClassNameWithoutNamespace(), "ThreadAbortException"))
				{
					AddViolation(parentElement, catchStatement.LineNumber,
								 "DoNotSupressThreadAbortException");
				}
			}

			else if(!ThrowsException(catchStatement.ChildStatements))
			{
				// if no expressions, its a catch { } (non-clr catch-all).
				AddViolation(parentElement, catchStatement.LineNumber,
				             "NoCatchAlls");
			}
		}


		/// <summary>
		/// Get the exception type name from the catch statement expression.
		/// </summary>
		/// <param name="catchStatement">The catch statement expression.</param>
		/// <returns>The string of the type name of the exception.</returns>
		private static string GetExceptionTypeName(CatchStatement catchStatement)
		{
			string typeName = string.Empty;

			switch(catchStatement.CatchExpression.ExpressionType)
			{
				case ExpressionType.VariableDeclaration:
					typeName = catchStatement.ClassType.Text;
					break;

				case ExpressionType.Literal:
					typeName = catchStatement.CatchExpression.Text;
					break;
			}

			return typeName;
		}

	
		/// <summary> 
		/// Check a catch statement to make sure it's not rethrowing incorrectly.
		/// </summary>
		/// <param name="catchStatement">The catch expression.</param>
		/// <param name="parentElement">The parent of the catch.</param>
		private void CheckForImpropperRethrow(CatchStatement catchStatement, CsElement parentElement)
		{
			if(catchStatement.CatchExpression != null)
			{
				string identifier = catchStatement.Identifier.Text;
				_log.InfoFormat("CatchStatement.Identifier: {0}", catchStatement.Identifier);

				if (identifier != null && catchStatement.EmbeddedStatement != null)
				{
					CheckForRethrowInChildStatements(catchStatement.EmbeddedStatement.ChildStatements,
						identifier, parentElement);
				}
			}
		}


		/// <summary>
		/// Checks for a rethrow in a set of child statements of the caught exception.
		/// </summary>
		/// <param name="collection">The collection of statements.</param>
		/// <param name="catchIdentifier">The exception identifier that was caught.</param>
		/// <param name="parentElement">The parent element that contains statements.</param>
		private void CheckForRethrowInChildStatements(IEnumerable<Statement> collection, 
			string catchIdentifier, CsElement parentElement)
		{
			foreach (var statement in collection)
			{
				// if the statement is sa throw, check to see if throwing what we caught.
				if (statement.StatementType == StatementType.Throw)
				{
					var throwStatement = statement as ThrowStatement;

					if (throwStatement != null)
					{
						if (throwStatement.ThrownExpression != null
						    && catchIdentifier.Equals(throwStatement.ThrownExpression.Text))
						{
							// if no expressions, its a catch { } (non-clr catch-all).
							AddViolation(parentElement, throwStatement.LineNumber,
							             "UseEmptyThrowToRethrow");
						}
					}
				}

				if(statement.ChildStatements.NullSafeCount() > 0)
				{
					// if the statement has child statements, check those as well.
					CheckForRethrowInChildStatements(statement.ChildStatements, catchIdentifier, 
						parentElement);
				}
			}			
		}

	
		/// <summary>
		/// Checks for a thrpow in a set of child statements.
		/// </summary>
		/// <param name="collection">The collection of statements.</param>
		/// <returns>True if block of statements contains a throw.</returns>
		private bool ThrowsException(IEnumerable<Statement> collection)
		{
			foreach (var statement in collection)
			{
				// if the statement is sa throw, check to see if throwing what we caught.
				if (statement.StatementType == StatementType.Throw)
				{
					return true;
				}

				if (statement.ChildStatements.NullSafeCount() > 0)
				{
					// if the statement has child statements, check those as well.
					if(ThrowsException(statement.ChildStatements))
					{
						return true;
					}
				}
			}

			return false;
		}
	}
}