using System;
using System.Collections.Generic;
using System.Data;


namespace SharedAssemblies.Security.UnitTests.Nonce
{
	/// <summary>
	/// Helper class to dry things out
	/// </summary>
	internal static class TestTypeFactory
	{
		/// <summary>
		/// Helper to build a collection of entities in the database
		/// </summary>
		/// <param name="identifier">The identifier for the data</param>
		/// <returns>The mocked DataTable</returns>
		public static DataTable[] CreateKeyResult(string identifier)
		{
			DataTable data = new DataTable();
			var columns = new List<string>
			{
				"session_string",
			};
			columns.ForEach(col => data.Columns.Add(col));

			if (identifier != null)
			{
				data.Rows.Add(identifier);
			}
			return new[] { data };
		}

		/// <summary>
		/// Helper to build a collection of entities in the database
		/// </summary>
		/// <param name="identifier">The identifier for the data</param>
		/// <returns>The mocked DataTable</returns>
		public static DataTable[] CreateValueResult(string identifier)
		{
			DataTable data = new DataTable();
			var columns = new List<string>
			{
				"session_string",
			};
			columns.ForEach(col => data.Columns.Add(col));

			if (identifier != null)
			{
				data.Rows.Add(identifier);
			}
			return new[] { data };
		}

		/// <summary>
		/// Helper to build a consistent entity
		/// </summary>
		/// <param name="populate">True to populate the entity, false otherwise</param>
		/// <returns>The resulting entity</returns>
		public static ExampleClass CreateEntity(bool populate)
		{
			return (!populate) ? new ExampleClass()
				: new ExampleClass 
				{
					Name = "Some Person",
					Age = 24,
					BirthDay = DateTime.Now,
				};
		}
	}
}