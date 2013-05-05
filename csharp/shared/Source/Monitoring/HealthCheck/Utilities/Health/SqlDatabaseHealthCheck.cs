using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using log4net;
using SharedAssemblies.Core.Extensions;
using SharedAssemblies.Monitoring.HealthCheck.Contracts.Health;
using SharedAssemblies.Monitoring.HealthCheck.Contracts.Web.Health;

namespace SharedAssemblies.Monitoring.HealthCheck.Utilities.Health
{
	/// <summary>
	/// A helper base class to introduce a health check to the DAO
	/// by simply overriding a few properties.
	/// </summary>
	public abstract class SqlDatabaseHealthCheck : IHealthCheck
	{
		/// <summary>
		/// Handle to the health database configuration
		/// </summary>
		protected abstract DatabaseHealthConfig Health { get; set; }

		/// <summary>
		/// A description of the database that we are currently testing
		/// </summary>
		public string DatabaseIdentifier
		{
			get { return _databaseIdentifier.Value; }
		}

		/// <summary>
		/// The backing store for the database name, lazily initialized in a
		/// thread safe manner.
		/// </summary>
		private readonly Lazy<string> _databaseIdentifier;

		/// <summary>
		/// The regular expression used to extract the relevant database fields
		/// from the connection string
		/// </summary>
		private static readonly Regex _databaseIdentifierRegex =
			new Regex(@"server=(?<server>[^;]+);database=(?<database>[^;]+);.*", RegexOptions.Compiled);

		/// <summary>
		/// An alternate regular expression used to extract the relevant database fields
		/// from the connection string
		/// </summary>
		private static readonly Regex _databaseAlternateRegex =
			new Regex(@"Data Source=(?<server>[^;]+);Initial Catalog=(?<database>[^;]+);.*", RegexOptions.Compiled);

		/// <summary>
		/// A collection of sql server error codes that we will trigger
		/// an error response for.
		/// </summary>
		private static readonly List<int> _sqlServerErrorCodes = new List<int>
		{
			229, 230,		// Execute Permission Errors
			18456,			// login failed for user
            10061,          // No connection could be made because the server actively refused
            11001,          // No connection could be made because the host does not exist
		};

		/// <summary>
		/// Logger for recording problems.
		/// </summary>
		private static readonly ILog _log = LogManager.GetLogger(typeof(SqlDatabaseHealthCheck));

		/// <summary>
		/// Default constructor.
		/// </summary>
		protected SqlDatabaseHealthCheck()
		{
			_databaseIdentifier = new Lazy<string>(
				() => ExtractDatabaseIdentifier(Health.Database.ConnectionString));
		}

		/// <summary>
		/// A quick check that indicates if the current dependency is healthy
		/// </summary>
		/// <returns>true if healthy, false otherwise</returns>
		public bool IsHealthy()
		{
			return PerformHealthCheck().IsFunctioning;
		}

		/// <summary>
		/// Runs a test of the procedures to see if we have access to everything
		/// </summary>
		/// <returns>A collection of the stored procedures and their health</returns>
		public HealthTestResult PerformHealthCheck()
		{
			var results = Health.Procedures.NullSafe()
				.Select(GuardedProcedureRunner).ToList();

			return new HealthTestResult
			{
				Name = DatabaseIdentifier,
				IsFunctioning = results.All(result => result.IsFunctioning),
				ResponseMessages = results.Select(result => result.ResponseMessages.First()).ToList(),
			};
		}

		/// <summary>
		/// Helper method to run a stored procedure through a guarded smoke
		/// test and record its result.
		/// </summary>
		/// <param name="procedure">The procedure to test</param>
		/// <returns>The result of the smoke test</returns>
		private HealthTestResult GuardedProcedureRunner(string procedure)
		{
			HealthTestResult result;

			try
			{
				//
				// This condition shouldn't actually happen unless the procedure is
				// called successfully (say with no parameters).
				//
				Health.Database.ExecuteNonQuery(procedure, CommandType.StoredProcedure);
				result = HealthTestResultFactory.GenerateSuccess(procedure, BuildMessage(procedure, true));
			}	
			catch (SqlException ex)
			{
				//
				// This should occur if we can get to the database and attempt to execute
				// a query. Since we are calling the procedure incorrectly, we will be
				// getting some kind of sql error. We only fail if one of those errors
				// is a bad case we care about (permissions, remote execution, etc).
				//
				bool hasError = ex.Errors.Cast<SqlError>()
					.Any(sqlError => _sqlServerErrorCodes.Contains(sqlError.Number));

				result = (hasError)
					? HealthTestResultFactory.GenerateFailure(procedure, BuildMessage(procedure, false, ex.Message))
					: HealthTestResultFactory.GenerateSuccess(procedure, BuildMessage(procedure, true));
			}
			catch (Exception ex)
			{
				//
				// This should occur if there are any errors outside of the database
				// library (sql). These could be things like network connection issues
				// such with a firewall being in the way.
				//
				result = HealthTestResultFactory.GenerateFailure(procedure,
					BuildMessage(procedure, false, ex.Message));
			}

			return result;
		}

		/// <summary>
		/// A helper method to build the response message for the
		/// database health check.
		/// </summary>
		/// <param name="procedure">The name of the procedure</param>
		/// <param name="successful">true if the test was successful, false otherwise</param>
		/// <param name="extra">An extra data to add to the result</param>
		/// <returns>The resulting health check message</returns>
		private static string BuildMessage(string procedure, bool successful, params string[] extra)
		{
			var builder = new StringBuilder();
			builder.Append(successful ? "(Successful) " : "(Failure) ");
			builder.Append(procedure);

			foreach (var field in extra)
			{
				builder.Append(": ");
				builder.Append(field);
			}

			// Right now the health checks are public, so we must not return anything
			// that may be considered harmful (such as stored procedure names).   Instead we
			// can log any failures, and just return a masked name to the web service.
			if (!successful)
			{
				_log.ErrorFormat("SQL Health Check Failure : {0}", builder);
			}
			return "MASKEDVALUE, SEE LOG";
		}
		
		/// <summary>
		/// A helper method to extract the relevant identifiers from the
		/// currently configured connection string.
		/// </summary>
		/// <returns>The compiled database identifier</returns>
		private static string ExtractDatabaseIdentifier(string connection)
		{
			var result = string.Empty;
			var match1 = _databaseIdentifierRegex.Match(connection ?? string.Empty);
			var match2 = _databaseAlternateRegex.Match(connection  ?? string.Empty);

			if (match1.Success)
			{
				// Health checks are public, we don't want to return the server name.
				// result = "[" + match.Groups["database"].Value
				//	+ "][" + match.Groups["server"].Value + "]";
				result = string.Format("[{0}]", match1.Groups["database"].Value);
			}
			else if (match2.Success)
			{
				result = string.Format("[{0}]", match2.Groups["database"].Value);
			}

			return result;
		}
	}
}
