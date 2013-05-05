using System.Text;
using StyleCop;


namespace SharedAssemblies.Tools.StyleCopRules.Extensions
{
    /// <summary>
    /// Extension methods to serialize a C# Element.
    /// </summary>
    internal static class SettingsExtensions
    {
        /// <summary>
        /// Serialize the C# element to string.
        /// </summary>
        /// <param name="element">Element to serialize.</param>
        /// <returns>String form of the element.</returns>
        public static string Serialize(this Settings element)
        {
            return Serialize(element, 0);
        }


        /// <summary>
        /// Serialize the C# element to string.
        /// </summary>
        /// <param name="element">Element to serialize.</param>
        /// <param name="level">Level of indention.</param>
        /// <returns>String form of the element.</returns>
        public static string Serialize(this Settings element, int level)
        {
            var builder = new StringBuilder().Display(level, "[Settings:");

            if (element != null)
            {
                builder.Display(level + 1, "Analyzer Settings");

                foreach(var settings in element.AnalyzerSettings)
                {
                    builder.Display(level + 2, settings.AddIn.Name);
                    builder.Append(settings.Serialize(level + 3));
                }

                if (element.GlobalSettings != null)
                {
                    builder.Display(level + 1, "Global Settings");
                    builder.Append(element.GlobalSettings.Serialize(level + 2));
                }

                builder.Display(level + 1, "XML Contents", element.Contents);
                builder.Display(level + 1, "Loaded", element.Loaded);
                builder.Display(level + 1, "Location", element.Location);
                builder.Display(level + 1, "Settings", element.ParserSettings);
            }
            else
            {
                builder.Display(level + 1, "null");
            }

            builder.Display(level, "]");

            return builder.ToString();
        }
    }
}
