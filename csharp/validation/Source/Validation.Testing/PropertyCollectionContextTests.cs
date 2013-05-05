using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bashwork.General.Validation.Rules;
using Bashwork.General.Validation.Tests.Types;

namespace Bashwork.General.Validation.Tests
{
	/// <summary>
	/// Code to test the code path for the EachItemIn projection
	/// </summary>
	[TestClass]
	public class PropertyCollectionContextTests
	{
		[TestMethod]
		public void PropertyCollectionContext_SucceedsWith_EachItemIn()
		{
			var handle = new ExampleClassType
			{
				Collection = new[] { 'a', 'b', 'c' },
			};

			var result = Validate.That<ExampleClassType>()
				.EachItem(x => x.Collection).IsLowercase().And()
				.Compile().Value.Validate(handle);

			Assert.IsTrue(result.IsSuccessful);
		}

		[TestMethod]
		public void PropertyCollectionContext_FailsWith_EachItemIn()
		{
			var handle = new ExampleClassType
			{
				Collection = new[] { 'a', 'b', 'c' },
			};

			var result = Validate.That<ExampleClassType>()
				.EachItem(x => x.Collection).IsUppercase().And()
				.Compile().Value.Validate(handle);

			Assert.IsFalse(result.IsSuccessful);
			Assert.AreEqual(1, result.Failures.Count());
		}
	}
}
