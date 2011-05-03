using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bashwork.Validation.Tests.Internal;
using Bashwork.Validation.Tests.Types;
using Bashwork.Validation.ValidationRules;

namespace Bashwork.Validation.Tests.ValidationRules
{
	/// <summary>
	/// Code to test the validation methods that check if the supplied
	/// input meets the specified <see cref="string"/> constraints.
	/// </summary>
	[TestClass]
	public class StringValidationTests
	{
		[TestMethod]
		public void ValidateThat_RegularExpressionValidation_ThrowsWithNullContext()
		{
			PropertyContext<ExampleClassType, string> context = null;

			AssertEx.Throws<ArgumentNullException>(() => context.Contains("123456"));
			AssertEx.Throws<ArgumentNullException>(() => context.DoesNotContain("123456"));
			AssertEx.Throws<ArgumentNullException>(() => context.StartsWith("123456"));
			AssertEx.Throws<ArgumentNullException>(() => context.DoesNotStartWith("123456"));
			AssertEx.Throws<ArgumentNullException>(() => context.EndsWith("123456"));
			AssertEx.Throws<ArgumentNullException>(() => context.DoesNotEndWith("123456"));
		}

		[TestMethod]
		public void ValidateThat_SomeValue_Contains()
		{
			var handle = new ExampleClassType { FirstName = "Mr. Bjangles" };

			var results = Validate.That(handle)
				.Property(x => x.FirstName).Contains("Mr.")
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.FirstName).Contains("Mrs.")
				.And().Validate();
			Assert.IsFalse(results.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_DoesNotContain()
		{
			var handle = new ExampleClassType { FirstName = "Mr. Bjangles" };

			var results = Validate.That(handle)
				.Property(x => x.FirstName).DoesNotContain("Mr.")
				.And().Validate();
			Assert.IsFalse(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.FirstName).DoesNotContain("Mrs.")
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_StartsWith()
		{
			var handle = new ExampleClassType { FirstName = "Mr. Bjangles" };

			var results = Validate.That(handle)
				.Property(x => x.FirstName).StartsWith("mr.")
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.FirstName).StartsWith("Mrs.")
				.And().Validate();
			Assert.IsFalse(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.FirstName).StartsWith("mr.", false)
				.And().Validate();
			Assert.IsFalse(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.FirstName).StartsWith("Mrs.", false)
				.And().Validate();
			Assert.IsFalse(results.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_DoesNotStartWith()
		{
			var handle = new ExampleClassType { FirstName = "Mr. Bjangles" };

			var results = Validate.That(handle)
				.Property(x => x.FirstName).DoesNotStartWith("mr.")
				.And().Validate();
			Assert.IsFalse(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.FirstName).DoesNotStartWith("Mrs.")
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.FirstName).DoesNotStartWith("mr.", false)
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.FirstName).DoesNotStartWith("Mrs.", false)
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_EndsWith()
		{
			var handle = new ExampleClassType { FirstName = "Mr. Bjangles" };

			var results = Validate.That(handle)
				.Property(x => x.FirstName).EndsWith("les")
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.FirstName).EndsWith("Mrs.")
				.And().Validate();
			Assert.IsFalse(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.FirstName).EndsWith("LES", false)
				.And().Validate();
			Assert.IsFalse(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.FirstName).EndsWith("Mrs.", false)
				.And().Validate();
			Assert.IsFalse(results.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_DoesNotEndWith()
		{
			var handle = new ExampleClassType { FirstName = "Mr. Bjangles" };

			var results = Validate.That(handle)
				.Property(x => x.FirstName).DoesNotEndWith("les")
				.And().Validate();
			Assert.IsFalse(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.FirstName).DoesNotEndWith("Mrs.")
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.FirstName).DoesNotEndWith("LES", false)
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.FirstName).DoesNotEndWith("Mrs.", false)
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);
		}
	}
}
