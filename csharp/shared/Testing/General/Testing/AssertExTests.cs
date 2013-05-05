using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.General.Testing.UnitTests.Types;

namespace SharedAssemblies.General.Testing.UnitTests
{
	/// <summary>
	/// AssetExtension fixture
	/// </summary>
	[TestClass]
	public class AssertExTests
	{
		/// <summary>
		/// Tests that all the throw extension variants are correct
		/// </summary>
		[TestMethod]
		public void AssertEx_TestThrowingMethods()
		{
			const string message = "Some Obscure Message";

			AssertEx.Throws(() => MethodThatThrows(message));
			AssertEx.Throws(() => MethodThatThrows(message), message);
			AssertEx.Throws<InvalidOperationException>(() => MethodThatThrows(message));
			AssertEx.Throws<InvalidOperationException>(() => MethodThatThrows(message), message);
		}

		/// <summary>
		/// Tests that all the does not throw extension variants are correct
		/// </summary>
		[TestMethod]
		public void AssertEx_TestNonThrowingMethods()
		{
			const string message1 = "Some Obscure Message";
			const string message2 = "Another Obscure Message";

			AssertEx.DoesNotThrow(() => MethodThatDoesNotThrow(message1));
			AssertEx.DoesNotThrow(() => MethodThatDoesNotThrow(message1), message1);
			AssertEx.DoesNotThrow(() => MethodThatThrows(message1), message2);
			AssertEx.DoesNotThrow<NullReferenceException>(() => MethodThatDoesNotThrow(message1));
			AssertEx.DoesNotThrow<NullReferenceException>(() => MethodThatDoesNotThrow(message1), 
				message1);
			AssertEx.DoesNotThrow<ArgumentException>(() => MethodThatThrows(message1));
			AssertEx.DoesNotThrow<NullReferenceException>(() => MethodThatThrows(message1), message2);
			AssertEx.DoesNotThrow<ArgumentException>(() => MethodThatThrows(message1), message1);
		}

		/// <summary>
		/// Tests that all the common extensions work correctly
		/// </summary>
		[TestMethod]
		public void AssertEx_TestCommonMethods()
		{
			AssertEx.Pass();
		}

		/// <summary>
		/// Tests that all the double extensions work correctly
		/// </summary>
		[TestMethod]
		public void AssertEx_TestDoubleMethods()
		{
			AssertEx.IsNaN(double.NaN);
			AssertEx.Throws(() => AssertEx.IsNaN(1.21));
			AssertEx.InRange(1.21, 1.2, 0.05);
			AssertEx.Throws(() => AssertEx.InRange(1.25, 1.2, 0.01));
			AssertEx.Throws(() => AssertEx.NotInRange(1.21, 1.2, 0.05));
		}

		/// <summary>
		/// Tests that all the greater than extensions work correctly
		/// </summary>
		[TestMethod]
		public void AssertEx_TestGreaterThanMethods()
		{
			AssertEx.GreaterThan('Z', 'A');
			AssertEx.GreaterThan("Marcus", "Maahus");
			AssertEx.GreaterThan(100, 10);
			AssertEx.GreaterThan(100.25, 10.25);

			AssertEx.LessThan('A', 'Z');
			AssertEx.LessThan("Marcus", "Mathus");
			AssertEx.LessThan(10, 100);
			AssertEx.LessThan(10.25, 100.25);

			AssertEx.InBetween(0, 10, 100);
			AssertEx.Throws(() => AssertEx.InBetween(0, 0, 1));
			AssertEx.Throws(() => AssertEx.InBetween(-1, 0, 0));
		}

		/// <summary>
		/// Tests that the generic assert are equal will work and fail correctly
		/// </summary>
		[TestMethod]
		public void AssertEx_AreEqual_Tests()
		{
			var expected = new SomeType { Interest = 1.25, Name = "Johnny", Number = 21, };
			var result = new SomeType { Interest = 1.25, Name = "Johnny", Number = 21, };
			var failure = new SomeType { Interest = 3.26, Name = "Samuel", Number = 25, };

			AssertEx.AreEqual(expected, result);
			AssertEx.Throws(() => AssertEx.AreEqual(expected, failure));
		}

		/// <summary>
		/// Tests that the generic assert are not equal will work and fail correctly
		/// </summary>
		[TestMethod]
		public void AssertEx_AreNotEqual_Tests()
		{
			var expected = new SomeType { Interest = 1.25, Name = "Johnny", Number = 21, };
			var result = new SomeType { Interest = 1.25, Name = "Johnny", Number = 21, };
			var failure = new SomeType { Interest = 3.26, Name = "Samuel", Number = 25, };

			AssertEx.AreNotEqual(expected, failure);
			AssertEx.Throws(() => AssertEx.AreNotEqual(expected, result));
		}

		/// <summary>
		/// Tests that the generic assert are behaviors equal will work and fail correctly
		/// </summary>
		[TestMethod]
		public void AssertEx_AreBehaviorsEqual_Tests()
		{
			var expected = new SomeType { Interest = 1.25, Name = "Johnny", Number = 21, };
			var result = new SomeType { Interest = 1.25, Name = "Johnny", Number = 21, };
			var failure = new SomeType { Interest = 3.26, Name = "Samuel", Number = 25, };

			AssertEx.AreBehaviorsEqual(() => expected, () => result);
			AssertEx.AreBehaviorsEqual<bool>(MethodThatThrows, MethodThatThrows);
			AssertEx.AreBehaviorsEqual<bool>(MethodThatDoesNotThrow, MethodThatDoesNotThrow);
			AssertEx.Throws(() => AssertEx.AreBehaviorsEqual(() => expected, () => failure));
			AssertEx.Throws(() => AssertEx.AreBehaviorsEqual<bool>(MethodThatThrows, MethodThatDoesNotThrow));
		}

		/// <summary>
		/// Tests that the generic assert are behaviors not equal will work and fail correctly
		/// </summary>
		[TestMethod]
		public void AssertEx_AreBehaviorsNotEqual_Tests()
		{
			var expected = new SomeType { Interest = 1.25, Name = "Johnny", Number = 21, };
			var result = new SomeType { Interest = 1.25, Name = "Johnny", Number = 21, };
			var failure = new SomeType { Interest = 3.26, Name = "Samuel", Number = 25, };

			AssertEx.AreBehaviorsNotEqual(() => expected, () => failure);
			AssertEx.AreBehaviorsNotEqual<bool>(MethodThatThrows, MethodThatDoesNotThrow);
			AssertEx.Throws(() => AssertEx.AreBehaviorsNotEqual(() => expected, () => result));
			AssertEx.Throws(() => AssertEx.AreBehaviorsNotEqual<bool>(MethodThatThrows, MethodThatThrows));
		}

		#region Private Test Helper Methods

		/// <summary>
		/// Just a method that always throws
		/// </summary>
		/// <returns>Doesn't matter, never returns</returns>
		private static bool MethodThatThrows()
		{
			throw new Exception("Something");
		}

		/// <summary>
		/// Helper method to throw an exception
		/// </summary>
		/// <param name="message">An optional message to throw</param>
		private static void MethodThatThrows(string message)
		{
			throw new InvalidOperationException(message);
		}

		/// <summary>
		/// Just a method that doesn't throw
		/// </summary>
		/// <returns>Always returns false</returns>
		private static bool MethodThatDoesNotThrow()
		{
			return false;
		}

		/// <summary>
		/// Helper method that does not throw an exception
		/// </summary>
		/// <param name="message">An optional message to throw</param>
		private static void MethodThatDoesNotThrow(string message)
		{
			// pass
		}

		#endregion
	}
}
