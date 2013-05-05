using System.Text;
using StyleCop.CSharp;


namespace SharedAssemblies.Tools.StyleCopRules.Extensions
{
    /// <summary>
    /// Extension methods to serialize a C# Element.
    /// </summary>
    internal static class CsDocumentExtensions
    {
        /// <summary>
        /// Serialize the C# element to string.
        /// </summary>
        /// <param name="element">Element to serialize.</param>
        /// <returns>String form of the element.</returns>
        public static string Serialize(this CsDocument element)
        {
            return Serialize(element, 0);
        }
            
            
        /// <summary>
        /// Serialize the C# element to string.
        /// </summary>
        /// <param name="element">Element to serialize.</param>
        /// <param name="level">Level of indention.</param>
        /// <returns>String form of the element.</returns>
        public static string Serialize(this CsDocument element, int level)
        {
            var builder = new StringBuilder().Display(level, "[CsDocument:");

            if(element != null)
            {
                builder.Append(element.FileHeader.Serialize(level + 1));
                builder.Append(element.DocumentContents.Serialize(level + 1));
                builder.Display(level + 1, "Root Element", element.RootElement);
                builder.Append(element.SourceCode.Serialize(level + 1));
                builder.Display(level + 1, "Tokens",
                                element.Tokens.Summarize(t => t.CsTokenType.ToString() + ':' +  
                                    (t.CsTokenType == CsTokenType.EndOfLine ? "\\n" : t.Text), 200));
                builder.Append(element.Settings.Serialize(level + 1));
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
