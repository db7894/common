using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using log4net;
using StyleCop;
using StyleCop.CSharp;
using SharedAssemblies.Tools.StyleCopRules.Extensions;


namespace SharedAssemblies.Tools.StyleCopRules
{
    /// <summary>
    /// Class to set up a tracer for analysis if the trace variable is set.
    /// </summary>
    [SourceAnalyzer(typeof(CsParser))]
    public sealed class AnalyzerTracer : SourceAnalyzer
    {
		// can't initalize yet...
		private static readonly ILog _log;


		/// <summary>
		/// Configure the xml configuration for the log4net instance.
		/// </summary>
		static AnalyzerTracer()
		{
			// load the logger config data
			var assembly = Assembly.GetExecutingAssembly();
			using (var stream = assembly.GetManifestResourceStream(
				"SharedAssemblies.Tools.StyleCopRules.AnalyzerTracer.log4net.xml"))
			{
				log4net.Config.XmlConfigurator.Configure(stream);
			}

			_log = LogManager.GetLogger(typeof(AnalyzerTracer));
			_log.InfoFormat("Started new log file at {0}", DateTime.Now);
		}


        /// <summary>
        /// Analyze the code document for C# ordering rules.
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
                if (_log.IsDebugEnabled && this.IsElementTracingEnabled(document))
                {
                    Param.RequireNotNull(document, "document");
                    var codeDocument = (CsDocument) document;

                    var collection = document.Settings != null
                        ? document.Settings.GetAddInSettings(this) : null;

                    _log.DebugFormat("Add-In Settings:\n{0}\n", collection.Serialize(1));

                    if (codeDocument.RootElement != null)
                    {
                        _log.DebugFormat("Document:\n{0}\n", codeDocument.Serialize(1));

                        codeDocument.WalkDocument(VisitElement, VisitStatement, VisitExpression, null);
                    }
                }
            }
            catch (Exception ex)
            {
				_log.Error("Exception in AnalzyerTracer.AnalyzeDocument().", ex);
            }
        }


        /// <summary>
        /// Visits each element of the document.
        /// </summary>
        /// <param name="element">Current element.</param>
        /// <param name="parentElement">The parent element of the current element.</param>
        /// <param name="context">The walker context.</param>
        /// <returns>True if should continue.</returns>
        private static bool VisitElement(CsElement element, CsElement parentElement, object context)
        {
            _log.DebugFormat("Visiting Element:\n\t[Current Element:\n{0}\t]\n\t"
                + "[Parent Element:\n{1}\t]\n\n",
                element.Serialize(2), parentElement.Summarize(2));

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
        private static bool VisitStatement(Statement statement, Expression parentExpression, 
            Statement parentStatement, CsElement parentElement, object context)
        {
            _log.DebugFormat(
                "Visiting Statement:\n\t[Current Statement:\n{0}\t]\n\t[Parent Expression:\n{1}\t]\n\t"
                + "[Parent Statement:\n{2}\t]\n\t[Parent Element:\n{3}\t]\n\n",
                statement.Serialize(2), parentExpression.Summarize(2),
                parentStatement.Summarize(2), parentElement.Summarize(2));

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
        private static bool VisitExpression(Expression expression, Expression parentExpression, 
            Statement parentStatement, CsElement parentElement, object context)
        {
        	_log.DebugFormat(
        		"Visiting Expression:\n\t[Current Expression:\n{0}\t]\n\t[Parent Expression:\n{1}\t]"
        		+ "\n\t[Parent Statement:\n{2}\t]\n\t[Parent Element:\n{3}\t]\n\n",
        		expression.Serialize(2), parentExpression.Summarize(2),
        		parentStatement.Summarize(2), parentElement.Summarize(2));

            return true;
        }
    }
}
