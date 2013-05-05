using System;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Security;
using System.Security.Permissions;
using System.Collections.Generic;


namespace SharedAssemblies.General.Database.Mock
{
	/// <summary>
	/// A mock factory that implements the DbProviderFactory abstract class
	/// </summary>
	public class MockClientFactory : DbProviderFactory
	{
		/// <summary>
		/// Index to get/set MockConnectionResults using array-like syntax
		/// </summary>
		/// <param name="connectionString">The connection string key</param>
		/// <returns>The mock results for the connection string</returns>
		public MockConnectionResults this[string connectionString]
		{
			get { return GetOrCreateConnectionResults(connectionString); }

			set { SetConnectionResults(connectionString, value); }
		}


		/// <summary>
		/// Index to get/set the default connection results set
		/// </summary>
		public MockConnectionResults ConnectionResults
		{
			get { return this[MockConnectionResults.DefaultConnectionString]; }

			set { this[MockConnectionResults.DefaultConnectionString] = value; }
		}


		/// <summary>
		/// Property that returns true if can create data source enumerator,
		/// always returns false for the mock object
		/// </summary>
		public override bool CanCreateDataSourceEnumerator
		{
			get { return false; }
		}


		/// <summary>
		/// Property to get the Instance, performs a fully thread-safe lazy
		/// instantiation of the singleton class
		/// </summary>
		public static MockClientFactory Instance
		{
			get { return LazyInstance.Instance; }
		}


		/// <summary>
		/// A list of all the connections created by the mock since last reset.
		/// </summary>
		public List<MockConnection> ConnectionsCreated { get; private set; }


		/// <summary>
		/// A list of all commands created by the mock since last reset.
		/// </summary>
		public List<MockCommand> CommandsCreated { get; private set; }


		/// <summary>
		/// A list of all parameters created by the mock since last reset.
		/// </summary>
		public List<MockParameter> ParametersCreated { get; private set; }


		/// <summary>
		/// Mock property to get the dictionary of connection results
		/// </summary>
		public Dictionary<string, MockConnectionResults> ConnectionResultsMap { get; set; }


		/// <summary>
		/// Private constructor to block public construction
		/// </summary>
		private MockClientFactory()
		{
			ConnectionResultsMap = new Dictionary<string, MockConnectionResults>();

			// create historical lists
			CommandsCreated = new List<MockCommand>();
			ConnectionsCreated = new List<MockConnection>();
			ParametersCreated = new List<MockParameter>();
		}


		/// <summary>
		/// Create a mock command object 
		/// </summary>
		/// <returns>A mock database command</returns>
		public override DbCommand CreateCommand()
		{
			// add to created list and return
			var command = new MockCommand();
			CommandsCreated.Add(command);
			return command;
		}


		/// <summary>
		/// Create a mock connection object
		/// </summary>
		/// <returns>A mock database connection</returns>
		public override DbConnection CreateConnection()
		{
			// add to created list and return
			var connection = new MockConnection();
			ConnectionsCreated.Add(connection);
			return connection;
		}


		/// <summary>
		/// Create a mock data adapter object
		/// </summary>
		/// <returns>A mock database data adapter</returns>
		public override DbDataAdapter CreateDataAdapter()
		{
			return new MockDataAdapter();
		}


		/// <summary>
		/// Create a mock parameter object
		/// </summary>
		/// <returns>A mock database parameter</returns>
		public override DbParameter CreateParameter()
		{
			// add to created list and return
			var parameter = new MockParameter();
			ParametersCreated.Add(parameter);
			return parameter;
		}


		/// <summary>
		/// Create a mock db command buidler - this is not implemented for the
		/// mock db object hierarchy
		/// </summary>
		/// <returns>Nothing, this is not implemented for mocks.</returns>
		/// <exception cref="NotImplementedException">Always throws</exception>
		public override DbCommandBuilder CreateCommandBuilder()
		{
			throw new NotImplementedException("MockProviderFactory does not implement " +
			                                  "CreateCommandBuilder().");
		}


		/// <summary>
		/// Create a mock connection string builder object
		/// </summary>
		/// <returns>A mock connection string builder</returns>
		public override DbConnectionStringBuilder CreateConnectionStringBuilder()
		{
			return new MockConnectionStringBuilder();
		}


		/// <summary>
		/// Create a mock data source enumerator - this is not implemented for the
		/// mock db object hierarchy
		/// </summary>
		/// <returns>Nothing, not implemented for mock.</returns>
		/// <exception cref="NotImplementedException">Always throws</exception>
		public override DbDataSourceEnumerator CreateDataSourceEnumerator()
		{
			throw new NotImplementedException("MockProviderFactory does not implement " +
			                                  "CreateDataSourceEnumerator().");
		}


		/// <summary>
		/// Create a mock code access permission - this is not implemented for the
		/// mock db object hierarchy       
		/// </summary>
		/// <param name="state">The state of the permission to create</param>
		/// <returns>Nothing, always throws</returns>
		/// <exception cref="NotImplementedException">Always throws</exception>
		public override CodeAccessPermission CreatePermission(PermissionState state)
		{
			throw new NotImplementedException("MockProviderFactory does not implement " +
			                                  "CreatePermission().");
		}


		/// <summary>
		/// For external classes to be able to reset MockClientFactory's Instance values.
		/// </summary>
		public void ResetMockResults()
		{
			ConnectionResultsMap.Clear();

			// also reset the history
			ResetHistory();
		}


		/// <summary>
		/// Reset the historical lists of what connections, commands, and parameters were created.
		/// </summary>
		public void ResetHistory()
		{
			ParametersCreated.Clear();
			CommandsCreated.Clear();
			ConnectionsCreated.Clear();
		}


		/// <summary>
		/// Look through provider's singleton results dictionary to find the command results queue element that 
		/// corresponds to this connection/command.
		/// </summary>
		/// <param name="connection">The connection string key for results.</param>
		/// <param name="command">The command string key for results.</param>
		/// <returns>A queue of mock command results specified by the keys.</returns>
		public static Queue<MockCommandResults> GetMockCommandResults(string connection, string command)
		{
			Queue<MockCommandResults> commandResults = null;

			MockConnectionResults connectionResults = GetMockConnectionResults(connection);

			if (connectionResults != null)
			{
				commandResults = connectionResults.GetMockCommandResults(command);
			}

			return commandResults;
		}


		/// <summary>
		/// Look through provider's singleton results dictionary to find the connection result element that 
		/// corresponds to this connection string.
		/// </summary>
		/// <param name="connection">The string key connection string to get results for.</param>
		/// <returns>The mock connection results for the connection string key.</returns>
		public static MockConnectionResults GetMockConnectionResults(string connection)
		{
			MockConnectionResults foundConnection;

			// do NOT create connection if it doesn't exist
			if (!Instance.ConnectionResultsMap.TryGetValue(connection.ToLower(), out foundConnection))
			{
				// but DO create the default connection if it doesn't exist
				foundConnection = Instance.ConnectionResults;
			}

			return foundConnection;
		}


		/// <summary>
		/// Given a connection string, creates the lookup key for that string
		/// </summary>
		/// <param name="connectionString">Given a connection string, returns the key for it.</param>
		/// <returns>key to use for a given connection string.</returns>
		private string GetLookupKey(string connectionString)
		{
			return !string.IsNullOrEmpty(connectionString)
			       	? connectionString
			       	: MockConnectionResults.DefaultConnectionString;
		}


		/// <summary>
		/// Helper method to retrieve mock connection results for a given connection string
		/// </summary>
		/// <param name="connectionString">The connection string key to search.</param>
		/// <returns>The results either created or found for the key.</returns>
		private MockConnectionResults GetOrCreateConnectionResults(string connectionString)
		{
			MockConnectionResults foundConnection;
			string lookup = GetLookupKey(connectionString).ToLower();

			// see if the connection map contains this connectionString key
			if (!ConnectionResultsMap.TryGetValue(lookup, out foundConnection))
			{
				// create new default connection result 
				foundConnection = new MockConnectionResults();
				ConnectionResultsMap.Add(lookup, foundConnection);
			}

			// return value for this dictionary element
			return foundConnection;
		}


		/// <summary>
		/// Helper method to set connection results for a particular connection string
		/// </summary>
		/// <param name="connectionString">The connection string to set results for.</param>
		/// <param name="results">The results to tie to the connection string.</param>
		private void SetConnectionResults(string connectionString, MockConnectionResults results)
		{
			string lookup = GetLookupKey(connectionString).ToLower();

			ConnectionResultsMap[lookup] = results;
		}


		/// <summary>
		/// Yes, we said no inner classes, but it is necessary to make a fully-thread-safe
		/// and lazy singleton a bit more elegant so excuse me on this one.
		/// </summary>
		private static class LazyInstance
		{
			/// <summary>
			/// The singleton instance for the MockClientFactory
			/// </summary>
			internal static readonly MockClientFactory Instance = new MockClientFactory();


			/// <summary>
			/// Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
			/// </summary>
			[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", 
				"SA1409:RemoveUnnecessaryCode",
				Justification = "Reviewed. Suppression is OK here.")]
			static LazyInstance()
			{
				// this static constructor is necessary, do not remove
			}
		}
	}
}