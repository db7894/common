using StyleCop;

namespace SharedAssemblies.Tools.StyleCopRules.Extensions
{
    /// <summary>
    /// A utility for checking common settings.
    /// </summary>
    internal static class CodeAnalyzerExtensions
    {
        /// <summary>
        /// Setting name for whether Element Tracing is enabled.
        /// </summary>
        public const string EnableElementTracingSetting = "EnableElementTracing";

        /// <summary>
        /// Checks to see if the code analyzer is set to analyze generated code
        /// as well as authored code.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="analyzer">The analyzer settings.</param>
        /// <param name="document">The document to analyze.</param>
        /// <param name="propertyName">The name of the setting to read.</param>
        /// <returns>True if should check auto-generated code.</returns>
        internal static T GetSetting<T>(this SourceAnalyzer analyzer, CodeDocument document, 
                                        string propertyName) where T : PropertyValue
        {
            T setting = null;

            if(document.Settings != null)
            {
                setting = analyzer.GetSetting(document.Settings, propertyName) as T;
            }

            return setting;
        }

        
        /// <summary>
        /// Checks to see if the code analyzer tracing is enabled.
        /// </summary>
        /// <param name="analyzer">The analyzer settings.</param>
        /// <param name="document">The document to analyze.</param>
        /// <returns>True if should check auto-generated code.</returns>
        internal static bool IsElementTracingEnabled(this SourceAnalyzer analyzer, CodeDocument document)
        {
            var setting = analyzer.GetSetting<BooleanProperty>(document, EnableElementTracingSetting);

            return setting != null ? setting.Value : false;
        }
    }
}