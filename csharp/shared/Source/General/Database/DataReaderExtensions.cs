using System;
using System.Data;


namespace SharedAssemblies.General.Database
{
	/// <summary>
	/// A static class that contains extension methods that can be used on the IDataReader family of classes.
	/// </summary>
	public static class DataReaderExtensions
	{
		/// <summary>
		/// Checks a data reader to see if there exists a column with the given name in the results.
		/// </summary>
		/// <param name="reader">The data reader to check for the given column.</param>
		/// <param name="columnName">The name of the column to check in the results.</param>
		/// <returns>True if a column by that name is found.</returns>
		public static bool HasColumn(this IDataReader reader, string columnName)
		{
			for (int i = 0; i < reader.FieldCount; i++)
			{
				if (reader.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}
	}
}

