using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bashwork.General.Validation.Tests
{
	/// <summary>
	/// Code to test the implementation of ValidationException
	/// </summary>
	[TestClass]
	public class ValidationExceptionsTests
	{
		[TestMethod]
		public void ValidationExceptions_Initializes_Correctly()
		{
			var failure = new ValidationFailure
			{
				ErrorMessage = "some error message",
				PropertyName = "property",
			};
			var exception = new ValidationException(failure);
			Assert.AreEqual(1, exception.Descriptions.Count());
			Assert.IsTrue(exception.Descriptions.First().ToString().Contains("property"));

			exception = new ValidationException(new [] { failure, failure });
			Assert.AreEqual(2, exception.Descriptions.Count());
		}

		[TestMethod]
		public void ValidationExceptions_WithNoFailures_Initializes()
		{
			var failures = (IEnumerable<ValidationFailure>)null;
			var exception = new ValidationException(failures);
			Assert.AreEqual(0, exception.Descriptions.Count());
		}
	}
}
