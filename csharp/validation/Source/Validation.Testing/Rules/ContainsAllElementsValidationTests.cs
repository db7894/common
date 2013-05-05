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
	/// input collection contains all of the specified elements.
	/// </summary>
	[TestClass]
	public class ContainsAllElementsValidationTests
	{
		[TestMethod]
		public void ValidateThat_ContainsAllElements_ThrowsWithNullContext()
		{
			PropertyContext<ExampleClassType, IEnumerable<char>> context = null;

			AssertEx.Throws<ArgumentNullException>(() => context.ContainsAllOf('a'));
			AssertEx.Throws<ArgumentNullException>(() => context.DoesNotContainAllOf('a'));
			AssertEx.Throws<ArgumentNullException>(() => context.ContainsAllOf(new []{ 'a' }));
			AssertEx.Throws<ArgumentNullException>(() => context.DoesNotContainAllOf(new []{ 'a' }));
		}

		[TestMethod]
		public void ValidateThat_SomeValue_ContainsAllOf()
		{
			var handle = new ExampleClassType
			{
				Collection = new [] { 'a', 'b', 'c' },
			};

			var result = Validate.That<ExampleClassType>()
				.Property(x => x.Collection).ContainsAllOf('a', 'b', 'c').And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.Collection).ContainsAllOf('d', '1', '2').And()
				.Compile().Value.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.Collection).ContainsAllOf(new[] { 'a', 'b', 'c' }).And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.Collection).ContainsAllOf(new[] { 'd', '1', '2' }).And()
				.Compile().Value.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_DoesNotContainAllOf()
		{
			var handle = new ExampleClassType
			{
				Collection = new [] { 'a', 'b', 'c' },
			};

			var result = Validate.That<ExampleClassType>()
				.Property(x => x.Collection).DoesNotContainAllOf('a', 'b', 'c').And()
				.Compile().Value.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.Collection).DoesNotContainAllOf('d', '1', '2').And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.Collection).DoesNotContainAllOf(new[] { 'a', 'b', 'c' }).And()
				.Compile().Value.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.Collection).DoesNotContainAllOf(new[] { 'd', '1', '2' }).And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);
		}
	}
}
