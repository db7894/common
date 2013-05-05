using System.Text;
using StyleCop;


namespace SharedAssemblies.Tools.StyleCopRules.Extensions
{
    /// <summary>
    /// Extension methods to serialize a C# Element.
    /// </summary>
    internal static class SourceCodeExtensions
    {
        /// <summary>
        /// Serialize the C# element to string.
        /// </summary>
        /// <param name="element">Element to serialize.</param>
        /// <returns>String form of the element.</returns>
        public static string Serialize(this SourceCode element)
        {
            return Serialize(element, 0);
        }


        /// <summary>
        /// Serialize the C# element to string.
        /// </summary>
        /// <param name="element">Element to serialize.</param>
        /// <param name="level">Level of indention.</param>
        /// <returns>String form of the element.</returns>
        public static string Serialize(this SourceCode element, int level)
        {
            var builder = new StringBuilder().Display(level, "[SourceCode:");

            if (element != null)
            {
                builder.Display(level + 1, "Name", element.Name);
                builder.Display(level + 1, "Type", element.Type);
                builder.Display(level + 1, "Exists", element.Exists);
                builder.Display(level + 1, "Parser", element.Parser);
                builder.Display(level + 1, "Path", element.Path);
                builder.Display(level + 1, "Project", element.Project);
                builder.Display(level + 1, "Configurations", element.Configurations);
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
