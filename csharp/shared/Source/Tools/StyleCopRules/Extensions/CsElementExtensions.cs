using System.Text;
using StyleCop.CSharp;


namespace SharedAssemblies.Tools.StyleCopRules.Extensions
{
    /// <summary>
    /// Extension methods to serialize a C# Element.
    /// </summary>
    internal static class CsElementExtensions
    {
        /// <summary>
        /// Serialize the C# element to string.
        /// </summary>
        /// <param name="element">Element to serialize.</param>
        /// <returns>String form of the element.</returns>
        public static string Serialize(this CsElement element)
        {
            return Serialize(element, 0);
        }


        /// <summary>
        /// True if the element has a declaration section.
        /// </summary>
        /// <param name="element">Element to check.</param>
        /// <returns>True if element has a declaration.</returns>
        public static bool HasDeclaration(this CsElement element)
        {
            return element.Declaration != null;
        }


        /// <summary>
        /// Returns true if the element has a declaration with a non-empty name.
        /// </summary>
        /// <param name="element">Element to check.</param>
        /// <returns>True if the element has a delcaration with a name.</returns>
        public static bool HasNamedDeclaration(this CsElement element)
        {
            return element.Declaration != null && !string.IsNullOrEmpty(element.Declaration.Name);
        }


        /// <summary>
        /// Extension method to determine if the element contains the given named attribute.
        /// </summary>
        /// <param name="element">Element to check.</param>
        /// <param name="attributeName">Name of the attribute to find in the element.</param>
        /// <returns>True if the element has the named attribute given.</returns>
        public static bool HasAttribute(this CsElement element, string attributeName)
        {
            if(element.Attributes != null)
            {
                foreach(var attribute in element.Attributes)
                {
                    foreach(var expression in attribute.AttributeExpressions)
                    {
                        if(expression.Text.StartsWith(attributeName))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
            
            
        /// <summary>
        /// Serialize the C# element to string.
        /// </summary>
        /// <param name="element">Element to serialize.</param>
        /// <param name="level">Level of indention.</param>
        /// <returns>String form of the element.</returns>
        public static string Serialize(this CsElement element, int level)
        {
            var builder = new StringBuilder().Display(level, "[CsElement:");

            if(element != null)
            {
                builder.Display(level + 1, "Name", element.Name);
                builder.Display(level + 1, "Type", element.GetType().FullName);
                builder.Display(level + 1, "Namespace", element.FullNamespaceName);
                builder.Display(level + 1, "Qualified Name", element.FullyQualifiedName);
                builder.Display(level + 1, "Plural Type", element.FriendlyPluralTypeText);
                builder.Display(level + 1, "Type Text", element.FriendlyTypeText);
                builder.Display(level + 1, "Line", element.LineNumber);
                builder.Display(level + 1, "Location", element.Location);
                builder.Display(level + 1, "Header", element.Header);
                builder.Display(level + 1, "Access Modifier", element.AccessModifier);
                builder.Display(level + 1, "Actual Access", element.ActualAccess);
                builder.Display(level + 1, "Analyzer Tag", element.AnalyzerTag);
                builder.Display(level + 1, "Attributes",
                                element.Attributes.Summarize(a => 
                                    a.AttributeExpressions.Summarize(e => e.Text, 10), 10));
                builder.Display(level + 1, "Child Code Count", 
                                element.ChildCodeElements.Summarize(10));
                builder.Display(level + 1, "Child Element Count", 
                                element.ChildElements.NullSafeCount());
                builder.Display(level + 1, "Child Expressions Count", 
                                element.ChildExpressions.NullSafeCount());
                builder.Display(level + 1, "Child Statements Count", 
                                element.ChildStatements.NullSafeCount());
                builder.Display(level + 1, "Element Token Count", 
                                element.ElementTokens.NullSafeCount());
                builder.Display(level + 1, "Element Type", element.ElementType);
                builder.Display(level + 1, "Is Generated", element.Generated);
                builder.Display(level + 1, "Parent", element.Parent);
				builder.Display(level + 1, "Tokens",
								element.Tokens.Summarize(t => t.CsTokenType.ToString() + ':' +
									(t.CsTokenType == CsTokenType.EndOfLine ? "\\n" : t.Text), 200));
                builder.Display(level + 1, "Is Unsafe", element.Unsafe);
                builder.Display(level + 1, "Variables", element.Variables.Summarize(10));
                builder.Append(element.Declaration.Serialize(level + 1));
            }
            else
            {
                builder.Display(level + 1, "null");
            }

            builder.Display(level, "]");

            return builder.ToString();
        }


        /// <summary>
        /// Serialize the C# element to string.
        /// </summary>
        /// <param name="element">Element to serialize.</param>
        /// <param name="level">Level of indention.</param>
        /// <returns>String form of the element.</returns>
        public static string Summarize(this CsElement element, int level)
        {
            var builder = new StringBuilder()
                .Indent(level)
                .Append("[CsElement: ")
                .Append(element != null ? element.Name : "null")
                .Append(']')
                .NewLine();

            return builder.ToString();
        }
    }
}