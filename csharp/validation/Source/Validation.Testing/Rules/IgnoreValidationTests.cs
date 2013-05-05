using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bashwork.General.Validation.Tests.Internal;
using Bashwork.General.Validation.Tests.Types;
using Bashwork.General.Validation.Rules;

namespace Bashwork.General.Validation.Tests.Rules
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
			var result = Validate.That<ExampleClassType>()
				.Property(x => x.Age).IgnoresValidation().And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_ValidationIsIgnored()
		{
			var handle = new ExampleClassType { Age = 25 };
			var result = Validate.That<ExampleClassType>()
				.IgnoresValidation()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);
		}
	}
}
