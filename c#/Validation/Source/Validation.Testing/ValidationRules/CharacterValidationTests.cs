using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.General.Validation.Tests.Internal;
using SharedAssemblies.General.Validation.Tests.Types;
using SharedAssemblies.General.Validation.ValidationRules;

namespace SharedAssemblies.General.Validation.Tests.ValidationRules
{
	/// <summary>
	/// Code to test the validation methods that tests if the supplied
	/// input meets the specified <see cref="char"/> constraints.
	/// </summary>
	[TestClass]
	public class CharacterValidationTests
	{
		[TestMethod]
		public void ValidateThat_CharacterValidation_ThrowsWithNullContext()
		{
			PropertyContext<ExampleClassType, char> context = null;

			AssertEx.Throws<ArgumentNullException>(() => context.IsUppercase());
			AssertEx.Throws<ArgumentNullException>(() => context.IsLowercase());
			AssertEx.Throws<ArgumentNullException>(() => context.IsDigit());
			AssertEx.Throws<ArgumentNullException>(() => context.IsPrintable());
		}

		[TestMethod]
		public void ValidateThat_SomeValue_IsUppercase()
		{
			var handle = new ExampleClassType { Letter = 'A' };

			var results = Validate.That(handle)
				.Property(x => x.Letter).IsUppercase()
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			handle = new ExampleClassType { Letter = 'a' };

			results = Validate.That(handle)
				.Property(x => x.Letter).IsUppercase()
				.And().Validate();
			Assert.IsFalse(results.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_IsLowercase()
		{
			var handle = new ExampleClassType { Letter = 'A' };

			var results = Validate.That(handle)
				.Property(x => x.Letter).IsLowercase()
				.And().Validate();
			Assert.IsFalse(results.IsSuccessful);

			handle = new ExampleClassType { Letter = 'a' };

			results = Validate.That(handle)
				.Property(x => x.Letter).IsLowercase()
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_IsDigit()
		{
			var handle = new ExampleClassType { Letter = 'A' };

			var results = Validate.That(handle)
				.Property(x => x.Letter).IsDigit()
				.And().Validate();
			Assert.IsFalse(results.IsSuccessful);

			handle = new ExampleClassType { Letter = '9' };

			results = Validate.That(handle)
				.Property(x => x.Letter).IsDigit()
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_IsPrintable()
		{
			var handle = new ExampleClassType { Letter = (char)0x00 };

			var results = Validate.That(handle)
				.Property(x => x.Letter).IsPrintable()
				.And().Validate();
			Assert.IsFalse(results.IsSuccessful);

			handle = new ExampleClassType { Letter = 'a' };

			results = Validate.That(handle)
				.Property(x => x.Letter).IsPrintable()
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);
		}
	}
}
