using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.General.Validation.Tests.Internal;
using SharedAssemblies.General.Validation.Tests.Types;
using SharedAssemblies.General.Validation.ValidationRules;

namespace SharedAssemblies.General.Validation.Tests.ValidationRules
{
	/// <summary>
	/// Code to test the validation methods that check if the supplied
	/// input value is equal to the test value.
	/// </summary>
	[TestClass]
	public class EqualToValidationTests
	{
		[TestMethod]
		public void ValidateThat_EqualToValidation_ThrowsWithNullContext()
		{
			PropertyContext<ExampleClassType, double> context = null;

			AssertEx.Throws<ArgumentNullException>(() => context.IsEqualTo(1.0));
			AssertEx.Throws<ArgumentNullException>(() => context.IsNotEqualTo(1.0));
		}

		[TestMethod]
		public void ValidateThat_SomeValue_IsEqualTo()
		{
			var handle = new ExampleClassType { Age = 25 };

			var results = Validate.That(handle)
				.Property(x => x.Age).IsEqualTo(25)
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Age).IsEqualTo(24)
				.And().Validate();
			Assert.IsFalse(results.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_IsNotEqualTo()
		{
			var handle = new ExampleClassType { Age = 25 };

			var results = Validate.That(handle)
				.Property(x => x.Age).IsNotEqualTo(25)
				.And().Validate();
			Assert.IsFalse(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Age).IsNotEqualTo(24)
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_VariousTypes_WillWorkWith_IsEqualTo()
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
				.Property(x => x.Letter).IsEqualTo('B')
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Birthday).IsEqualTo(date)
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Insured).IsEqualTo(true)
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Prefix).IsEqualTo("Mr")
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Time).IsEqualTo(TimeSpan.FromMinutes(1))
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Balance).IsEqualTo(12.34)
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);
		}
	}
}
