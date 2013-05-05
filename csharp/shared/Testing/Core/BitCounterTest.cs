using SharedAssemblies.Core.Binary;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace SharedAssemblies.Core.UnitTests
{
    /// <summary>
    /// This is a test class for BitCounterTest and is intended
    /// to contain all BitCounterTest Unit Tests
    /// </summary>
    [TestClass]
    public class BitCounterTest
    {
        /// <summary>
        /// A test for BitCounter Constructor
        /// </summary>
        [TestMethod]
        public void BitCounterConstructorDoesntThrowTest()
        {
            new BitCounter();
        }

        /// <summary>
        /// A test for Count
        /// </summary>
        [TestMethod]
		[Ignore] // too slow for now
        public void CountTest()
        {
            var counter = new BitCounter();

            for (int i = int.MinValue; i < int.MaxValue; i++)
            {
                int expected = 0;
                int mask = 0x01;

                for (int x = 0; x < 32; x++)
                {
                    if ((i & mask) == mask)
                    {
                        expected++;
                    }
                    mask <<= 1;
                }

                var actual = counter.Count(i);

                Assert.AreEqual(expected, actual);
            }
        }
    }
}
