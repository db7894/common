using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bashwork.General.Validation.Tests.Internal;
using Bashwork.General.Validation.Tests.Types;
using Bashwork.General.Validation.Rules;

namespace Bashwork.General.Validation.Tests.Rules
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
			var validator = Validate.That<ExampleClassType>()
				.Property(x => x.Age).IsEqualTo(25).And()
				.Compile().Value;

			var handle = new ExampleClassType { Age = 25 };
			var result = validator.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			handle = new ExampleClassType { Age = 24 };
			result = validator.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_IsNotEqualTo()
		{
			var validator = Validate.That<ExampleClassType>()
				.Property(x => x.Age).IsNotEqualTo(25).And()
				.Compile().Value;

			var handle = new ExampleClassType { Age = 25 };
			var result = validator.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);

			handle = new ExampleClassType { Age = 24 };
			result = validator.Validate(handle);			
			Assert.IsTrue(result.IsSuccessful);
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

			var result = Validate.That<ExampleClassType>()
				.Property(x => x.Letter).IsEqualTo('B').And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.Birthday).IsEqualTo(date).And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.Insured).IsEqualTo(true).And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.Prefix).IsEqualTo("Mr").And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.Time).IsEqualTo(TimeSpan.FromMinutes(1)).And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.Balance).IsEqualTo(12.34).And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);
		}
	}
}
