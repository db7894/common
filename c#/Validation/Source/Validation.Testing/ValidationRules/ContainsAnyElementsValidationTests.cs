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
	/// input collection contains any of the specified elements.
	/// </summary>
	[TestClass]
	public class ContainsAnyElementsValidationTests
	{
		[TestMethod]
		public void ValidateThat_ContainsAnyElements_ThrowsWithNullContext()
		{
			PropertyContext<ExampleClassType, List<char>> context = null;

			AssertEx.Throws<ArgumentNullException>(() => context.ContainsAnyOf('a'));
			AssertEx.Throws<ArgumentNullException>(() => context.DoesNotContainAnyOf('a'));
			AssertEx.Throws<ArgumentNullException>(() => context.ContainsAnyOf(new[] { 'a' }));
			AssertEx.Throws<ArgumentNullException>(() => context.DoesNotContainAnyOf(new[] { 'a' }));
		}

		public void ValidateThat_SomeValue_ContainsOneOf()
		{
			var handle = new ExampleClassType
			{
				Collection = new List<char> { 'a', 'b', 'c' },
			};

			var results = Validate.That(handle)
				.Property(x => x.Collection).ContainsAnyOf('1', '2', 'b')
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Collection).ContainsAnyOf('d', '1', '2')
				.And().Validate();
			Assert.IsFalse(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Collection).ContainsAnyOf(new []{ '1', '2', 'b' })
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Collection).ContainsAnyOf(new [] { 'd', '1', '2' })
				.And().Validate();
			Assert.IsFalse(results.IsSuccessful);
		}

		public void ValidateThat_SomeValue_DoesNotContainOneOf()
		{
			var handle = new ExampleClassType
			{
				Collection = new List<char> { 'a', 'b', 'c' },
			};

			var results = Validate.That(handle)
				.Property(x => x.Collection).DoesNotContainAnyOf('1', '2', 'b')
				.And().Validate();
			Assert.IsFalse(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Collection).DoesNotContainAnyOf('d', '1', '2')
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Collection).DoesNotContainAnyOf(new[] { '1', '2', 'b' })
				.And().Validate();
			Assert.IsFalse(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Collection).DoesNotContainAnyOf(new[] { 'd', '1', '2' })
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);
		}
	}
}
