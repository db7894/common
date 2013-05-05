using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bashwork.General.Validation.Tests.Types;
using Bashwork.General.Validation.Rules;

namespace Bashwork.General.Validation.Tests.Rules
{
	/// <summary>
	/// Code to test the validation methods that tests if the supplied
	/// input meets the specified <see cref="int"/> constraints.
	/// </summary>
	[TestClass]
	public class IntegerValidationTests
	{
		[TestMethod]
		public void ValidateThat_SomeValue_IsEven()
		{
			var validator = Validate.That<ExampleClassType>()
				.Property(x => x.Age).IsEven().And()
				.Compile().Value;

			var handle = new ExampleClassType { Age = 22 };
			var result = validator.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			handle = new ExampleClassType { Age = 23 };
			result = validator.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_IsOdd()
		{
			var validator = Validate.That<ExampleClassType>()
				.Property(x => x.Age).IsOdd().And()
				.Compile().Value;

			var handle = new ExampleClassType { Age = 22 };
			var result = validator.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);

			handle = new ExampleClassType { Age = 23 };
			result = validator.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_IsMultipleOf()
		{
			var validator = Validate.That<ExampleClassType>()
				.Property(x => x.Age).IsMultipleOf(11).And()
				.Compile().Value;

			var handle = new ExampleClassType { Age = 22 };
			var result = validator.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			handle = new ExampleClassType { Age = 23 };
			result = validator.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_IsNotMultipleOf()
		{
			var validator = Validate.That<ExampleClassType>()
				.Property(x => x.Age).IsNotMultipleOf(11).And()
				.Compile().Value;

			var handle = new ExampleClassType { Age = 22 };
			var result = validator.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);

			handle = new ExampleClassType { Age = 23 };
			result = validator.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);
		}
	}
}
