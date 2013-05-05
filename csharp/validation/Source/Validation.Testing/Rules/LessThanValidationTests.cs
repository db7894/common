using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bashwork.General.Validation.Tests.Internal;
using Bashwork.General.Validation.Tests.Types;
using Bashwork.General.Validation.Rules;

namespace Bashwork.General.Validation.Tests.Rules
{
	/// <summary>
	/// Code to test the validation methods that check if the supplied
	/// input value is less than the requested test value.
	/// </summary>
	[TestClass]
	public class LessThanValidationTests
	{
		[TestMethod]
		public void ValidateThat_LessThanValidation_ThrowsWithNullContext()
		{
			PropertyContext<ExampleClassType, double> context = null;

			AssertEx.Throws<ArgumentNullException>(() => context.IsLessThan(1.0));
		}

		[TestMethod]
		public void ValidateThat_SomeValue_IsLessThan()
		{
			var handle = new ExampleClassType { Age = 25 };

			var result = Validate.That<ExampleClassType>()
				.Property(x => x.Age).IsLessThan(100).And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.Age).IsLessThan(24).And()
				.Compile().Value.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.Age).IsLessThan(25).And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.Age).IsLessThan(25, RangeComparison.Inclusive).And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.Age).IsLessThan(25, RangeComparison.Exclusive).And()
				.Compile().Value.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_VariousTypes_WillWorkWith_IsLessThan()
		{
			var date = DateTime.Now;
			var handle = new ExampleClassType
			{
				Letter = 'B',
				Birthday = date,
				Insured = true,
				Prefix = "Mr",
				Time = TimeSpan.FromMinutes(1),
				Balance = 12.34,
			};

			var result = Validate.That<ExampleClassType>()
				.Property(x => x.Letter).IsLessThan('Z').And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.Birthday).IsLessThan(date.AddDays(1)).And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.Insured).IsLessThan(true).And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.Prefix).IsLessThan("Ms").And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.Time).IsLessThan(TimeSpan.FromHours(1)).And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.Balance).IsLessThan(13.00).And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);
		}
	}
}
