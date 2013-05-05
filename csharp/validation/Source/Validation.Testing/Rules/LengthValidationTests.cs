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
	/// input collection has the specified length.
	/// </summary>
	[TestClass]
	public class LengthValidationTests
	{
		[TestMethod]
		public void ValidateThat_SomeType_LengthValue_WithNull()
		{
			var handle = new ExampleClassType
			{
				Collection = null,
			};

			var result = Validate.That<ExampleClassType>()
				.Property(x => x.Collection).HasLength(0).And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_LengthValue_Inclusive()
		{
			var handle = new ExampleClassType
			{
				Collection = new [] { 'a', 'b', 'c' },
			};

			var result = Validate.That<ExampleClassType>()
				.Property(x => x.Collection).HasLength(3).And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.Collection).HasLength(1, 3).And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.Collection).HasLength(minimum:4).And()
				.Compile().Value.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.Collection).HasLength(1, 2).And()
				.Compile().Value.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_LengthValidation_Exclusive()
		{
			var handle = new ExampleClassType
			{
				Collection = new[] { 'a', 'b', 'c' },
			};

			var result = Validate.That<ExampleClassType>()
				.Property(x => x.Collection).HasLength(maximum: 4, comparison: RangeComparison.Exclusive).And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.Collection).HasLength(1, 4, RangeComparison.Exclusive).And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.Collection).HasLength(4, comparison:RangeComparison.Exclusive).And()
				.Compile().Value.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.Collection).HasLength(1, 2, RangeComparison.Exclusive).And()
				.Compile().Value.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);
		}
	}
}
