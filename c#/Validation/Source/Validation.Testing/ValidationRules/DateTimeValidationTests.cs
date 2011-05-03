using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bashwork.Validation.Tests.Internal;
using Bashwork.Validation.Tests.Types;
using Bashwork.Validation.ValidationRules;

namespace Bashwork.Validation.Tests.ValidationRules
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
			var handle = new ExampleClassType { Birthday = DateTime.Now.AddDays(-1)  };

			var results = Validate.That(handle)
				.Property(x => x.Birthday).IsPast()
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);

			handle = new ExampleClassType { Birthday = DateTime.Now.AddDays(1) };

			results = Validate.That(handle)
				.Property(x => x.Birthday).IsPast()
				.And().Validate();
			Assert.IsFalse(results.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_IsFuture()
		{
			var handle = new ExampleClassType { Birthday = DateTime.Now.AddDays(-1) };

			var results = Validate.That(handle)
				.Property(x => x.Birthday).IsFuture()
				.And().Validate();
			Assert.IsFalse(results.IsSuccessful);

			handle = new ExampleClassType { Birthday = DateTime.Now.AddDays(1) };

			results = Validate.That(handle)
				.Property(x => x.Birthday).IsFuture()
				.And().Validate();
			Assert.IsTrue(results.IsSuccessful);
		}
	}
}
