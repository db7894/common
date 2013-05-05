using System.Text;
using StyleCop.CSharp;


namespace SharedAssemblies.Tools.StyleCopRules.Extensions
{
    /// <summary>
    /// Extension methods to serialize a C# Statement.
    /// </summary>
    internal static class StatementExtensions
    {
        /// <summary>
        /// Serialize the C# element to string.
        /// </summary>
        /// <param name="element">Element to serialize.</param>
        /// <returns>String form of the element.</returns>
        public static string Serialize(this Statement element)
        {
            return Serialize(element, 0);
        }


        /// <summary>
        /// Serialize the C# element to string.
        /// </summary>
        /// <param name="element">Element to serialize.</param>
        /// <param name="level">Number of tabs to indent.</param>
        /// <returns>String form of the element.</returns>
		public static string Serialize(this Statement element, int level)
        {
        	var builder = new StringBuilder().Display(level, "[Statement:");

        	if (element != null)
        	{
        		builder.Display(level + 1, "Type", element.StatementType);
        		builder.Display(level + 1, "Attached Count", element.AttachedStatements.NullSafeCount());
        		builder.Display(level + 1, "Plural Type", element.FriendlyPluralTypeText);
        		builder.Display(level + 1, "Type Text", element.FriendlyTypeText);
        		builder.Display(level + 1, "Line", element.LineNumber);
        		builder.Display(level + 1, "Location", element.Location);
        		builder.Display(level + 1, "Child Expressions Count",
        		                element.ChildExpressions.NullSafeCount());
        		builder.Display(level + 1, "Child Statements Count",
        		                element.ChildStatements.NullSafeCount());
        		builder.Display(level + 1, "Location", element.Location);
        		builder.Display(level + 1, "Parent", element.Parent);
				builder.Display(level + 1, "Tokens",
								element.Tokens.Summarize(t => t.CsTokenType.ToString() + ':' +
									(t.CsTokenType == CsTokenType.EndOfLine ? "\\n" : t.Text), 200));
        		builder.Display(level + 1, "Variables", element.Variables.Summarize(10));
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
        public static string Summarize(this Statement element, int level)
        {
            var builder = new StringBuilder()
                .Indent(level)
                .Append("[Statement: ")
                .Append(element != null ? element.FriendlyTypeText : "null")
                .Append(']')
                .NewLine();

            return builder.ToString();
        }
    }
}
