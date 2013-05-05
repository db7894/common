using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bashwork.General.Validation.Tests.Internal;
using Bashwork.General.Validation.Tests.Types;
using Bashwork.General.Validation.Rules;
using System.Collections.Generic;

namespace Bashwork.General.Validation.Tests.Rules
{
	/// <summary>
	/// Code to test the validation methods that check if the supplied
	/// input collection is a unique collection.
	/// </summary>
	[TestClass]
	public class UniqueCollectionValidationTests
	{
		[TestMethod]
		public void ValidateThat_SomeValue_IsUnique()
		{
			var validator = Validate.That<ExampleClassType>()
				.Property(x => x.Collection).IsUnique().And()
				.Compile().Value;

			var handle = new ExampleClassType
			{
				Collection = new List<char> { 'a', 'b', 'c' },
			};
			var result = validator.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			handle.Collection = new List<char> { 'a', 'a', 'a' };
			result = validator.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_IsNotUnique()
		{
			var validator = Validate.That<ExampleClassType>()
				.Property(x => x.Collection).IsNotUnique().And()
				.Compile().Value;

			var handle = new ExampleClassType
			{
				Collection = new List<char> { 'a', 'b', 'c' },
			};
			var result = validator.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);

			handle.Collection = new List<char> { 'a', 'a', 'a' };
			result = validator.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);
		}
	}
}
