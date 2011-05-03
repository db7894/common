using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.General.Validation.Tests.Internal;
using SharedAssemblies.General.Validation.Tests.Types;
using SharedAssemblies.General.Validation.ValidationRules;

namespace SharedAssemblies.General.Validation.Tests.ValidationRules
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
