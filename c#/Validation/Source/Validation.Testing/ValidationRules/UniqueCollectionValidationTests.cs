using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.General.Validation.Tests.Internal;
using SharedAssemblies.General.Validation.Tests.Types;
using SharedAssemblies.General.Validation.ValidationRules;

namespace SharedAssemblies.General.Validation.Tests.ValidationRules
{
	/// <summary>
	/// Code to test the validation methods that check if the supplied
	/// input collection is a unique collection.
	/// </summary>
	[TestClass]
	public class UniqueCollectionValidationTests
	{
		[TestMethod]
		public void ValidateThat_LengthValidation_ThrowsWithNullContext()
		{
			PropertyContext<ExampleClassType, string> context = null;

			// TODO AssertEx.Throws<ArgumentNullException>(() => context.IsValid(FormValidationType.Address));
		}
	}
}
