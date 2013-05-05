using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bashwork.General.Validation.Tests.Internal;
using Bashwork.General.Validation.Tests.Types;
using Bashwork.General.Validation.Rules;

namespace Bashwork.General.Validation.Tests.Rules
{
	/// <summary>
	/// A collection of validation methods that check if the supplied
	/// input value is greater than the requested test value.
	/// </summary>
	[TestClass]
	public class GreaterThanValidationTests
	{
		[TestMethod]
		public void ValidateThat_GreaterThanValidation_ThrowsWithNullContext()
		{
			PropertyContext<ExampleClassType, double> context = null;

			AssertEx.Throws<ArgumentNullException>(() => context.IsGreaterThan(1.0));
		}

		[TestMethod]
		public void ValidateThat_SomeValue_IsGreaterThan()
		{
			var handle = new ExampleClassType { Age = 25 };

			var result = Validate.That<ExampleClassType>()
				.Property(x => x.Age).IsGreaterThan(0).And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.Age).IsGreaterThan(100).And()
				.Compile().Value.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.Age).IsGreaterThan(25).And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.Age).IsGreaterThan(25, RangeComparison.Inclusive).And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.Age).IsGreaterThan(25, RangeComparison.Exclusive).And()
				.Compile().Value.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_VariousTypes_WillWorkWith_IsGreaterThan()
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
				.Property(x => x.Letter).IsGreaterThan('A').And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.Birthday).IsGreaterThan(date.AddDays(-1)).And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.Insured).IsGreaterThan(false).And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.Prefix).IsGreaterThan("Jr").And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.Time).IsGreaterThan(TimeSpan.FromSeconds(1)).And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.Balance).IsGreaterThan(11.00).And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);
		}
	}
}
