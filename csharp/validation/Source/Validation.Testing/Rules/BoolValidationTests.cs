using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bashwork.General.Validation.Tests.Internal;
using Bashwork.General.Validation.Tests.Types;
using Bashwork.General.Validation.Rules;

namespace Bashwork.General.Validation.Tests.Rules
{
	/// <summary>
	/// Code to test the validation methods that tests if the supplied
	/// input meets the specified <see cref="bool"/> constraints.
	/// </summary>
	[TestClass]
	public class BoolValidationTests
	{
		[TestMethod]
		public void ValidateThat_BoolValidation_ThrowsWithNullContext()
		{
			PropertyContext<ExampleClassType, bool> context = null;

			AssertEx.Throws<ArgumentNullException>(() => context.IsFalse());
			AssertEx.Throws<ArgumentNullException>(() => context.IsTrue());
		}

		[TestMethod]
		public void ValidateThat_SomeValue_IsTrue()
		{
			var validator = Validate.That<ExampleClassType>()
				.Property(x => x.Insured).IsTrue().And()
				.Compile().Value;

			var handle = new ExampleClassType { Insured = true };
			var result = validator.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			handle = new ExampleClassType { Insured = false };
			result = validator.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_IsFalse()
		{
			var validator = Validate.That<ExampleClassType>()
				.Property(x => x.Insured).IsFalse().And()
				.Compile().Value;

			var handle = new ExampleClassType { Insured = true };
			var result = validator.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);

			handle = new ExampleClassType { Insured = false };
			result = validator.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeFunction_IsTrue()
		{
			var validator = Validate.That<ExampleClassType>()
				.Property(x => x.Function).IsTrue().And()
				.Compile().Value;

			var handle = new ExampleClassType { Function = () => false };
			var result = validator.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);

			handle = new ExampleClassType { Function = () => true };
			result = validator.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeFunction_IsFalse()
		{
			var validator = Validate.That<ExampleClassType>()
				.Property(x => x.Function).IsFalse().And()
				.Compile().Value;

			var handle = new ExampleClassType { Function = () => true };
			var result = validator.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);

			handle = new ExampleClassType { Function = () => false };
			result = validator.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);
		}
	}
}
