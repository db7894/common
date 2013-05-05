using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
	public sealed class NamingAnalyzer : SourceAnalyzer
	{
		// create logger for this analyzer in case of errors
		private static readonly ILog _log = LogManager.GetLogger(typeof(NamingAnalyzer));

		// The allowed prefixes for bool property names.
		private static readonly string[] _allowedBoolPropertyPrefixes 
			= new[] { "Can", "Should", "Is", "Has", "Was", "Did" };

		// allowed private field names w/o underscores because they are generated
		private static readonly string[] _allowedPrivateFieldNames
			= new[] { "components" };


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

				if (codeDocument.RootElement != null && !codeDocument.RootElement.Generated)
				{
					// check each element for method length
					codeDocument.WalkDocument(VisitElement, null, null, null);
				}
			}
			catch (Exception ex)
			{
				_log.Error("An exception occurred in NamingAnalyzer.AnalyzeDocument", ex);
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
			// can't check names w/o a declaration name, so skip ones that don't have one
			if (element.Declaration != null && !string.IsNullOrEmpty(element.Declaration.Name))
			{
				switch(element.ElementType)
				{
					case ElementType.Field:
						CheckFieldNameValid(element);
						break;

					case ElementType.Interface:
						CheckInterfaceNameValid(element);
						CheckGenericArgumentNames(element);
						break;

					case ElementType.Class:
						CheckClassNameValid(element);
						CheckGenericArgumentNames(element);
						break;

					case ElementType.Method:
						CheckGenericArgumentNames(element);
						break;

					case ElementType.Struct:
						CheckGenericArgumentNames(element);
						break;

					case ElementType.Property:
						CheckPropertyNames(element);
						break;
				}

				// regardless, check for invalid underscore usage
				CheckInvalidUnderscoreUsage(element);
			}
			return true;
		}


		/// <summary>
		/// Checks an element to see if it is an interface, and if so, is it named
		/// appropriately.
		/// </summary>
		/// <remarks>Assumes Declaration and Name exist.</remarks>
		/// <param name="element">The element to check.</param>
		private void CheckInterfaceNameValid(CsElement element)
		{
			if (!element.Declaration.Name.StartsWith("I"))
			{
			   AddViolation(element, element.LineNumber,
				   "InterfacesMustStartWithLetterI",
				   element.Declaration.Name);
			}
		}


		/// <summary>
		/// Checks a class to see if the name is valid.
		/// </summary>
		/// <param name="element">Element representing a class.</param>
		private void CheckClassNameValid(CsElement element)
		{
			var classElement = element as Class;

			if (classElement != null)
			{
				// abstract class check
				if (element.Declaration.HasToken(CsTokenType.Abstract))
				{
					CheckAbstractClassNameValid(classElement);
				}

				// exception check
				if (classElement.BaseClass.EndsWith("Exception"))
				{
					CheckExceptionClassNameValid(classElement);
				}
				
				if (classElement.BaseClass.EndsWith("Attribute"))
				{
					CheckAttributeClassNameValid(classElement);
				}
			}
		}


		/// <summary>
		/// Checks to see if generic argument names are valid.
		/// </summary>
		/// <param name="element">Element representing a unit of code.</param>
		private void CheckGenericArgumentNames(CsElement element)
		{
			string typeParameters = GetClassParameters(element);
			if (typeParameters != null)
			{
				foreach (var typeParameter in typeParameters.Split(new[] { ',' }))
				{
					CheckTypeParameterName(element, typeParameter.Trim());
				}
			}
		}


		/// <summary>
		/// Checks an element to see if it is a type parameter, and if so, is it named
		/// appropriately.
		/// </summary>
		/// <remarks>Assumes Declaration and Name exist.</remarks>
		/// <param name="element">The element to check.</param>
		/// <param name="typeParameter">The type parameter name.</param>
		private void CheckTypeParameterName(CsElement element, string typeParameter)
		{
			if (!typeParameter.StartsWith("T"))
			{
				AddViolation(element, element.LineNumber,
					"TypeParametersMustStartWithLetterT",
					typeParameter,
					element.Declaration.Name);
			}
		}


		/// <summary>
		/// Gets the class parameter names.
		/// </summary>
		/// <param name="classElement">The class element.</param>
		/// <returns>The list of class parameters.</returns>
		private static string GetClassParameters(CsElement classElement)
		{
			string typeParameters = null;

			int startIndex = classElement.Declaration.Name.IndexOf('<');
			if (startIndex >= 0)
			{
				int endIndex = classElement.Declaration.Name.IndexOf('>', startIndex);
				if (endIndex >= 0)
				{
					typeParameters = classElement.Declaration.Name.Substring(startIndex + 1,
																			 endIndex - startIndex
																			 - 1);
				}
			}

			return typeParameters;
		}


		/// <summary>
		/// Gets the class name without any generic parameters.
		/// </summary>
		/// <param name="classElement">The class element.</param>
		/// <returns>The list of class name without parameter names.</returns>
		private static string GetClassNameWithoutParameters(CsElement classElement)
		{
			string undecoratedName = classElement.Declaration.Name;

			int startIndex = undecoratedName.IndexOf('<');
			if (startIndex >= 0)
			{
				undecoratedName = undecoratedName.Substring(0, startIndex);
			}

			return undecoratedName;
		}


		/// <summary>
		/// Checks an element to see if it an abstract class, and if so, is it named
		/// appropriately.
		/// </summary>
		/// <remarks>Assumes Declaration and Name exist.</remarks>
		/// <param name="element">The element to check.</param>
		private void CheckAbstractClassNameValid(CsElement element)
		{
			if (!element.Declaration.Name.StartsWith("Abstract"))
			{
				AddViolation(element, element.LineNumber,
					"AbstractClassesMustStartWithAbstract",
					element.Declaration.Name);
			}
		}


		/// <summary>
		/// Checks an element to see if it an exception class, and if so, is it named
		/// appropriately.
		/// </summary>
		/// <remarks>Assumes Declaration and Name exist.</remarks>
		/// <param name="element">The element to check.</param>
		private void CheckExceptionClassNameValid(CsElement element)
		{
			if (!element.Declaration.Name.EndsWith("Exception"))
			{
				AddViolation(element, element.LineNumber,
					"ExceptionClassesMustEndWithException",
					element.Declaration.Name);
			}
		}


		/// <summary>
		/// Checks an element to see if it an attribute class, and if so, is it named
		/// appropriately.
		/// </summary>
		/// <remarks>Assumes Declaration and Name exist.</remarks>
		/// <param name="element">The element to check.</param>
		private void CheckAttributeClassNameValid(CsElement element)
		{
			if (!element.Declaration.Name.EndsWith("Attribute"))
			{
				AddViolation(element, element.LineNumber,
					"AttributeClassesMustStartWithAttribute",
					element.Declaration.Name);
			}
		}


		/// <summary>
		/// Checks an element to see if it is a private field, and if so, is it named
		/// appropriately.
		/// </summary>
		/// <remarks>Assumes Declaration and Name exist.</remarks>
		/// <param name="element">The element to check.</param>
		private void CheckFieldNameValid(CsElement element)
		{
			// compare starting line of this element to the line of the final token '}'
			if (element.Declaration.AccessModifierType == AccessModifierType.Private)
			{
				if (!element.Declaration.Name.StartsWith("_") &&
					!CheckExemptFieldNames(element))
				{
					AddViolation(element, element.LineNumber,
						"NonPublicFieldsMustStartWithUnderscore",
						element.ActualAccess,
						element.Declaration.Name);
				}
			}
		}


		/// <summary>
		/// Checks an element declaration to see if underscores are used in it.
		/// </summary>
		/// <param name="element">The element to check for underscores in name.</param>
		private void CheckInvalidUnderscoreUsage(CsElement element)
		{
			bool shouldCheckForUnderscore = true;

			// if the element is a method, check to see if it's a unit test or an event handler
			if (element.ElementType == ElementType.Method)
			{
				var method = (Method)element;

				// if the element is a method, 
				if (method.HasAttribute("TestMethod") || 
					method.Parameters.Any(param => param.Type.Text.IndexOf("Event") >= 0))
				{
					shouldCheckForUnderscore = false;
				}
			}

			// if not a method, or if not a special method (unit test, event handler), check for underscore
			if (shouldCheckForUnderscore && DoesIdentifierHaveInvalidUnderscore(element))
			{
				AddViolation(element, element.LineNumber,
							 "NoUnderscoresExceptWhereValid",
							 element.FriendlyTypeText,
							 element.Declaration.Name);
			}
		}


		/// <summary>
		/// Checks an identifier to see if it has an underscore invalid for its element type
		/// </summary>
		/// <param name="element">The element to check.</param>
		/// <returns>True if invalid underscore found.</returns>
		private bool DoesIdentifierHaveInvalidUnderscore(CsElement element)
		{
			bool foundInvalidUnderscore = false;

			// if it's a private field, ignore the first pos and start looking at second.
			int start = (element.Declaration.AccessModifierType == AccessModifierType.Private
						 && element.ElementType == ElementType.Field)
							? 1
							: 0;

			int foundAt = element.Declaration.Name.IndexOf('_', start);

			if (foundAt >= 0)
			{
				foundInvalidUnderscore = true;
			}

			return foundInvalidUnderscore;
		}


		/// <summary>
		/// Checks a property name to make sure it satisfies the naming rules for properties.
		/// </summary>
		/// <param name="element">The element to check.</param>
		private void CheckPropertyNames(CsElement element)
		{
			var property = element as Property;

			if (property != null && element.HasNamedDeclaration())
			{
				// bool properties must have right prefix
				if(property.ReturnType.Text.Equals("bool"))
				{
					CheckBoolPropertyNames(element);
				}

				// for any other property, check for invalid prefixes
				else
				{
					// if the property name starts with the enclosing type name, this is not allowed.
					var parent = element.FindParentElement();
					if(parent != null && parent.HasNamedDeclaration())
					{
						string parentTypeName = GetClassNameWithoutParameters(parent);

						if(element.Declaration.Name.StartsWith(parentTypeName))
						{
							AddViolation(element, element.LineNumber,
										 "PropertyMayNotStartWithTypeName",
										 element.Declaration.Name,
										 parentTypeName);
						}
					}

					// make sure property doesn't start with superfluous Get or Set.
					if(element.Declaration.Name.StartsWith("Get") 
						|| element.Declaration.Name.StartsWith("Set"))
					{
						if(element.Declaration.Name.Length < 3 
							|| char.IsUpper(element.Declaration.Name[3]))
						{
							AddViolation(element, element.LineNumber,
										 "PropertyMayNotStartWithGetOrSet",
										 element.Declaration.Name);
						}
					}
				}
			}
		}


		/// <summary>
		/// Checks an element to see if it's in the exempt elements list due to auto-generation.
		/// </summary>
		/// <param name="element">The element to check.</param>
		/// <returns>True if element is exempt.</returns>
		private bool CheckExemptFieldNames(CsElement element)
		{
			bool isExempt = false;

			foreach (var field in _allowedPrivateFieldNames)
			{
				if (field.Equals(element.Declaration.Name))
				{
					isExempt = true;
				}
			}

			return isExempt;
		}


		/// <summary>
		/// Checks a bool property to make sure it starts with the right prefix.
		/// </summary>
		/// <param name="element">The element to check.</param>
		private void CheckBoolPropertyNames(CsElement element)
		{
			bool isValid = false;

			foreach (var prefix in _allowedBoolPropertyPrefixes)
			{
				if (element.Declaration.Name.StartsWith(prefix))
				{
					isValid = true;
				}
			}

			if (!isValid)
			{
				AddViolation(element, element.LineNumber,
							 "BoolPropertyNamesMustBePrefixed",
							 element.Declaration.Name);
			}			
		}
	}
}