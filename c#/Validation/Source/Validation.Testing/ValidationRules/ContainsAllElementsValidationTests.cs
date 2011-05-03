using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.General.Validation.Tests.Internal;
using SharedAssemblies.General.Validation.Tests.Types;
using SharedAssemblies.General.Validation.ValidationRules;

namespace SharedAssemblies.General.Validation.Tests.ValidationRules
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
			PropertyContext<ExampleClassType, List<char>> context = null;

			AssertEx.Throws<ArgumentNullException>(() => context.ContainsAllOf('a'));
			AssertEx.Throws<ArgumentNullException>(() => context.DoesNotContainAllOf('a'));
			AssertEx.Throws<ArgumentNullException>(() => context.ContainsAllOf(new []{ 'a' }));
			AssertEx.Throws<ArgumentNullException>(() => context.DoesNotContainAllOf(new []{ 'a' }));
		}

		public void ValidateThat_SomeValue_ContainsAllOf()
		{
			var handle = new ExampleClassType
			{
				Collection = new List<char> { 'a', 'b', 'c' },
			};

			var results = Validate.That(handle)
				.Property(x => x.Collection).ContainsAllOf('a', 'b', 'c')
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Collection).ContainsAllOf('d', '1', '2')
				.And().Validate();
			Assert.IsFalse(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Collection).ContainsAllOf(new[] { 'a', 'b', 'c' })
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Collection).ContainsAllOf(new[] { 'd', '1', '2' })
				.And().Validate();
			Assert.IsFalse(results.IsSuccessful);
		}

		public void ValidateThat_SomeValue_DoesNotContainAllOf()
		{
			var handle = new ExampleClassType
			{
				Collection = new List<char> { 'a', 'b', 'c' },
			};

			var results = Validate.That(handle)
				.Property(x => x.Collection).DoesNotContainAllOf('a', 'b', 'c')
				.And().Validate();
			Assert.IsFalse(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Collection).DoesNotContainAllOf('d', '1', '2')
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Collection).DoesNotContainAllOf(new[] { 'a', 'b', 'c' })
				.And().Validate();
			Assert.IsFalse(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Collection).DoesNotContainAllOf(new[] { 'd', '1', '2' })
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);
		}
	}
}
