using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharedAssemblies.General.Validation.UnitTests
{
    /// <summary>
    /// Unit tests for binary and unary validators
    /// </summary>
    [TestClass]
    public class BinaryRequestValidatorTest
    {
		/// <summary>
		/// Gets or sets the test context which provides
		/// information about and functionality for the current test run.
		/// </summary>
		public TestContext TestContext { get; set; }


		/// <summary>
        /// test method that returns true always
        /// </summary>
        /// <param name="request">the request to test</param>
        /// <param name="response">the response to test</param>
        /// <returns>true, always</returns>
        public bool BinaryReturnsTrue(DummyRequest request, DummyResponse response)
        {
            ++request.CallCount;
            ++response.CallCount;
            return true;
        }


        /// <summary>
        /// test method that returns false always
        /// </summary>
        /// <param name="request">the request to test</param>
        /// <param name="response">the response to test</param>
        /// <returns>false, always</returns>
        public bool BinaryReturnsFalse(DummyRequest request, DummyResponse response)
        {
            ++request.CallCount;
            ++response.CallCount;
            return false;
        }


        /// <summary>
        /// Test that validate returns false when empty
        /// </summary>
        [TestMethod]
        public void BinaryValidate_ReturnsFalse_WhenEmptyList()
        {
            var actual = new RequestValidator<DummyRequest, DummyResponse>();

            DummyRequest request = new DummyRequest();
            DummyResponse response = new DummyResponse();

            Assert.IsFalse(actual.Validate(request, response));
        }

        /// <summary>
        /// Validate returns true when all return true
        /// </summary>
        [TestMethod]
        public void BinaryValidate_ReturnsTrue_WhenAllReturnTrue()
        {
            var actual = new RequestValidator<DummyRequest, DummyResponse>
                             {
                                 BinaryReturnsTrue, 
                                 BinaryReturnsTrue, 
                                 BinaryReturnsTrue
                             };

            DummyRequest request = new DummyRequest();
            DummyResponse response = new DummyResponse();

            Assert.IsTrue(actual.Validate(request, response));
        }


        /// <summary>
        /// Validate() returns false when all return false
        /// </summary>
        [TestMethod]
        public void BinaryValidate_ReturnsFalse_WhenAllReturnFalse()
        {
            var actual = new RequestValidator<DummyRequest, DummyResponse>
                             {
                                 BinaryReturnsFalse, 
                                 BinaryReturnsFalse, 
                                 BinaryReturnsFalse
                             };

            DummyRequest request = new DummyRequest();
            DummyResponse response = new DummyResponse();

            Assert.IsFalse(actual.Validate(request, response));
        }


        /// <summary>
        /// Validate count is three when all three return true
        /// </summary>
        [TestMethod]
        public void BinaryValidate_CountIsThree_WhenAllThreeReturnTrue()
        {
            var actual = new RequestValidator<DummyRequest, DummyResponse>
                             {
                                 BinaryReturnsTrue, 
                                 BinaryReturnsTrue, 
                                 BinaryReturnsTrue
                             };

            DummyRequest request = new DummyRequest();
            DummyResponse response = new DummyResponse();

            actual.Validate(request, response);

            Assert.AreEqual(3, request.CallCount);
            Assert.AreEqual(3, response.CallCount);
        }


        /// <summary>
        /// Validate() count is one when all return fasle
        /// </summary>
        [TestMethod]
        public void BinaryValidate_CountIsOne_WhenAllReturnFalse()
        {
            var actual = new RequestValidator<DummyRequest, DummyResponse>
                             {
                                 BinaryReturnsFalse, 
                                 BinaryReturnsFalse, 
                                 BinaryReturnsFalse
                             };

            DummyRequest request = new DummyRequest();
            DummyResponse response = new DummyResponse();

            actual.Validate(request, response);

            Assert.AreEqual(1, request.CallCount);
            Assert.AreEqual(1, response.CallCount);
        }


        /// <summary>
        /// Validate failrs on first when first returns false
        /// </summary>
        [TestMethod]
        public void BinaryValidate_FailsOnFirst_WhenOnlyFirstReturnsFalse()
        {
            var actual = new RequestValidator<DummyRequest, DummyResponse>
                             {
                                 BinaryReturnsFalse, 
                                 BinaryReturnsTrue, 
                                 BinaryReturnsTrue
                             };

            DummyRequest request = new DummyRequest();
            DummyResponse response = new DummyResponse();

            actual.Validate(request, response);

            Assert.AreEqual(1, request.CallCount);
            Assert.AreEqual(1, response.CallCount);
        }


        /// <summary>
        /// Validate() fails on middle when only middle return false
        /// </summary>
        [TestMethod]
        public void BinaryValidate_FailsOnMiddle_WhenOnlyMiddleReturnsFalse()
        {
            var actual = new RequestValidator<DummyRequest, DummyResponse>
                             {
                                 BinaryReturnsTrue, 
                                 BinaryReturnsFalse, 
                                 BinaryReturnsTrue
                             };

            DummyRequest request = new DummyRequest();
            DummyResponse response = new DummyResponse();

            actual.Validate(request, response);

            Assert.AreEqual(2, request.CallCount);
            Assert.AreEqual(2, response.CallCount);
        }


        /// <summary>
        /// Validate() fails on last, when only last returns false
        /// </summary>
        [TestMethod]
        public void BinaryValidate_FailsOnLast_WhenOnlyLastReturnsFalse()
        {
            var actual = new RequestValidator<DummyRequest, DummyResponse>
                             {
                                 BinaryReturnsTrue, 
                                 BinaryReturnsTrue, 
                                 BinaryReturnsFalse
                             };

            DummyRequest request = new DummyRequest();
            DummyResponse response = new DummyResponse();

            actual.Validate(request, response);

            Assert.AreEqual(3, request.CallCount);
            Assert.AreEqual(3, response.CallCount);
        }

	
		/// <summary>
		/// Dummy class for validation testing
		/// </summary>
		public class DummyRequest
		{
			/// <summary>
			/// Number of times called
			/// </summary>
			public int CallCount { get; set; }
		}


		/// <summary>
		/// Dummy class for validation testing
		/// </summary>
		public class DummyResponse
		{
			/// <summary>
			/// Number of times called
			/// </summary>
			public int CallCount { get; set; }
		}
	}
}