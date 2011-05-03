using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.General.Validation.Tests.Types;
using SharedAssemblies.General.Validation.ValidationRules;

namespace SharedAssemblies.General.Validation.Tests.ValidationRules
{
	/// <summary>
	/// Code to test the validation methods that check if the supplied
	/// input meets the specified not null / empty constraints.
	/// </summary>
	[TestClass]
	public class IsNotNullValidationTests
	{
		[TestMethod]
		public void ValidateThat_SomeValue_IsNotNull()
		{
			var handle = new ExampleClassType { Prefix = null };

			var results = Validate.That(handle)
				.Property(x => x.Prefix).IsNotNull().And().Validate();
			Assert.IsFalse(results.IsSuccessful);

			handle = new ExampleClassType { Prefix = "Mr." };

			results = Validate.That(handle)
				.Property(x => x.Prefix).IsNotNull().And().Validate();
			Assert.IsTrue(results.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValidationValue_IsNotNull()
		{
			var handle = new ExampleClassType { Prefix = null };

			var results = Validate.That(handle).IsNotNull().Validate();
			Assert.IsTrue(results.IsSuccessful);

			handle = null;

			results = Validate.That(handle).IsNotNull().Validate();
			Assert.IsFalse(results.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_IsStructNotNull()
		{
			var handle = new ExampleClassType { NullableType = null };

			var results = Validate.That(handle)
				.Property(x => x.NullableType).IsNotNull().And().Validate();
			Assert.IsFalse(results.IsSuccessful);

			handle = new ExampleClassType { NullableType = 1 };

			results = Validate.That(handle)
				.Property(x => x.NullableType).IsNotNull().And().Validate();
			Assert.IsTrue(results.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValidationValue_IsStructNotNull()
		{
			TimeSpan? handle = null;

			var results = Validate.That(handle).IsNotNull().Validate();
			Assert.IsFalse(results.IsSuccessful);

			handle = TimeSpan.FromHours(1);

			results = Validate.That(handle).IsNotNull().Validate();
			Assert.IsTrue(results.IsSuccessful);
		}

		//[TestMethod]
		//public void ValidateThat_SomeValidationValue_IsNotEmpty()
		//{
		//    var handle = new ExampleClassType { Collection = null, };

		//    var results = Validate.That(handle)
		//        .Property(x => x.Collection).IsNotEmpty()
		//        .And().Validate();
		//    Assert.IsFalse(results.IsSuccessful);

		//    handle.Collection = new List<char> { 'a', 'b', 'c' };

		//    results = Validate.That(handle)
		//        .Property(x => x.Collection).IsNotEmpty()
		//        .And().Validate();
		//    Assert.IsTrue(results.IsSuccessful);
		//}

		//[TestMethod]
		//public void ValidateThat_SomeValidationValue_IsNotNullOrEmpty()
		//{
		//    var handle = new ExampleClassType { Collection = null, };

		//    var results = Validate.That(handle)
		//        .Property(x => x.Collection).IsNotNullOrEmpty()
		//        .And().Validate();
		//    Assert.IsFalse(results.IsSuccessful);

		//    handle.Collection = new List<char> { 'a', 'b', 'c' };

		//    results = Validate.That(handle)
		//        .Property(x => x.Collection).IsNotNullOrEmpty()
		//        .And().Validate();
		//    Assert.IsTrue(results.IsSuccessful);
		//}
	}
}
