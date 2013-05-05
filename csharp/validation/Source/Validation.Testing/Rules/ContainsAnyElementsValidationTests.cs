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
	/// input collection contains any of the specified elements.
	/// </summary>
	[TestClass]
	public class ContainsAnyElementsValidationTests
	{
		[TestMethod]
		public void ValidateThat_ContainsAnyElements_ThrowsWithNullContext()
		{
			PropertyContext<ExampleClassType, IEnumerable<char>> context = null;

			AssertEx.Throws<ArgumentNullException>(() => context.ContainsAnyOf('a'));
			AssertEx.Throws<ArgumentNullException>(() => context.DoesNotContainAnyOf('a'));
			AssertEx.Throws<ArgumentNullException>(() => context.ContainsAnyOf(new[] { 'a' }));
			AssertEx.Throws<ArgumentNullException>(() => context.DoesNotContainAnyOf(new[] { 'a' }));
		}

		[TestMethod]
		public void ValidateThat_SomeValue_IsNull()
		{
			var elements = (IEnumerable<char>)null;
			var handle = new ExampleClassType
			{
				Collection = null,
			};
			var result = Validate.That<ExampleClassType>()
				.Property(x => x.Collection).ContainsAnyOf('1', '2', 'b').And()
				.Compile().Value.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);

			handle = new ExampleClassType
			{
				Collection = new[] { 'a', 'b', 'c' },
			};			
			result = Validate.That<ExampleClassType>()
				.Property(x => x.Collection).ContainsAnyOf(elements).And()
				.Compile().Value.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_ContainsOneOf()
		{
			var handle = new ExampleClassType
			{
				Collection = new [] { 'a', 'b', 'c' },
			};

			var result = Validate.That<ExampleClassType>()
				.Property(x => x.Collection).ContainsAnyOf('1', '2', 'b').And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.Collection).ContainsAnyOf('d', '1', '2').And()
				.Compile().Value.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.Collection).ContainsAnyOf(new []{ '1', '2', 'b' }).And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.Collection).ContainsAnyOf(new [] { 'd', '1', '2' }).And()
				.Compile().Value.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_DoesNotContainOneOf()
		{
			var handle = new ExampleClassType
			{
				Collection = new [] { 'a', 'b', 'c' },
			};

			var result = Validate.That<ExampleClassType>()
				.Property(x => x.Collection).DoesNotContainAnyOf('1', '2', 'b').And()
				.Compile().Value.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.Collection).DoesNotContainAnyOf('d', '1', '2').And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.Collection).DoesNotContainAnyOf(new[] { '1', '2', 'b' }).And()
				.Compile().Value.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.Collection).DoesNotContainAnyOf(new[] { 'd', '1', '2' }).And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);
		}
	}
}
