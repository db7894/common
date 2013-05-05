using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bashwork.General.Validation.Tests
{
	/// <summary>
	/// Code to test the various extension methods attatched to the validation context.
	/// </summary>
	[TestClass]
	public class ValidationResultTests
	{
		[TestMethod]
		public void ValidationResult_HasValid_Defaults()
		{
			var result = new ValidationResult();

			Assert.IsTrue(result.IsSuccessful);
			Assert.IsNotNull(result.Failures);
		}
	}
}
