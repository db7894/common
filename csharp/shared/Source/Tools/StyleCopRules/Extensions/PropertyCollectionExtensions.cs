using System.Text;
using StyleCop;


namespace SharedAssemblies.Tools.StyleCopRules.Extensions
{
    /// <summary>
    /// Extension methods for property collection.
    /// </summary>
    internal static class PropertyCollectionExtensions
    {
        /// <summary>
        /// Serializes the list of properties to string.
        /// </summary>
        /// <param name="collection">Collection of the properties.</param>
        /// <param name="level">Indention level.</param>
        /// <returns>String list of properties.</returns>
        public static string Serialize(this PropertyCollection collection, int level)
        {
            var builder = new StringBuilder();

            builder.Display(level, "[PropertyCollection:");

            if (collection != null)
            {
                foreach (var property in collection)
                {
                    builder.Display(level + 1,
                                    string.Format("{0} ({1})", property.PropertyName, 
                                        property.PropertyType),
                                        collection[property.PropertyName].Serialize());
                }
            }
            else
            {
                builder.Display(level + 1, "null");
            }

            builder.Display(level, "]");

            return builder.ToString();
        }


		/// <summary>
		/// Serialize the property value.
		/// </summary>
		/// <param name="property">The property to serialize.</param>
		/// <returns>The string form of the property value.</returns>
		public static string Serialize(this PropertyValue property)
		{
			if(property != null)
			{
				switch (property.PropertyType)
				{
					case PropertyType.String:
						return (property as StringProperty).Value;
					case PropertyType.Boolean:
						return (property as BooleanProperty).Value.ToString();
					case PropertyType.Int:
						return (property as IntProperty).Value.ToString();
					case PropertyType.Collection:
						return (property as CollectionProperty).Values.Summarize(10);
				}
			}

			return null;
		}
    }
}
