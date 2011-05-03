using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bashwork.Validation.Tests.Internal;
using Bashwork.Validation.Tests.Types;
using Bashwork.Validation.ValidationRules;

namespace Bashwork.Validation.Tests.ValidationRules
{
	[TestClass]
	public class IgnoreValidationTests
	{
		[TestMethod]
		public void ValidateThat_IgnoreValidation_ThrowsWithNullContext()
		{
			PropertyContext<ExampleClassType, double> context = null;

			AssertEx.Throws<ArgumentNullException>(() => context.IgnoresValidation());
		}

		[TestMethod]
		public void ValidateThat_ValidationIgnoresValidation_ValidationThrowsWithNullContext()
		{
			ValidationContext<ExampleClassType> context = null;

			AssertEx.Throws<ArgumentNullException>(() => context.IgnoresValidation());
		}

		[TestMethod]
		public void ValidateThat_SomeValue_IsIgnored()
		{
			var handle = new ExampleClassType { Age = 25 };

			var results = Validate.That(handle)
				.Property(x => x.Age).IgnoresValidation()
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_ValidationIsIgnored()
		{
			var handle = new ExampleClassType { Age = 25 };

			var results = Validate.That(handle).IgnoresValidation()
				.Validate();
			Assert.IsTrue(results.IsSuccessful);
		}
	}
}
