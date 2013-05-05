using System;
using System.Text;


namespace SharedAssemblies.Tools.StyleCopRules.Extensions
{
    /// <summary>
    /// Extension methods for the string builder class.
    /// </summary>
    internal static class StringBuilderExtensions
    {
        /// <summary>
        /// Indent the builder to the given number of tabs.
        /// </summary>
        /// <param name="builder">Builder to indent.</param>
        /// <param name="indentLevel">Number of tabs to indent.</param>
        /// <returns>The builder itself.</returns>
        public static StringBuilder Indent(this StringBuilder builder, int indentLevel)
        {
            for (int i = 0; i < indentLevel; i++)
            {
                builder.Append('\t');
            }

            return builder;
        }


        /// <summary>
        /// Places a newline at the end of the builder.
        /// </summary>
        /// <param name="builder">Builder to append new line to.</param>
        /// <returns>The builder itself.</returns>
        public static StringBuilder NewLine(this StringBuilder builder)
        {
            builder.Append(Environment.NewLine);

            return builder;
        }


        /// <summary>
        /// Displays a property.
        /// </summary>
        /// <param name="builder">The string builder.</param>
        /// <param name="level">The level to indent.</param>
        /// <param name="tag">The tag of the property.</param>
        /// <returns>The builder itself.</returns>
        public static StringBuilder Display(this StringBuilder builder, int level, 
                                            string tag)
        {
            builder.Indent(level);
            builder.Append(tag);
            builder.NewLine();

            return builder;
        }

        
        /// <summary>
        /// Displays a property.
        /// </summary>
        /// <param name="builder">The string builder.</param>
        /// <param name="level">The level to indent.</param>
        /// <param name="tag">The tag of the property.</param>
        /// <param name="value">The value of the property.</param>
        /// <returns>The builder itself.</returns>
        public static StringBuilder Display(this StringBuilder builder, int level,
                                            string tag, object value)
        {
            builder.Indent(level);
            builder.Append(tag);
            builder.Append(" - ");
            builder.Append(value);
            builder.NewLine();

            return builder;
        }
    }
}