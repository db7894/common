namespace SharedAssemblies.Tools.StyleCopRules.Extensions
{
	/// <summary>
	/// A collection of helper string extension methods just for parsing C# files.
	/// </summary>
	internal static class StringExtensions
	{
		/// <summary>
		/// Get the class name from a constructor call.
		/// </summary>
		/// <param name="constructorCall">Fully or partially qualified constructor call.</param>
		/// <returns>The class name from the constructor call.</returns>
		public static string ClassNameFromConstructorCall(this string constructorCall)
		{
			string name = null;

			if (constructorCall != null)
			{
				int lastPos = constructorCall.IndexOf('(');

				if (lastPos > 0)
				{
					// strip namespace, if any
					int firstPos = constructorCall.LastIndexOf('.', lastPos);

					name = constructorCall.Substring(firstPos + 1, lastPos - firstPos - 1);
				}
			}

			return name;
		}


		/// <summary>
		/// Get the class name without leading namespace.
		/// </summary>
		/// <param name="name">Full or partial qualified class name.</param>
		/// <returns>Just the class name.</returns>
		public static string ClassNameWithoutNamespace(this string name)
		{
			string result = name;

			if (name != null)
			{
				// strip namespace, if any
				int firstPos = name.LastIndexOf('.');

				if (firstPos >= 0)
				{
					name = name.Substring(firstPos + 1);
				}
			}

			return name;
		}
	}
}
