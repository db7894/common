using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.General.Validation.Tests.Internal;
using SharedAssemblies.General.Validation.Tests.Types;
using SharedAssemblies.General.Validation.ValidationRules;

namespace SharedAssemblies.General.Validation.Tests.ValidationRules
{
	/// <summary>
	/// A collection of validation methods that check if the supplied
	/// input value is one of the supplied values.
	/// </summary>
	[TestClass]
	public class EqualsOneOfValidationTests
	{
		[TestMethod]
		public void ValidateThat_EqualsOneOfValidation_ThrowsWithNullContext()
		{
			PropertyContext<ExampleClassType, double> context = null;

			AssertEx.Throws<ArgumentNullException>(() => context.EqualsOneOf(1.0));
			AssertEx.Throws<ArgumentNullException>(() => context.EqualsOneOf(new []{ 1.0 }));
			AssertEx.Throws<ArgumentNullException>(() => context.DoesNotEqualOneOf(1.0));
			AssertEx.Throws<ArgumentNullException>(() => context.DoesNotEqualOneOf(new[] { 1.0 }));
		}

		[TestMethod]
		public void ValidateThat_SomeValue_EqualsOneOf()
		{
			var handle = new ExampleClassType { Age = 25 };

			var results = Validate.That(handle)
				.Property(x => x.Age).EqualsOneOf(24, 25, 26).And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Age).EqualsOneOf(1, 2, 3).And().Validate();
			Assert.IsFalse(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Age).EqualsOneOf(new [] { 24, 25, 26 }).And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Age).EqualsOneOf(new []{ 1, 2, 3 }).And().Validate();
			Assert.IsFalse(results.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_DoesNotEqualOneOf()
		{
			var handle = new ExampleClassType { Age = 25 };

			var results = Validate.That(handle)
				.Property(x => x.Age).DoesNotEqualOneOf(24, 25, 26).And().Validate();
			Assert.IsFalse(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Age).DoesNotEqualOneOf(1, 2, 3).And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Age).DoesNotEqualOneOf(new[] { 24, 25, 26 }).And().Validate();
			Assert.IsFalse(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Age).DoesNotEqualOneOf(new[] { 1, 2, 3 }).And().Validate();
			Assert.IsTrue(results.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_VariousTypes_WillWorkWith_EqualsOneOf()
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
				.Property(x => x.Letter).EqualsOneOf('A', 'B', 'C').And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Birthday).EqualsOneOf(date).And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Insured).EqualsOneOf(true, false).And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Prefix).EqualsOneOf("Mr", "Mrs").And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Time).EqualsOneOf(TimeSpan.FromMinutes(1), TimeSpan.FromTicks(1)).And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Balance).EqualsOneOf(12.34, 56.78).And().Validate();
			Assert.IsTrue(results.IsSuccessful);
		}
	}
}
