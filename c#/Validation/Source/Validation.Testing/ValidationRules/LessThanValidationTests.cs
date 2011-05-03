using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bashwork.Validation.Tests.Internal;
using Bashwork.Validation.Tests.Types;
using Bashwork.Validation.ValidationRules;

namespace Bashwork.Validation.Tests.ValidationRules
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

			var results = Validate.That(handle)
				.Property(x => x.Age).IsLessThan(100).And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Age).IsLessThan(24).And().Validate();
			Assert.IsFalse(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Age).IsLessThan(25).And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Age).IsLessThan(25, RangeComparison.Inclusive).And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Age).IsLessThan(25, RangeComparison.Exclusive).And().Validate();
			Assert.IsFalse(results.IsSuccessful);
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

			var results = Validate.That(handle)
				.Property(x => x.Letter).IsLessThan('Z').And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Birthday).IsLessThan(date.AddDays(1)).And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Insured).IsLessThan(true).And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Prefix).IsLessThan("Ms").And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Time).IsLessThan(TimeSpan.FromHours(1)).And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Balance).IsLessThan(13.00).And().Validate();
			Assert.IsTrue(results.IsSuccessful);
		}
	}
}
