using System;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bashwork.Validation.Tests.Internal;
using Bashwork.Validation.Tests.Types;
using Bashwork.Validation.ValidationRules;

namespace Bashwork.Validation.Tests.ValidationRules
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
			var handle = new ExampleClassType { FirstName = "Mr. Bjangles" };

			var results = Validate.That(handle)
				.Property(x => x.FirstName).Matches(@"\w*\. \w*")
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.FirstName).Matches(@"[0-9]+")
				.And().Validate();
			Assert.IsFalse(results.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_MatchesRegex()
		{
			var handle = new ExampleClassType { FirstName = "Mr. Bjangles" };

			var results = Validate.That(handle)
				.Property(x => x.FirstName).Matches(new Regex(@"\w*\. \w*"))
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.FirstName).Matches(new Regex(@"[0-9]+"))
				.And().Validate();
			Assert.IsFalse(results.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_DoesNotMatch()
		{
			var handle = new ExampleClassType { FirstName = "Mr. Bjangles" };

			var results = Validate.That(handle)
				.Property(x => x.FirstName).DoesNotMatch(@"\w*\. \w*")
				.And().Validate();
			Assert.IsFalse(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.FirstName).DoesNotMatch(@"[0-9]+")
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_DoesNotMatchRegex()
		{
			var handle = new ExampleClassType { FirstName = "Mr. Bjangles" };

			var results = Validate.That(handle)
				.Property(x => x.FirstName).DoesNotMatch(new Regex(@"\w*\. \w*"))
				.And().Validate();
			Assert.IsFalse(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.FirstName).DoesNotMatch(new Regex(@"[0-9]+"))
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ValidateThat_SomeValue_WithNullStringThrows()
		{
			var handle = new ExampleClassType { FirstName = "Mr. Bjangles" };

			var results = Validate.That(handle)
				.Property(x => x.FirstName).DoesNotMatch((string)null)
				.And().Validate();
			Assert.IsFalse(results.IsSuccessful);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ValidateThat_SomeValue_WithNullRegexThrows()
		{
			var handle = new ExampleClassType { FirstName = "Mr. Bjangles" };

			var results = Validate.That(handle)
				.Property(x => x.FirstName).DoesNotMatch((Regex)null)
				.And().Validate();
			Assert.IsFalse(results.IsSuccessful);
		}
	}
}
