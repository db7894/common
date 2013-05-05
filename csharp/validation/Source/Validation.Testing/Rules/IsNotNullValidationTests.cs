using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bashwork.General.Validation.Tests.Types;
using Bashwork.General.Validation.Rules;
using System.Collections.Generic;

namespace Bashwork.General.Validation.Tests.Rules
{
	/// <summary>
	/// Code to test the validation methods that check if the supplied
	/// input meets the specified not null / empty constraints.
	/// </summary>
	[TestClass]
	public class IsNotNullValidationTests
	{
		[TestMethod]
		public void ValidateThat_SomeValue_IsNotNull()
		{
			var validator = Validate.That<ExampleClassType>()
				.Property(x => x.Prefix).IsNotNull().And()
				.Compile().Value;
	
			var handle = new ExampleClassType { Prefix = null };
			var result = validator.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);

			handle = new ExampleClassType { Prefix = "Mr." };
			result = validator.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValidationValue_IsNotNull()
		{
			var validator = Validate.That<ExampleClassType>()
				.IsNotNull()
				.Compile().Value;

			var handle = new ExampleClassType { Prefix = null };
			var result = validator.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			handle = null;
			result = validator.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_IsStructNotNull()
		{
			var validator = Validate.That<ExampleClassType>()
				.Property(x => x.NullableType).IsNotNull().And()
				.Compile().Value;

			var handle = new ExampleClassType { NullableType = null };
			var result = validator.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);

			handle = new ExampleClassType { NullableType = 1 };
			result = validator.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValidationValue_IsStructNotNull()
		{
			var validator = Validate.That<TimeSpan?>()
				.IsNotNull()
				.Compile().Value;

			TimeSpan? handle = null;
			var result = validator.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);

			handle = TimeSpan.FromHours(1);
			result = validator.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValidationValue_IsNotEmpty()
		{
			var validator = Validate.That<ExampleClassType>()
				.Property(x => x.Collection).IsNotEmpty().And()
				.Compile().Value;

			var handle = new ExampleClassType { Collection = null, };
			var result = validator.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);

			handle.Collection = new List<char> { 'a', 'b', 'c' };
			result = validator.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValidationValue_IsNotNullOrEmpty()
		{
			var validator = Validate.That<ExampleClassType>()
				.Property(x => x.Collection).IsNotNullOrEmpty().And()
				.Compile().Value;

			var handle = new ExampleClassType { Collection = null, };
			var result = validator.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);

			handle.Collection = new List<char> { 'a', 'b', 'c' };
			result = validator.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);
		}
	}
}
