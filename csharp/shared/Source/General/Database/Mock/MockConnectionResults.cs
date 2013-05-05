using System.Collections.Generic;

namespace SharedAssemblies.General.Database.Mock
{
    /// <summary>
    /// A mock connection results class containing a dictionary of MockCommandResults 
    /// and a throwOnOpen, throwOnCommit and throwOnRollback value.
    /// </summary>
    public class MockConnectionResults
    {
        /// <summary>Constant for the default connection string</summary>
        public static readonly string DefaultConnectionString = "DefaultConnection";

        /// <summary>Dictionary of mock commands, whose key is the command text and value</summary>
        private readonly Dictionary<string, Queue<MockCommandResults>> _commandResultsMap;

        /// <summary>
        /// Index to get/set MockCommandResults using array-like syntax
        /// </summary>
        /// <param name="commandText">The text of the command.</param>
        /// <returns>A queue of mock command results for the given command text.</returns>
        public Queue<MockCommandResults> this[string commandText]
        {
            get
            {
                return GetOrCreateMockCommandResults(commandText);
            }

            set
            {
                SetMockCommandResults(commandText, value);
            }
        }


        /// <summary>
        /// Property to get/set the default results if command not specified
        /// </summary>
        public Queue<MockCommandResults> CommandResults
        {
            get
            {
                return this[MockCommandResults.DefaultCommandString];
            }

            set
            {
                this[MockCommandResults.DefaultCommandString] = value;
            }
        }

        /// <summary>
        /// Mock property to get/set whether corresponding mock connection should throw on open
        /// </summary>
        public bool ShouldMockConnectionThrowOnOpen { get; set; }

        /// <summary>
        /// Mock property to get/set whether corresponding mock transaction should throw on commit
        /// </summary>
        public bool ShouldMockTransactionThrowOnCommit { get; set; }

        /// <summary>
        /// Mock property to get/set whether corresponding mock transaction should throw on rollback
        /// </summary>
        public bool ShouldMockTransactionThrowOnRollback { get; set; }

        /// <summary>
        /// Constructor initializing the should throw variables to false.
        /// </summary>
        public MockConnectionResults()
        {
            _commandResultsMap = new Dictionary<string, Queue<MockCommandResults>>();
            ShouldMockConnectionThrowOnOpen = false;
            ShouldMockTransactionThrowOnCommit = false;
            ShouldMockTransactionThrowOnRollback = false;
        }


        /// <summary>
        /// Looks up the command results but does not insert an entry if not found
        /// </summary>
        /// <param name="commandText">The text of the command to get results for.</param>
        /// <returns>The queue of mock command results for the command text.</returns>
        public Queue<MockCommandResults> GetMockCommandResults(string commandText)
        {
            Queue<MockCommandResults> foundCommandQueue;

            // check if in map under name, do NOT create command if does not exist
            if (!_commandResultsMap.TryGetValue(commandText, out foundCommandQueue))
            {
                // this WILL create the default if it doesn't exist
                foundCommandQueue = CommandResults;
            }

            return foundCommandQueue.Count > 0 ? foundCommandQueue : null;
        }


        /// <summary>
        /// Get the results from the map, creating them if they don't exist
        /// </summary>
        /// <param name="commandText">The text of the command to retrieve/create results for.</param>
        /// <returns>The retrieved or created results.</returns>
        private Queue<MockCommandResults> GetOrCreateMockCommandResults(string commandText)
        {
            string lookupKey = GetLookupKey(commandText); 

            Queue<MockCommandResults> foundCommandQueue;

            // see if the command map contains this commandText key
            if (!_commandResultsMap.TryGetValue(lookupKey, out foundCommandQueue))
            {
                // create new default command result queue
                foundCommandQueue = new Queue<MockCommandResults>();
                _commandResultsMap.Add(lookupKey, foundCommandQueue);
            }

            // return value for this dictionary element
            return foundCommandQueue;
        }


        /// <summary>
        /// Set the results in the map
        /// </summary>
        /// <param name="commandText">The command text to set results for.</param>
        /// <param name="results">A queue of results for the command.</param>
        private void SetMockCommandResults(string commandText, Queue<MockCommandResults> results)
        {
            string lookupKey = GetLookupKey(commandText);

            _commandResultsMap[lookupKey] = results;
        }


        /// <summary>
        /// Get the lookup key given the command text
        /// </summary>
        /// <param name="commandText">The text of the command to convert to a key.</param>
        /// <returns>The key from the command string.</returns>
        private string GetLookupKey(string commandText)
        {
            return !string.IsNullOrEmpty(commandText) 
                ? commandText 
                : MockCommandResults.DefaultCommandString;
        }
    }
}
