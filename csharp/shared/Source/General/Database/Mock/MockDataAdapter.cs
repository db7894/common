using System.Data;
using System.Data.Common;

using SharedAssemblies.General.Database.Exceptions;

namespace SharedAssemblies.General.Database.Mock
{
    /// <summary>
    /// Mock class that implements the DbDataAdapter abstract class for unit testing
    /// </summary>
    public class MockDataAdapter : DbDataAdapter
    {
        /// <summary>
        /// Overrides DbDataAdapter fill method so that the correct results from MockClientFactory
        /// are loaded into the DataSet.
        /// </summary>
        /// <param name="results">The results DataSet to fill.</param>
        /// <returns>The number of rows filled.</returns>
        public override int Fill(DataSet results)
        {
            int recordNum = 0;

            var found = MockClientFactory.GetMockCommandResults(
				SelectCommand.Connection.ConnectionString,
                SelectCommand.CommandText);

            if (found != null)
            {
                // remove first result from the queue
                MockCommandResults resultFromQueue = found.Dequeue();
                CheckThrowOnExecute(resultFromQueue);

                foreach (DataTable t in resultFromQueue.ResultSet)
                {
                    results.Tables.Add(t);
                    recordNum += t.Rows.Count;
                }
            }

            return recordNum;
        }


        /// <summary>
        /// Checks if should throw an exception on execution.
        /// </summary>
        /// <param name="temp">The mock command results to check.</param>
        private void CheckThrowOnExecute(MockCommandResults temp)
        {
            // check whether this execution should throw
            if (temp.ShouldMockCommandThrowOnExecute)
            {
                throw new DataAccessException("Test requested mock throw exception on ExecuteDataSet.");
            }
        }
    }
}
