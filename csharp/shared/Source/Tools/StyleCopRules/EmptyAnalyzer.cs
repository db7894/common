using System;
using System.Diagnostics.CodeAnalysis;
using log4net;
using StyleCop;
using StyleCop.CSharp;


namespace SharedAssemblies.Tools.StyleCopRules
{
    /// <summary>
    /// Just an empty shell analyzer to copy-and-paste because for some reason
    /// StyleCop doesn't like inheritence that is not directly from SourceAnalyzer.
    /// </summary>
    [SourceAnalyzer(typeof(CsParser))]
    public sealed class EmptyAnalyzer : SourceAnalyzer
    {
		// create logger for this analyzer in case of errors
		private static readonly ILog _log = LogManager.GetLogger(typeof(EmptyAnalyzer));
		
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
            return true;
        }
    }
}