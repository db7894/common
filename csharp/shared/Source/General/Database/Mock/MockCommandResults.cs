using System.Collections.Generic;
using System.Data;

namespace SharedAssemblies.General.Database.Mock
{
    /// <summary>
    /// A mock command results class containing all the possible query return types 
    /// and a throwOnExecute value.
    /// </summary>
    public class MockCommandResults
    {
        /// <summary>
        /// Constant for the default connection string
        /// </summary>
        public static readonly string DefaultCommandString = "DefaultCommand";
        
        /// <summary>
        /// Default value returned for ExecuteNonQuery() when Set NOCOUNT ON is defined in a stored proc
        /// </summary>
        public static readonly int DefaultRowsAffectedResult = -1;

        /// <summary>
        /// Mock property to get/set whether corresponding mock command should throw on execute
        /// </summary>
        public bool ShouldMockCommandThrowOnExecute { get; set; }

        /// <summary>
        /// Data table property representing rows returned from a mock db command (null by default).
        /// </summary>
        public DataTable[] ResultSet { get; set; }

        /// <summary>
        /// Data property representing the # of rows affected by a mock db command (-1 by default).
        /// </summary>
        public int RowsAffectedResult { get; set; }

        /// <summary>
        /// Data object property representing the scalar result for a mock db command (null by default).
        /// </summary>
        public object ScalarResult { get; set; }

		/// <summary>
		/// Property to allow you to set any desired output parameters or return values you want returned
		/// from the mock of this command.
		/// </summary>
		public Dictionary<string, object> OutParameters { get; private set; }

        /// <summary>
        /// Constructor initializing the should throw variable to false and result sets to 
        /// default values.
        /// </summary>
        public MockCommandResults()
        {
            ResultSet = null;
            RowsAffectedResult = DefaultRowsAffectedResult;
            ScalarResult = null;
            ShouldMockCommandThrowOnExecute = false;
			OutParameters = new Dictionary<string, object>();
        }
    }
}
