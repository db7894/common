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
	/// input collection contains the specified element.
	/// </summary>
	[TestClass]
	public class ContainsElementValidationTests
	{
		[TestMethod]
		public void ValidateThat_ContainsElement_ThrowsWithNullContext()
		{
			PropertyContext<ExampleClassType, List<char>> context = null;

			AssertEx.Throws<ArgumentNullException>(() => context.Contains('a'));
			AssertEx.Throws<ArgumentNullException>(() => context.DoesNotContain('a'));
		}

		public void ValidateThat_SomeValue_Contains()
		{
			var handle = new ExampleClassType
			{
				Collection = new List<char> { 'a', 'b', 'c' },
			};

			var results = Validate.That(handle)
				.Property(x => x.Collection).Contains('b')
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Collection).Contains('d')
				.And().Validate();
			Assert.IsFalse(results.IsSuccessful);
		}

		public void ValidateThat_SomeValue_DoesNotContain()
		{
			var handle = new ExampleClassType
			{
				Collection = new List<char> { 'a', 'b', 'c' },
			};

			var results = Validate.That(handle)
				.Property(x => x.Collection).DoesNotContain('b')
				.And().Validate();
			Assert.IsFalse(results.IsSuccessful);

			results = Validate.That(handle)
				.Property(x => x.Collection).DoesNotContain('d')
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);
		}
	}
}
