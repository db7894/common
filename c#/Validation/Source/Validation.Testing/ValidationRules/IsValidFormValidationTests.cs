using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bashwork.Validation.Tests.Internal;
using Bashwork.Validation.Tests.Types;
using Bashwork.Validation.ValidationRules;

namespace Bashwork.Validation.Tests.ValidationRules
{
	/// <summary>
	/// A collection of validation methods that check if the supplied
	/// input string is one of the available valid types (backed by regular expressions)
	/// </summary>
	[TestClass]
	public class IsValidFormValidationTests
	{
		[TestMethod]
		public void ValidateThat_IsValidFormValidation_ThrowsWithNullContext()
		{
			PropertyContext<ExampleClassType, string> context = null;

			AssertEx.Throws<ArgumentNullException>(() => context.IsValid(FormValidationType.Address));
		}
	}
}
