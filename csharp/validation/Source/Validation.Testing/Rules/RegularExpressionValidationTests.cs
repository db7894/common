using System;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bashwork.General.Validation.Tests.Internal;
using Bashwork.General.Validation.Tests.Types;
using Bashwork.General.Validation.Rules;

namespace Bashwork.General.Validation.Tests.Rules
{
	/// <summary>
	/// Code to test the validation methods that check if the supplied
	/// input value matches the supplied regular expression test.
	/// </summary>
	[TestClass]
	public class RegularExpressionValidationTests
	{
		[TestMethod]
		public void ValidateThat_RegularExpressionValidation_ThrowsWithNullContext()
		{
			PropertyContext<ExampleClassType, string> context = null;

			AssertEx.Throws<ArgumentNullException>(() => context.Matches("123456"));
			AssertEx.Throws<ArgumentNullException>(() => context.Matches(new Regex("123456")));
			AssertEx.Throws<ArgumentNullException>(() => context.DoesNotMatch("123456"));
			AssertEx.Throws<ArgumentNullException>(() => context.DoesNotMatch(new Regex("123456")));
		}

		[TestMethod]
		public void ValidateThat_SomeValue_Matches()
		{
			var validator = Validate.That<ExampleClassType>()
				.Property(x => x.FirstName).Matches(@"\w*\. \w*").And()
				.Compile().Value;

			var handle = new ExampleClassType { FirstName = "Mr. Bjangles" };
			var result = validator.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			handle = new ExampleClassType { FirstName = "123456789" };
			result = validator.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_MatchesRegex()
		{
			var validator = Validate.That<ExampleClassType>()
				.Property(x => x.FirstName).Matches(new Regex(@"\w*\. \w*")).And()
				.Compile().Value;

			var handle = new ExampleClassType { FirstName = "Mr. Bjangles" };
			var result = validator.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			handle = new ExampleClassType { FirstName = "123456789" };
			result = validator.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_DoesNotMatch()
		{
			var validator = Validate.That<ExampleClassType>()
				.Property(x => x.FirstName).DoesNotMatch(@"\w*\. \w*").And()
				.Compile().Value;

			var handle = new ExampleClassType { FirstName = "Mr. Bjangles" };
			var result = validator.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);

			handle = new ExampleClassType { FirstName = "123456789" };
			result = validator.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_DoesNotMatchRegex()
		{
			var validator = Validate.That<ExampleClassType>()
				.Property(x => x.FirstName).DoesNotMatch(new Regex(@"\w*\. \w*")).And()
				.Compile().Value;

			var handle = new ExampleClassType { FirstName = "Mr. Bjangles" };
			var result = validator.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);

			handle = new ExampleClassType { FirstName = "123456789" };
			result = validator.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ValidateThat_SomeValue_WithNullStringThrows()
		{
			var handle = new ExampleClassType { FirstName = "Mr. Bjangles" };
			var result = Validate.That<ExampleClassType>()
				.Property(x => x.FirstName).DoesNotMatch((string)null).And()
				.Compile().Value.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ValidateThat_SomeValue_WithNullRegexThrows()
		{
			var handle = new ExampleClassType { FirstName = "Mr. Bjangles" };
			var result = Validate.That<ExampleClassType>()
				.Property(x => x.FirstName).DoesNotMatch((Regex)null).And()
				.Compile().Value.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);
		}
	}
}
