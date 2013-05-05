using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bashwork.General.Validation.Tests.Internal;
using Bashwork.General.Validation.Tests.Types;
using Bashwork.General.Validation.Rules;

namespace Bashwork.General.Validation.Tests.Rules
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
			var validator = Validate.That<ExampleClassType>()
				.Property(x => x.Letter).IsUppercase()
				.And().Compile().Value;

			var handle = new ExampleClassType { Letter = 'A' };
			var result = validator.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			handle = new ExampleClassType { Letter = 'a' };
			result = validator.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_IsLowercase()
		{
			var validator = Validate.That<ExampleClassType>()
				.Property(x => x.Letter).IsLowercase()
				.And().Compile().Value;

			var handle = new ExampleClassType { Letter = 'A' };
			var result = validator.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);

			handle = new ExampleClassType { Letter = 'a' };
			result = validator.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_IsDigit()
		{
			var validator = Validate.That<ExampleClassType>()
				.Property(x => x.Letter).IsDigit()
				.And().Compile().Value;
	
			var handle = new ExampleClassType { Letter = 'A' };
			var result = validator.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);

			handle = new ExampleClassType { Letter = '9' };
			result = validator.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_IsPrintable()
		{
			var validator = Validate.That<ExampleClassType>()
				.Property(x => x.Letter).IsPrintable()
				.And().Compile().Value;

			var handle = new ExampleClassType { Letter = (char)0x00 };
			var result = validator.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);

			handle = new ExampleClassType { Letter = 'a' };
			result = validator.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);
		}
	}
}
