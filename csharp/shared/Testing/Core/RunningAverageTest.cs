using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.Core.Math;


namespace SharedAssemblies.Core.UnitTests
{
    /// <summary>
    /// Test Fixture for the RunningAverage class
    /// </summary>
    [TestClass]
    public class RunningAverageTest
    {
        /// <summary>
        /// Test the Average 
        /// </summary>
        [TestMethod]
        public void ConstructorHasAverageOfZero()
        {
            var target = new RunningAverage();

            Assert.AreEqual(0.0, target.Average);
        }

        /// <summary>
        /// Test the Average 
        /// </summary>
        [TestMethod]
        public void AverageTest()
        {
            var total = 0;
            var count = 0;
            var target = new RunningAverage();

            for (int i = 0; i < 1000; i += 3)
            {
                // do our average
                total += i;
                count++;

                // do running average
                target.Add(i);

                // compare to running
                Assert.AreEqual(total / (double)count, target.Average);
            }

            // make sure reset goes back to zero
            target.Reset();
            Assert.AreEqual(0.0, target.Average);
        }
    }
}
