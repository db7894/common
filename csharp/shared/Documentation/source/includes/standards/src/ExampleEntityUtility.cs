
using System.Data;
using System.Data.Common;
using SharedAssemblies.Entities.ExampleDomain;
using SharedAssemblies.General.Database;
using SharedAssemblies.General.Utilities;

namespace SharedAssemblies.DataAccess.ExampleDomain
{
	/// <summary>
	/// Implementation of the <see cref="IExampleEntityDao"> to work with a SQL database.
	/// </summary>
	public static class ExampleEntityUtility
	{
		/// <summary>
		/// Helper used to hydrate a <see cref="ExampleEntity"> from a database reader.  
		/// </summary>
		/// <param name="reader">The DataReader to extract a ticket from</param>
		/// <returns>A populated ticket from the database record</returns>
		public static ExampleEntity Hydrate(IDataReader reader)
		{
			return new ExampleEntity
			{
				Identifier = reader["identifier"].ToString(),
				BranchCode = reader["branch_code"].ToString(),
				Symbol = reader["symbol"].ToString(),
				Quantity = reader["quantity"].ToType<uint>(),
				Price = reader["price"].ToType<double>(),
			};
		}

		/// <summary>
		/// Helper used to abstract mapping entity to the database
		/// </summary>
		/// <param name="provider">The database provider factory to use</param>
		/// <param name="entity">The entity to create a paramater set for</param>
		/// <returns>A populated parameter set to send to the database</returns>
		public static ParameterSet Parameterize(DbProviderFactory provider, ExampleEntity entity)
		{
			return new ParameterSet(provider)
			{
				{ "@identifier", DbType.String, entity.Identifier },
				{ "@branch_code", DbType.String, entity.BranchCode },
				{ "@symbol", DbType.String, entity.Symbol },
				{ "@quantity", DbType.UInt32, entity.Quantity },
				{ "@price", DbType.Double, entity.Price },
			};
		}
	}
}

