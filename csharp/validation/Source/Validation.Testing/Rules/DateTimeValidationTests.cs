using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bashwork.General.Validation.Tests.Internal;
using Bashwork.General.Validation.Tests.Types;
using Bashwork.General.Validation.Rules;

namespace Bashwork.General.Validation.Tests.Rules
{
	/// <summary>
	/// Code to test the validation methods that check if the supplied
	/// input meets the specified <see cref="DateTime"/> constraints.
	/// </summary>
	[TestClass]
	public class DateTimeValidationTests
	{
		[TestMethod]
		public void ValidateThat_ContainsElement_ThrowsWithNullContext()
		{
			PropertyContext<ExampleClassType, DateTime> context = null;

			AssertEx.Throws<ArgumentNullException>(() => context.IsFuture());
			AssertEx.Throws<ArgumentNullException>(() => context.IsPast());
		}

		[TestMethod]
		public void ValidateThat_SomeValue_IsPast()
		{
			var validator = Validate.That<ExampleClassType>()
				.Property(x => x.Birthday).IsPast().And()
				.Compile().Value;

			var handle = new ExampleClassType { Birthday = DateTime.Now.AddDays(-1)  };
			var result = validator.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			handle = new ExampleClassType { Birthday = DateTime.Now.AddDays(1) };
			result = validator.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_IsFuture()
		{
			var validator = Validate.That<ExampleClassType>()
				.Property(x => x.Birthday).IsFuture().And()
				.Compile().Value;

			var handle = new ExampleClassType { Birthday = DateTime.Now.AddDays(-1) };
			var result = validator.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);

			handle = new ExampleClassType { Birthday = DateTime.Now.AddDays(1) };
			result = validator.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);
		}
	}
}
