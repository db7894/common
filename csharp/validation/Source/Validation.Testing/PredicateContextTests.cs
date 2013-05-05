using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bashwork.General.Validation.Rules;
using Bashwork.General.Validation.Tests.Types;

namespace Bashwork.General.Validation.Tests
{
	/// <summary>
	/// Code to test the code paths for the predicate context
	/// </summary>
	[TestClass]
	public class PredicateContextTests
	{
		[TestMethod]
		public void PredicateContext_PropertyName_Overloads()
		{
			var name = "UserAge";
			var validator = Validate.That<ExampleClassType>()
				.Property(x => x.Age, name).IsEqualTo(10).And()
				.Compile().Value;

			var handle = new ExampleClassType { Age = 20, };
			var result = validator.Validate(handle);
			
			Assert.IsFalse(result.IsSuccessful);
			Assert.AreEqual(name, result.Failures.First().PropertyName);			
		}

		[TestMethod]
		public void PredicateContext_ErrorMessage_Overloads()
		{
			var message = "The user's age is not a valid value";
			var validator = Validate.That<ExampleClassType>()
				.Property(x => x.Age).IsEqualTo(10).And(message)
				.Compile().Value;

			var handle = new ExampleClassType { Age = 20, };
			var result = validator.Validate(handle);

			Assert.IsFalse(result.IsSuccessful);
			Assert.AreEqual(message, result.Failures.First().ToString());
		}
	}
}
