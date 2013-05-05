using System.Linq;
using System.Text;
using StyleCop.CSharp;


namespace SharedAssemblies.Tools.StyleCopRules.Extensions
{
    /// <summary>
    /// Extension methods to serialize a C# Expression.
    /// </summary>
    internal static class DeclarationExtensions
    {
        /// <summary>
        /// Serialize the C# element to string.
        /// </summary>
        /// <param name="element">Element to serialize.</param>
        /// <returns>String form of the element.</returns>
        public static string Serialize(this Declaration element)
        {
            return Serialize(element, 0);
        }


        /// <summary>
        /// Serialize the C# element to string.
        /// </summary>
        /// <param name="element">Element to serialize.</param>
        /// <param name="level">Level of indention.</param>
        /// <returns>String form of the element.</returns>
        public static string Serialize(this Declaration element, int level)
        {
            var builder = new StringBuilder().Display(level, "[Declaration:");

            if (element != null)
            {
                builder.Display(level + 1, "Name", element.Name);
                builder.Display(level + 1, "Element Type", element.ElementType);
                builder.Display(level + 1, "Access Modifier", element.AccessModifier);
                builder.Display(level + 1, "Access Modifier Type", element.AccessModifierType);
                builder.Display(level + 1, "Tokens",
                    element.Tokens.Summarize(t => t.CsTokenType == CsTokenType.EndOfLine
                                                                  ? "\\n" : t.Text, 10));
            }
            else
            {
                builder.Display(level + 1, "null");
            }

            builder.Display(level, "]");

            return builder.ToString();
        }


        /// <summary>
        /// Returns try if the declaration has the token type indicated.
        /// </summary>
        /// <param name="element">The declaration element.</param>
        /// <param name="tokenType">The type of the token to find.</param>
        /// <returns>True if a token of the type is found.</returns>
        public static bool HasToken(this Declaration element, CsTokenType tokenType)
        {
            if(element != null && element.Tokens != null)
            {
                return element.Tokens.Any(token => token.CsTokenType == tokenType);
            }

            return false;
        }
    }
}