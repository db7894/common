using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.General.Validation.Tests.Internal;
using SharedAssemblies.General.Validation.Tests.Types;
using SharedAssemblies.General.Validation.ValidationRules;

namespace SharedAssemblies.General.Validation.Tests.ValidationRules
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

			var results = Validate.That(handle)
				.Property(x => x.Age).IsGreaterThan(0).And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Age).IsGreaterThan(100).And().Validate();
			Assert.IsFalse(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Age).IsGreaterThan(25).And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Age).IsGreaterThan(25, RangeComparison.Inclusive).And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Age).IsGreaterThan(25, RangeComparison.Exclusive).And().Validate();
			Assert.IsFalse(results.IsSuccessful);
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

			var results = Validate.That(handle)
				.Property(x => x.Letter).IsGreaterThan('A').And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Birthday).IsGreaterThan(date.AddDays(-1)).And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Insured).IsGreaterThan(false).And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Prefix).IsGreaterThan("Jr").And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Time).IsGreaterThan(TimeSpan.FromSeconds(1)).And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Balance).IsGreaterThan(11.00).And().Validate();
			Assert.IsTrue(results.IsSuccessful);
		}
	}
}
