using System.Text;
using StyleCop.CSharp;


namespace SharedAssemblies.Tools.StyleCopRules.Extensions
{
    /// <summary>
    /// Extension methods to serialize a C# FileHeader.
    /// </summary>
    internal static class FileHeaderExtensions
    {
        /// <summary>
        /// Serialize the C# element to string.
        /// </summary>
        /// <param name="element">Element to serialize.</param>
        /// <returns>String form of the element.</returns>
        public static string Serialize(this FileHeader element)
        {
            return Serialize(element, 0);
        }


        /// <summary>
        /// Serialize the C# element to string.
        /// </summary>
        /// <param name="element">Element to serialize.</param>
        /// <param name="level">Number of tabs to indent.</param>
        /// <returns>String form of the element.</returns>
        public static string Serialize(this FileHeader element, int level)
        {
            var builder = new StringBuilder().Display(level, "[FileHeader:");

            if (element != null)
            {
                builder.Display(level + 1, "Generated", element.Generated);
                builder.Display(level + 1, "Header Text", element.HeaderText);
                builder.Display(level + 1, "Header Xml", element.HeaderXml);
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
