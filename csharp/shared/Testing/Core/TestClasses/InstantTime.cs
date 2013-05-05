using System;


namespace SharedAssemblies.Core.UnitTests.TestClasses
{
    /// <summary>
    /// Class that is just used for the singleton unit tests
    /// </summary>
    internal class InstantTime
    {
        /// <summary>
        /// the current date and time
        /// </summary>
        private readonly DateTime _time = DateTime.Now;

        /// <summary>
        /// accessor for the time constructed
        /// </summary>
        public DateTime Time
        {
            get { return _time; }
        }
    }
}
