using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharedAssemblies.General.Validation.UnitTests
{
    /// <summary>
    /// Unit tests for binary and unary validators
    /// </summary>
    [TestClass]
    public class UnaryRequestValidatorTest
    {
		/// <summary>
		/// Gets or sets the test context which provides
		/// information about and functionality for the current test run.
		/// </summary>
		public TestContext TestContext { get; set; }


		/// <summary>
        /// test method that returns true always
        /// </summary>
        /// <param name="d">dummy value</param>
        /// <returns>True always</returns>
        public bool UnaryReturnsTrue(Dummy d)
        {
            ++d.CallCount;
            return true;
        }


        /// <summary>
        /// test method that returns false always
        /// </summary>
        /// <param name="d">dummy request</param>
        /// <returns>false always</returns>
        public bool UnaryReturnsFalse(Dummy d)
        {
            ++d.CallCount;
            return false;
        }


        /// <summary>
        /// Validate returns false when empty
        /// </summary>
        [TestMethod]
        public void UnaryValidate_ReturnsFalse_WhenEmptyList()
        {
            var actual = new RequestValidator<Dummy>();

            Assert.IsFalse(actual.Validate(new Dummy()));
        }

        /// <summary>
        /// Validate returns true when all true
        /// </summary>
        [TestMethod]
        public void UnaryValidate_ReturnsTrue_WhenAllReturnTrue()
        {
            var actual = new RequestValidator<Dummy>
                             {
                                 UnaryReturnsTrue, 
                                 UnaryReturnsTrue, 
                                 UnaryReturnsTrue
                             };

            Assert.IsTrue(actual.Validate(new Dummy()));
        }


        /// <summary>
        /// Validate reutrn false when all false
        /// </summary>
        [TestMethod]
        public void UnaryValidate_ReturnsFalse_WhenAllReturnFalse()
        {
            var actual = new RequestValidator<Dummy>
                             {
                                 UnaryReturnsFalse, 
                                 UnaryReturnsFalse, 
                                 UnaryReturnsFalse
                             };

            Assert.IsFalse(actual.Validate(new Dummy()));
        }


        /// <summary>
        /// validate count is three when all three return true
        /// </summary>
        [TestMethod]
        public void UnaryValidate_CountIsThree_WhenAllThreeReturnTrue()
        {
            var actual = new RequestValidator<Dummy>
                             {
                                 UnaryReturnsTrue, 
                                 UnaryReturnsTrue, 
                                 UnaryReturnsTrue
                             };

            Dummy d = new Dummy();
            actual.Validate(d);

            Assert.AreEqual(3, d.CallCount);
        }


        /// <summary>
        /// Validate count is one when all false
        /// </summary>
        [TestMethod]
        public void UnaryValidate_CountIsOne_WhenAllReturnFalse()
        {
            var actual = new RequestValidator<Dummy>
                             {
                                 UnaryReturnsFalse, 
                                 UnaryReturnsFalse, 
                                 UnaryReturnsFalse
                             };

            Dummy d = new Dummy();
            actual.Validate(d);

            Assert.AreEqual(1, d.CallCount);
        }


        /// <summary>
        /// Validate fails on first when only first false
        /// </summary>
        [TestMethod]
        public void UnaryValidate_FailsOnFirst_WhenOnlyFirstReturnsFalse()
        {
            var actual = new RequestValidator<Dummy>
                             {
                                 UnaryReturnsFalse, 
                                 UnaryReturnsTrue, 
                                 UnaryReturnsTrue
                             };

            Dummy d = new Dummy();
            actual.Validate(d);

            Assert.AreEqual(1, d.CallCount);
        }


        /// <summary>
        /// Validate fails on middle when only middle false
        /// </summary>
        [TestMethod]
        public void UnaryValidate_FailsOnMiddle_WhenOnlyMiddleReturnsFalse()
        {
            var actual = new RequestValidator<Dummy>
                             {
                                 UnaryReturnsTrue, 
                                 UnaryReturnsFalse, 
                                 UnaryReturnsTrue
                             };

            Dummy d = new Dummy();
            actual.Validate(d);

            Assert.AreEqual(2, d.CallCount);
        }


        /// <summary>
        /// Validate fails on last when only last false
        /// </summary>
        [TestMethod]
        public void UnaryValidate_FailsOnLast_WhenOnlyLastReturnsFalse()
        {
            var actual = new RequestValidator<Dummy>
                             {
                                 UnaryReturnsTrue, 
                                 UnaryReturnsTrue, 
                                 UnaryReturnsFalse
                             };

            Dummy d = new Dummy();
            actual.Validate(d);

            Assert.AreEqual(3, d.CallCount);
        }


		/// <summary>
		/// Dummy class for validation testing
		/// </summary>
		public class Dummy
		{
			/// <summary>
			/// Number of times called
			/// </summary>
			public int CallCount { get; set; }
		}
	}
}