using System.Threading;


namespace SharedAssemblies.Core.Math
{
    /// <summary>
    /// Class to perform a running average calculation.
    /// </summary>
    public sealed class RunningAverage
    {
        private long _value = 0;

        /// <summary>
        /// Gets the current average
        /// </summary>
        public double Average
        {
            get
            {
                var result = Interlocked.Read(ref _value);
                var count = (int)(result & uint.MaxValue);
                var value = (int)(result >> 32);

                return count != 0 ? (value / (double)count) : 0.0;
            }
        }

        /// <summary>
        /// Resets the running average to zero.
        /// </summary>
        public void Reset()
        {
            Interlocked.Exchange(ref _value, 0);
        }

        /// <summary>
        /// Adds a new value to the running average
        /// </summary>
        /// <param name="newValue">The new value to add.</param>
        public void Add(int newValue)
        {
            var result = Interlocked.Read(ref _value);
            var count = (int)(result & int.MaxValue);
            var value = (int)(result >> 32);

            result = value + newValue;

            Interlocked.Exchange(ref _value, (result << 32) | (uint)(count + 1));
        }
    }
}
