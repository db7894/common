using System;
using System.Diagnostics.CodeAnalysis;
using log4net;
using StyleCop;
using StyleCop.CSharp;
using SharedAssemblies.Tools.StyleCopRules.Extensions;


namespace SharedAssemblies.Tools.StyleCopRules
{
    /// <summary>
    /// Custom source code organization rules that differ from StyleCop's rules.
    /// </summary>
    [SourceAnalyzer(typeof(CsParser))]
    public sealed class OrganizationAnalyzer : SourceAnalyzer
    {
		// create logger for this analyzer in case of errors
		private static readonly ILog _log = LogManager.GetLogger(typeof(OrganizationAnalyzer));


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
                    CheckFileNameAndPath(codeDocument);
                }
            }
            catch (Exception ex)
            {
                _log.Error("An exception occurred in OrganizationAnalyzer.AnalyzeDocument.", ex);
            }
        }


        /// <summary>
        /// Checks to see if the C# file has correct namespace for it's name and path.
        /// </summary>
        /// <param name="document">The document to parse.</param>
        private void CheckFileNameAndPath(CsDocument document)
        {
            string path = null;
            string fileName = null;

            if (document.SourceCode != null)
            {
                int lastSlash = document.SourceCode.Path.LastIndexOf('\\');

                if(lastSlash >= 0)
                {
                    // get the path and filename
                    path = document.SourceCode.Path.Substring(0, lastSlash);
                    fileName = document.SourceCode.Path.Substring(lastSlash + 1);

                    // make sure file begins upper case
                    if (!char.IsUpper(fileName[0]))
                    {
                        AddViolation(document.RootElement, 1,
                            "FilenamesMustBeginUpperCase", fileName);
                    }

                    // trim off everything after the '.' (i.e. ".cs")
                    int firstDotInFileName = fileName.IndexOf('.');
                    if(firstDotInFileName >= 0)
                    {
                        fileName = fileName.Substring(0, firstDotInFileName);
                    }
                }
            }

            // check that the namespace in the document is correct
            CheckNamespaceConsistency(document, path, fileName);
        }


        /// <summary>
        /// Checks the file's namespaces to make sure there is one and only one and that
        /// it is organized correctly.
        /// </summary>
        /// <param name="document">The C# code document.</param>
        /// <param name="path">The path to the source file.</param>
        /// <param name="fileName">The source file name.</param>
        private void CheckNamespaceConsistency(CsDocument document, string path, string fileName)
        {
            int namespaceCount = 0;

            // check all root level elements for namespces
            foreach (var element in document.RootElement.ChildElements)
            {
                switch(element.ElementType)
                {
                    case ElementType.Namespace:
                        // make sure not more than one namespace
                        if (++namespaceCount > 1)
                        {
                            AddViolation(element, element.LineNumber, "OnlyOneNamespacePerFile");
                        }

                        // check the namespace consistency
                        CheckNamespacePath(element, path);
                        CheckNamespaceContents(element, fileName);
                        break;

                    case ElementType.Enum:
                    case ElementType.Interface:
                    case ElementType.Struct:
                    case ElementType.Class:
                        AddViolation(element, element.LineNumber,
                            "PrimaryTypesMustBeInsideNamespaces", element.FriendlyPluralTypeText);
                        break;
                }
            }

            if (namespaceCount == 0 && !string.Equals(fileName, "AssemblyInfo"))
            {
            	_log.DebugFormat("No namespace found in {0}", fileName);

                AddViolation(document.RootElement, document.RootElement.LineNumber,
                    "FileMustHaveNamspace");
            }            
        }


        /// <summary>
        /// Checks the consistency of the file namespace vs the file system path.
        /// </summary>
        /// <param name="element">The element to check.</param>
        /// <param name="path">The path to the source file.</param>
        private void CheckNamespacePath(CsElement element, string path)
        {
            // make sure the namespace name is consistent with the path
            if (element.Declaration != null)
            {
                // check that its identical to path
                var expectedPath = element.Declaration.Name.Replace('.', '\\');

                if (path != null && !path.EndsWith(expectedPath))
                {
                    AddViolation(element, element.LineNumber,
                        "NamespaceMustMatchPath", expectedPath, path);
                }

                // check that namespaces are PascalCase.
                foreach(var part in expectedPath.Split(new char[] { '\\' }))
                {
                    if(!char.IsUpper(part[0]))
                    {
                        AddViolation(element, element.LineNumber,
                            "NamespaceElementsMustBePascalCase", part, expectedPath);                        
                    }
                }
            }            
        }


        /// <summary>
        /// Make sure that the namespace doesn't contain usings or other namespaces 
        /// and that it only has one primary, root-level type.
        /// </summary>
        /// <param name="element">The namespace element.</param>
        /// <param name="fileName">The filename of the document.</param>
        private void CheckNamespaceContents(CsElement element, string fileName)
        {
            CsElement previousTopLevelType = null;

            if (element.ChildElements != null)
            { 
                // check each element under the namespace
                foreach (var childElement in element.ChildElements)
                {
                    switch (childElement.ElementType)
                    {
                        case ElementType.UsingDirective:
                            // usings only allowed outside the namespace
                            AddViolation(childElement, childElement.LineNumber,
                                         "UsingsOutsideNamespace");
                            break;

                        case ElementType.Namespace:
                            // no nested namespaces allowed
                            AddViolation(childElement, childElement.LineNumber,
                                         "NoNestedNamespaces");
                            break;

                        case ElementType.Enum:
                        case ElementType.Interface:
                        case ElementType.Struct:
                        case ElementType.Class:
                            {
                                // only one primary type allowed per namespace
                                if (previousTopLevelType != null)
                                {
                                    AddViolation(childElement, childElement.LineNumber,
                                                 "OnlyOnePrimaryTypePerFile");
                                }

                                // primary type's name must match file name prefix
                                CheckPrimaryTypeFileName(childElement, fileName);

                                // save this type in case more
                                previousTopLevelType = childElement;
                            }
                            break;
                    }
                }
            }
        }


        /// <summary>
        /// Checks the primary type in the file to make sure that its
        /// name is the same as the start of the file name.
        /// </summary>
        /// <param name="element">The element for the primary type.</param>
        /// <param name="fileName">The filename prefix (before the '.').</param>
        private void CheckPrimaryTypeFileName(CsElement element, string fileName)
        {
            if (fileName != null)
            {
                if (element.HasNamedDeclaration() &&
                    !string.Equals(fileName, GetNonGenericName(element)))
                {
                    AddViolation(element, element.LineNumber,
                                 "PrimaryTypeNameMustEqualFileName",
                                 element.FriendlyPluralTypeText,
                                 fileName);
                }
            }            
        }


        /// <summary>
        /// Checks the element name and removes any generic argument list.
        /// </summary>
        /// <param name="element">The element to check.</param>
        /// <returns>The name without any named argument list.</returns>
        private static string GetNonGenericName(CsElement element)
        {
            string name = element.Declaration.Name;
            int typeParameterStart = name.IndexOf('<');

            if (typeParameterStart >= 0)
            {
                name = name.Substring(0, typeParameterStart);
            }

            return name.Trim();
        }
    }
}