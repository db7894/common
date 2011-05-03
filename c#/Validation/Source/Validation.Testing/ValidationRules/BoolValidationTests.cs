using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bashwork.Validation.Tests.Internal;
using Bashwork.Validation.Tests.Types;
using Bashwork.Validation.ValidationRules;

namespace Bashwork.Validation.Tests.ValidationRules
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
			var handle = new ExampleClassType { Insured = true };

			var results = Validate.That(handle)
				.Property(x => x.Insured).IsTrue()
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			handle = new ExampleClassType { Insured = false };

			results = Validate.That(handle)
				.Property(x => x.Insured).IsTrue()
				.And().Validate();
			Assert.IsFalse(results.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_IsFalse()
		{
			var handle = new ExampleClassType { Insured = true };

			var results = Validate.That(handle)
				.Property(x => x.Insured).IsFalse()
				.And().Validate();
			Assert.IsFalse(results.IsSuccessful);

			handle = new ExampleClassType { Insured = false };

			results = Validate.That(handle)
				.Property(x => x.Insured).IsFalse()
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);
		}
	}
}
