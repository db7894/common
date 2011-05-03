using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bashwork.Validation.Tests.Internal;
using Bashwork.Validation.Tests.Types;
using Bashwork.Validation.ValidationRules;

namespace Bashwork.Validation.Tests.ValidationRules
{
	/// <summary>
	/// Code to test the validation methods that tests if the supplied
	/// input meets the specified <see cref="double"/> constraints.
	/// </summary>
	[TestClass]
	public class DoubleValidationTests
	{
		[TestMethod]
		public void ValidateThat_DoubleValidation_ThrowsWithNullContext()
		{
			PropertyContext<ExampleClassType, double> context = null;

			AssertEx.Throws<ArgumentNullException>(() => context.IsNear(1));
			AssertEx.Throws<ArgumentNullException>(() => context.IsNotNear(1));
		}

		[TestMethod]
		public void ValidateThat_SomeValue_IsNear()
		{
			var handle = new ExampleClassType { Balance = 12.34 };

			var results = Validate.That(handle)
				.Property(x => x.Balance).IsNear(12)
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Balance).IsNear(12, 0.01)
				.And().Validate();
			Assert.IsFalse(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Balance).IsNear(15)
				.And().Validate();
			Assert.IsFalse(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Balance).IsNear(15, 3)
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_IsNotNear()
		{
			var handle = new ExampleClassType { Balance = 12.34 };

			var results = Validate.That(handle)
				.Property(x => x.Balance).IsNotNear(12)
				.And().Validate();
			Assert.IsFalse(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Balance).IsNotNear(12, 0.01)
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Balance).IsNotNear(15)
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Balance).IsNotNear(15, 3)
				.And().Validate();
			Assert.IsFalse(results.IsSuccessful);
		}
	}
}
