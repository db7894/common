using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bashwork.General.Validation.Tests.Internal;
using Bashwork.General.Validation.Tests.Types;
using Bashwork.General.Validation.Rules;

namespace Bashwork.General.Validation.Tests.Rules
{
	/// <summary>
	/// Code to test the validation methods that check if the supplied
	/// input meets the specified <see cref="string"/> constraints.
	/// </summary>
	[TestClass]
	public class StringValidationTests
	{
		[TestMethod]
		public void ValidateThat_RegularExpressionValidation_ThrowsWithNullContext()
		{
			PropertyContext<ExampleClassType, string> context = null;

			AssertEx.Throws<ArgumentNullException>(() => context.Contains("123456"));
			AssertEx.Throws<ArgumentNullException>(() => context.DoesNotContain("123456"));
			AssertEx.Throws<ArgumentNullException>(() => context.StartsWith("123456"));
			AssertEx.Throws<ArgumentNullException>(() => context.DoesNotStartWith("123456"));
			AssertEx.Throws<ArgumentNullException>(() => context.EndsWith("123456"));
			AssertEx.Throws<ArgumentNullException>(() => context.DoesNotEndWith("123456"));
		}

		[TestMethod]
		public void ValidateThat_SomeValue_IsNotNullOrEmpty()
		{
			var validator = Validate.That<ExampleClassType>()
				.Property(x => x.FirstName).IsNotNullOrEmpty()
				.And().Compile().Value;

			var handle = new ExampleClassType { FirstName = "Mr. Bjangles" };
			var result = validator.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			handle = new ExampleClassType { FirstName = null };
			result = validator.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_Contains()
		{
			var validator = Validate.That<ExampleClassType>()
				.Property(x => x.FirstName).Contains("Mr.").And()
				.Compile().Value;

			var handle = new ExampleClassType { FirstName = "Mr. Bjangles" };
			var result = validator.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			handle = new ExampleClassType { FirstName = "Mrs. Bjangles" };
			result = validator.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_DoesNotContain()
		{
			var validator = Validate.That<ExampleClassType>()
				.Property(x => x.FirstName).DoesNotContain("Mr.").And()
				.Compile().Value;

			var handle = new ExampleClassType { FirstName = "Mr. Bjangles" };
			var result = validator.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);

			handle = new ExampleClassType { FirstName = "Mrs. Bjangles" };
			result = validator.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_StartsWith()
		{
			var handle = new ExampleClassType
			{
				FirstName = "Mr. Bjangles"
			};

			var result = Validate.That<ExampleClassType>()
				.Property(x => x.FirstName).StartsWith("mr.").And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.FirstName).StartsWith("Mrs.").And()
				.Compile().Value.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.FirstName).StartsWith("mr.", false).And()
				.Compile().Value.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.FirstName).StartsWith("Mrs.", false).And()
				.Compile().Value.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_DoesNotStartWith()
		{
			var handle = new ExampleClassType
			{
				FirstName = "Mr. Bjangles"
			};

			var result = Validate.That<ExampleClassType>()
				.Property(x => x.FirstName).DoesNotStartWith("mr.").And()
				.Compile().Value.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.FirstName).DoesNotStartWith("Mrs.").And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.FirstName).DoesNotStartWith("mr.", false).And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.FirstName).DoesNotStartWith("Mrs.", false).And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_EndsWith()
		{
			var handle = new ExampleClassType
			{
				FirstName = "Mr. Bjangles"
			};

			var result = Validate.That<ExampleClassType>()
				.Property(x => x.FirstName).EndsWith("les").And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.FirstName).EndsWith("Mrs.").And()
				.Compile().Value.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.FirstName).EndsWith("LES", false).And()
				.Compile().Value.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.FirstName).EndsWith("Mrs.", false).And()
				.Compile().Value.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_DoesNotEndWith()
		{
			var handle = new ExampleClassType
			{
				FirstName = "Mr. Bjangles"
			};

			var result = Validate.That<ExampleClassType>()
				.Property(x => x.FirstName).DoesNotEndWith("les").And()
				.Compile().Value.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.FirstName).DoesNotEndWith("Mrs.").And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.FirstName).DoesNotEndWith("LES", false).And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			result = Validate.That<ExampleClassType>()
				.Property(x => x.FirstName).DoesNotEndWith("Mrs.", false).And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);
		}
	}
}
