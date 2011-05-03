using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.General.Validation.Tests.Internal;
using SharedAssemblies.General.Validation.ValidationRules;
using SharedAssemblies.General.Validation.Tests.Types;

namespace SharedAssemblies.General.Validation.Tests.ValidationRules
{
	/// <summary>
	/// Code to test the validation methods that check if the supplied
	/// input value is between a set of test values.
	/// </summary>
	[TestClass]
	public class BetweenValidationTests
	{
		[TestMethod]
		public void ValidateThat_BetweenValidation_ThrowsWithNullContext()
		{
			PropertyContext<ExampleClassType, string> context = null;

			AssertEx.Throws<ArgumentNullException>(() => context.IsBetween(null, null));
		}

		[TestMethod]
		public void ValidateThat_SomeValue_IsBetween()
		{
			var handle = new ExampleClassType { Age = 25 };

			var results = Validate.That(handle)
				.Property(x => x.Age).IsBetween(0, 100)
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Age).IsBetween(0, 24)
				.And().Validate();
			Assert.IsFalse(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Age).IsBetween(26, 27)
				.And().Validate();
			Assert.IsFalse(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Age).IsBetween(25, 25)
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Age).IsBetween(0, 25, RangeComparison.Inclusive)
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);
			
			results = Validate.That(handle)
				.Property(x => x.Age).IsBetween(0, 25, RangeComparison.Exclusive)
				.And().Validate();
			Assert.IsFalse(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Age).IsBetween(25, 25, RangeComparison.Exclusive)
				.And().Validate();
			Assert.IsFalse(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Age).IsBetween(25, 27, RangeComparison.Exclusive)
				.And().Validate();
			Assert.IsFalse(results.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_IsNotBetween()
		{
			var handle = new ExampleClassType { Age = 25 };

			var results = Validate.That(handle)
				.Property(x => x.Age).IsNotBetween(0, 100)
				.And().Validate();
			Assert.IsFalse(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Age).IsNotBetween(0, 24)
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Age).IsNotBetween(0, 25)
				.And().Validate();
			Assert.IsFalse(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Age).IsNotBetween(0, 25, RangeComparison.Inclusive)
				.And().Validate();
			Assert.IsFalse(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Age).IsNotBetween(0, 25, RangeComparison.Exclusive)
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_VariousTypes_WillWorkWith_IsBetween()
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
				.Property(x => x.Letter).IsBetween('A', 'Z')
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Birthday).IsBetween(date.AddDays(-1), date.AddDays(1))
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Insured).IsBetween(false, true)
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Prefix).IsBetween("Jr", "Ms")
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Time).IsBetween(TimeSpan.FromSeconds(1), TimeSpan.FromHours(1))
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Balance).IsBetween(11.00, 13.00)
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);
		}
	}
}
