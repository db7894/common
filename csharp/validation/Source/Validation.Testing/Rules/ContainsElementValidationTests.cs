using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bashwork.General.Validation.Tests.Internal;
using Bashwork.General.Validation.Tests.Types;
using Bashwork.General.Validation.Rules;

namespace Bashwork.General.Validation.Tests.Rules
{
	/// <summary>
	/// Code to test the validation methods that check if the supplied
	/// input collection contains the specified element.
	/// </summary>
	[TestClass]
	public class ContainsElementValidationTests
	{
		[TestMethod]
		public void ValidateThat_ContainsElement_ThrowsWithNullContext()
		{
			PropertyContext<ExampleClassType, IEnumerable<char>> context = null;

			AssertEx.Throws<ArgumentNullException>(() => context.Contains('a'));
			AssertEx.Throws<ArgumentNullException>(() => context.DoesNotContain('a'));
		}

		[TestMethod]
		public void ValidateThat_SomeValue_IsNull()
		{
			var validator = Validate.That<ExampleClassType>()
				.Property(x => x.Collection).Contains('b').And()
				.Compile().Value;

			var handle = new ExampleClassType
			{
				Collection = null,
			};
			var result = validator.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_Contains()
		{
			var validator = Validate.That<ExampleClassType>()
				.Property(x => x.Collection).Contains('b').And()
				.Compile().Value;

			var handle = new ExampleClassType
			{
				Collection = new List<char> { 'a', 'b', 'c' },
			};
			var result = validator.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			handle = new ExampleClassType
			{
				Collection = new List<char> { 'a', 'a', 'c' },
			};
			result = validator.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_DoesNotContain()
		{
			var validator = Validate.That<ExampleClassType>()
				.Property(x => x.Collection).DoesNotContain('b').And()
				.Compile().Value;

			var handle = new ExampleClassType
			{
				Collection = new List<char> { 'a', 'b', 'c' },
			};
			var result = validator.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);

			handle = new ExampleClassType
			{
				Collection = new List<char> { 'a', 'a', 'c' },
			};
			result = validator.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);
		}
	}
}
