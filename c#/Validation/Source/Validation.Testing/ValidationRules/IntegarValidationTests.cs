using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.General.Validation.Tests.Types;
using SharedAssemblies.General.Validation.ValidationRules;

namespace SharedAssemblies.General.Validation.Tests.ValidationRules
{
	/// <summary>
	/// Code to test the validation methods that tests if the supplied
	/// input meets the specified <see cref="int"/> constraints.
	/// </summary>
	[TestClass]
	public class IntegarValidationTests
	{
		[TestMethod]
		public void ValidateThat_SomeValue_IsEven()
		{
			var handle = new ExampleClassType { Age = 22 };

			var results = Validate.That(handle)
				.Property(x => x.Age).IsEven().And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			handle = new ExampleClassType { Age = 23 };

			results = Validate.That(handle)
				.Property(x => x.Age).IsEven().And().Validate();
			Assert.IsFalse(results.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_IsOdd()
		{
			var handle = new ExampleClassType { Age = 22 };

			var results = Validate.That(handle)
				.Property(x => x.Age).IsOdd().And().Validate();
			Assert.IsFalse(results.IsSuccessful);

			handle = new ExampleClassType { Age = 23 };

			results = Validate.That(handle)
				.Property(x => x.Age).IsOdd().And().Validate();
			Assert.IsTrue(results.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_IsMultipleOf()
		{
			var handle = new ExampleClassType { Age = 22 };

			var results = Validate.That(handle)
				.Property(x => x.Age).IsMultipleOf(11).And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Age).IsMultipleOf(13).And().Validate();
			Assert.IsFalse(results.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_IsNotMultipleOf()
		{
			var handle = new ExampleClassType { Age = 22 };

			var results = Validate.That(handle)
				.Property(x => x.Age).IsNotMultipleOf(11).And().Validate();
			Assert.IsFalse(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Age).IsNotMultipleOf(13).And().Validate();
			Assert.IsTrue(results.IsSuccessful);
		}
	}
}
