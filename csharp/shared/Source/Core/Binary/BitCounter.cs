namespace SharedAssemblies.Core.Binary
{
    /// <summary>
    /// A class that can be used to calculate the number of bits in an integer
    /// </summary>
    public sealed class BitCounter
    {
        private readonly int[] _bitCounts = new int[65536];

        /// <summary>
        /// Construct the BitCounter by creating an array of bit counts for quick lookup.
        /// </summary>
        public BitCounter()
        {
            _bitCounts = new int[65536];
            int position1 = -1;
            int position2 = -1;

            // Loop through all the elements and assign them.
            for (int i = 1; i < 65536; i++, position1++)
            {
                // Adjust the positions we read from.
                if (position1 == position2)
                {
                    position1 = 0;
                    position2 = i;
                }

                _bitCounts[i] = _bitCounts[position1] + 1;
            }
        }

        /// <summary>
        /// Counts the bits in an integer value.
        /// </summary>
        /// <param name="value">The value to count the bits in.</param>
        /// <returns>The number of bits set.</returns>
        public int Count(int value)
        {
            // count the bits in the upper and lower halves
            return _bitCounts[value & 65535] + _bitCounts[(value >> 16) & 65535];
        }
    }
}
