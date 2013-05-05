using System;
using SharedAssemblies.Core.Patterns;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharedAssemblies.Core.UnitTests
{
	/// <summary>
	/// A collection of code to test the functionality of the Functional module
	/// </summary>
	[TestClass]
	public class FunctionalTest
	{
		/// <summary>
		/// This tests that we can successfully curry methods with different
		/// number of parameters.
		/// </summary>
		[TestMethod]
		public void Functional_TestCurring_IsSuccessful()
		{
			const string expected = "abcdefg";
			Func<string, string, string> method1 = (a, b) => a + b;
			Func<string, string, string, string> method2 = (a, b, c) => a + b + c;
			Func<string, string, string, string, string> method3 = (a, b, c, d) => a + b + c + d;
            
			Assert.AreEqual(expected, Functional.Curry(method1)("abcd")("efg"));
			Assert.AreEqual(expected, Functional.Curry(method2)("ab")("cd")("efg"));
			Assert.AreEqual(expected, Functional.Curry(method3)("ab")("cd")("ef")("g"));
		}

		/// <summary>
		/// This tests that we can successfully reverse curry methods with
		/// different number of parameters.
		/// </summary>
		[TestMethod]
		public void Functional_TestReverseCurring_IsSuccessful()
		{
			const string expected = "abcdefg";
			Func<string, string, string> method1 = (a, b) => a + b;
			Func<string, string, string, string> method2 = (a, b, c) => a + b + c;
			Func<string, string, string, string, string> method3 = (a, b, c, d) => a + b + c + d;

			Assert.AreEqual(expected, Functional.ReverseCurry(method1)("efg")("abcd"));
			Assert.AreEqual(expected, Functional.ReverseCurry(method2)("efg")("cd")("ab"));
			Assert.AreEqual(expected, Functional.ReverseCurry(method3)("g")("ef")("cd")("ab"));
		}

		/// <summary>
		/// This tests that we can successfully thunk methods with different
		/// number of parameters.
		/// </summary>
		[TestMethod]
		public void Functional_TestThunking_IsSuccessful()
		{
			const string expected = "abcdefg";
			Func<string, string, string> method1 = (a, b) => a + b;
			Func<string, string, string, string> method2 = (a, b, c) => a + b + c;
			Func<string, string, string, string, string> method3 = (a, b, c, d) => a + b + c + d;

			Assert.AreEqual(expected, Functional.Thunk("abcdefg")());
			Assert.AreEqual(expected, Functional.Thunk(method1, "abcd", "efg")());
			Assert.AreEqual(expected, Functional.Thunk(method2, "ab", "cd", "efg")());
			Assert.AreEqual(expected, Functional.Thunk(method3, "ab", "cd", "ef", "g")());
		}

		/// <summary>
		/// This tests that we can successfully un-curry methods with different
		/// number of parameters.
		/// </summary>
		[TestMethod]
		public void Functional_TestUncurring_IsSuccessful()
		{
			const string expected = "abcdefg";
			Func<string, string, string> method1 = (a, b) => a + b;
			Func<string, string, string, string> method2 = (a, b, c) => a + b + c;
			Func<string, string, string, string, string> method3 = (a, b, c, d) => a + b + c + d;

			var rmethod1 = Functional.Curry(method1);
			var rmethod2 = Functional.Curry(method2);
			var rmethod3 = Functional.Curry(method3);

			Assert.AreEqual(expected, Functional.Uncurry(rmethod1)("abcd", "efg"));
			Assert.AreEqual(expected, Functional.Uncurry(rmethod2)("ab", "cd", "efg"));
			Assert.AreEqual(expected, Functional.Uncurry(rmethod3)("ab", "cd", "ef", "g"));
		}

		/// <summary>
		/// This tests that we can successfully reverse un-curry methods with
		/// different number of parameters.
		/// </summary>
		[TestMethod]
		public void Functional_TestReverseUncurring_IsSuccessful()
		{
			const string expected = "abcdefg";
			Func<string, string, string> method1 = (a, b) => a + b;
			Func<string, string, string, string> method2 = (a, b, c) => a + b + c;
			Func<string, string, string, string, string> method3 = (a, b, c, d) => a + b + c + d;

			var rmethod1 = Functional.ReverseCurry(method1);
			var rmethod2 = Functional.ReverseCurry(method2);
			var rmethod3 = Functional.ReverseCurry(method3);

			Assert.AreEqual(expected, Functional.ReverseUncurry(rmethod1)("abcd", "efg"));
			Assert.AreEqual(expected, Functional.ReverseUncurry(rmethod2)("ab", "cd", "efg"));
			Assert.AreEqual(expected, Functional.ReverseUncurry(rmethod3)("ab", "cd", "ef", "g"));
		}

		/// <summary>
		/// This tests that we can successfully curry methods with different
		/// number of parameters.
		/// </summary>
		[TestMethod]
		public void Functional_TestYCombinator_IsSuccessful()
		{
			Func<Func<int, int>, Func<int, int>> factorial =
				method => // recursion injection
				arg => arg == 0 ? 1 : arg * method(arg - 1);
			var recursion = Functional.Y(factorial);

			Assert.AreEqual(120, recursion(5));
		}

		/// <summary>
		/// This tests that we can successfully curry methods with different
		/// number of parameters.
		/// </summary>
		[TestMethod]
		public void Functional_TestICombinator_IsSuccessful()
		{
			const string expected = "hello";

			Assert.AreEqual(expected, Functional.I<string>()("hello"));
			Assert.AreEqual(expected, Functional.I<string>("hello"));
		}

		/// <summary>
		/// This tests that we can successfully curry methods with different
		/// number of parameters.
		/// </summary>
		[TestMethod]
		public void Functional_TestKCombinator_IsSuccessful()
		{
			const string expected = "hello";

			Assert.AreEqual(expected, Functional.K<string>("hello")());
			Assert.AreEqual(expected, Functional.K<int, string>("hello")(22));
			Assert.AreEqual(expected, Functional.K<int, string>()("hello")(22));
		}
	}
}
