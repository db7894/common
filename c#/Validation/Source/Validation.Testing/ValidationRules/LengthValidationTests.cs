using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bashwork.Validation.Tests.Internal;
using Bashwork.Validation.Tests.Types;
using Bashwork.Validation.ValidationRules;

namespace Bashwork.Validation.Tests.ValidationRules
{
	/// <summary>
	/// Code to test the validation methods that check if the supplied
	/// input collection has the specified length.
	/// </summary>
	[TestClass]
	public class LengthValidationTests
	{
		[TestMethod]
		public void ValidateThat_LengthValidation_ThrowsWithNullContext()
		{
			PropertyContext<ExampleClassType, string> context = null;

			// TODO AssertEx.Throws<ArgumentNullException>(() => context.IsValid(FormValidationType.Address));
		}
	}
}
