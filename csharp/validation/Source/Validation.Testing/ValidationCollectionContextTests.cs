using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bashwork.General.Validation.Rules;

namespace Bashwork.General.Validation.Tests
{
	/// <summary>
	/// Code to test the various extension methods attatched
	/// to the validation context.
	/// </summary>
	/// TODO
	[TestClass]
	public class ValidationCollectionContextTests
	{
		//[TestMethod]
		//public void ValidationCollectionContext_WorksWith_ElementValues()
		//{
		//    var handle = new List<string> { "a", "b", "c", "  " };
		//    var result = Validate.ThatAll<string>()
		//        .Property(a => a.Length).IsEqualTo(1).And()
		//        .Compile().Value.Validate(handle);

		//    Assert.IsFalse(result.IsSuccessful);
		//    Assert.AreEqual(1, result.Failures.Count());
		//}

		//[TestMethod]
		//public void ValidationCollectionContext_WorksWith_Elements()
		//{
		//    var handle = new List<string> { "a", "b", "c", "  " };
		//    var result = Validate.ThatAll<string>()
		//        .Property(a => a).DoesNotContain(" ").And()
		//        .Compile().Value.Validate(handle);

		//    Assert.IsFalse(result.IsSuccessful);
		//    Assert.AreEqual(1, result.Failures.Count());
		//}

		//[TestMethod]
		//public void ValidationCollectionContext_WorksWith_SingleElement()
		//{
		//    var handle = new List<string> { "a" };
		//    var result = Validate.ThatAll<string>()
		//        .Property(a => a).DoesNotContain(" ").And()
		//        .Compile().Value.Validate(handle);

		//    Assert.IsTrue(result.IsSuccessful);
		//}
	}
}
