using System.Text;
using StyleCop;
using StyleCop.CSharp;


namespace SharedAssemblies.Tools.StyleCopRules.Extensions
{
    /// <summary>
    /// Extension methods to serialize a C# CodeElement.
    /// </summary>
    internal static class CodeElementExtensions
    {
        /// <summary>
        /// Serialize the C# element to string.
        /// </summary>
        /// <param name="element">Element to serialize.</param>
        /// <returns>String form of the element.</returns>
        public static string Serialize(this ICodeElement element)
        {
            return Serialize(element, 0);
        }


        /// <summary>
        /// Serialize the C# element to string.
        /// </summary>
        /// <param name="element">Element to serialize.</param>
        /// <param name="level">Number of tabs to indent.</param>
        /// <returns>String form of the element.</returns>
		public static string Serialize(this ICodeElement element, int level)
        {
            var builder = new StringBuilder().Display(level, "[CodeElement:");

            if (element != null)
            {
                builder.Display(level + 1, "Qualified Name", element.FullyQualifiedName);
                builder.Display(level + 1, "Line", element.LineNumber);
                builder.Display(level + 1, "Child Code Count",
                                element.ChildCodeElements.Summarize(10));
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
