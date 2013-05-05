using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bashwork.General.Validation.Tests.Internal;
using Bashwork.General.Validation.Tests.Types;
using Bashwork.General.Validation.Rules;

namespace Bashwork.General.Validation.Tests.Rules
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

			var result = Validate.That<ExampleClassType>()
				.Property(x => x.Balance).IsNear(12).And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.Balance).IsNear(12, 0.01).And()
				.Compile().Value.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.Balance).IsNear(15).And()
				.Compile().Value.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.Balance).IsNear(15, 3).And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_IsNotNear()
		{
			var handle = new ExampleClassType { Balance = 12.34 };

			var result = Validate.That<ExampleClassType>()
				.Property(x => x.Balance).IsNotNear(12).And()
				.Compile().Value.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.Balance).IsNotNear(12, 0.01).And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.Balance).IsNotNear(15).And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.Balance).IsNotNear(15, 3).And()
				.Compile().Value.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);
		}
	}
}
