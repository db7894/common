using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharedAssemblies.Core.Containers.UnitTests
{
	/// <summary>
	/// Tests to exercise the OperationResult functionality
	/// </summary>
	[TestClass]
	public class OperationResultTest
	{
		[TestMethod]
		public void TestInitializedResult()
		{
			var value = "success";
			var result = new OperationResult<string>(true, value);

			Assert.IsTrue(result.IsSuccessful);
			Assert.AreEqual(value, result.Value);
		}

		[TestMethod]
		public void TestPositiveResult()
		{
			var value = "success";
			var result = OperationResult.Create(true, value);

			Assert.IsTrue(result.IsSuccessful);
			Assert.AreEqual(value, result.Value);
		}

		[TestMethod]
		public void TestNegativeResult()
		{
			var value = "failure";
			var result = OperationResult.Create(false, value);

			Assert.IsFalse(result.IsSuccessful);
			Assert.AreEqual(value, result.Value);
		}
	}
}
