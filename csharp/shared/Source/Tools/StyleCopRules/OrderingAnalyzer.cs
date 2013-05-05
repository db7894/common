using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using log4net;
using StyleCop;
using StyleCop.CSharp;


namespace SharedAssemblies.Tools.StyleCopRules
{
    /// <summary>
    /// Custom ordering rules that differ from StyleCop's rules.
    /// </summary>
    [SourceAnalyzer(typeof(CsParser))]
    public sealed class OrderingAnalyzer : SourceAnalyzer
    {
		// create logger for this analyzer in case of errors
		private static readonly ILog _log = LogManager.GetLogger(typeof(OrderingAnalyzer));


		/// <summary>Map to determine order of elements.</summary>
        private static readonly Dictionary<ElementType, int> _orderingMap
            = new Dictionary<ElementType, int>
                  {
                        { ElementType.File, 0 },
                        { ElementType.Root, 1 },
                        { ElementType.ExternAliasDirective, 2 },
                        { ElementType.UsingDirective, 3 },
                        { ElementType.Namespace, 4 },
                        { ElementType.Field, 5 },
                        { ElementType.Constructor, 8 },
                        { ElementType.Destructor, 10 },
                        { ElementType.Delegate, 7 },
                        { ElementType.Event, 7 },
                        { ElementType.Enum, 11 },
                        { ElementType.Interface, 12 },
                        { ElementType.Property, 6 },
                        { ElementType.Accessor, 6 },
                        { ElementType.Indexer, 6 },
                        { ElementType.Method, 9 },
                        { ElementType.Struct, 13 },
                        { ElementType.Class, 14 },
                        { ElementType.EnumItem, -1 },
                        { ElementType.ConstructorInitializer, -1 },
                        { ElementType.EmptyElement, -1 }
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

                if (codeDocument.RootElement != null && !codeDocument.RootElement.Generated)
                {
                    codeDocument.WalkDocument(VisitElement, null, null, null);
                }
            }
            catch (Exception ex)
            {
                _log.Error("An exception occurred in OrderingAnalyzer.AnalyzeDocument", ex);
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
            if (element.ChildElements != null)
            {
                var children = element.ChildElements.ToArray();

                // check each child element in turn
                for(int current = 0; current < children.Length; current++)
                {
                    // against every other component that follows it
                    for(int following = current + 1; following < children.Length; following++)
                    {
                        CheckElementRelativeOrdering(children[current], children[following]);
                    }
                }
            }

            return true;
        }


        /// <summary>
        /// Checks the ordering of two elements relative to each other.
        /// </summary>
        /// <param name="current">The current element in the document.</param>
        /// <param name="following">An element that follows it in the document.</param>
        private void CheckElementRelativeOrdering(CsElement current, CsElement following)
        {
            // get the ordering of the element types
            int currentOrder = _orderingMap[current.ElementType];
            int followingOrder = _orderingMap[following.ElementType];

            // if both elements are ordered (not -1) then compare them
            if (currentOrder != -1 && followingOrder != -1)
            {
                // if they are turned around, this is an issue.
                if(currentOrder > followingOrder)
                {
                    AddViolation(following, following.LineNumber,
                                 "ChildElementsMustFollowOrder", 
                                 following.FriendlyPluralTypeText,
                                 current.FriendlyPluralTypeText);
                }
            }
        }
    }
}