using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharedAssemblies.General.Testing
{
	/// <summary>
	/// A collection of Asserts that are not included in MSTest
	/// but are included in other unit test frameworks
	/// </summary>
	[SuppressMessage("SharedAssemblies.Tools.StyleCopRules.SafetyAndPerformanceAnalyzer",
		"ST5007:NoCatchSystemException",
		Justification = "Only used by unit tests to verify an exception is thrown.")]
	[Obsolete("Use SharedAssemblies.General.Testing.AssertEx instead.", false)]
	public static class AssertExtensions
	{
		/// <summary>
		/// Assert an explicit test pass
		/// </summary>
		/// <remarks>This is useful as an explicit comment in the test</remarks>
		public static void Pass()
		{
			Assert.IsTrue(true);
		}


		/// <summary>
		/// Assert that some kind of exception should be thrown
		/// </summary>
		/// <param name="action">The method to invoke</param>
		public static void Throws(Action action)
		{
			Assert.IsTrue(ThrowHelper(action, null, null, true),
				"Exception should be thrown");
		}

		/// <summary>
		/// Assert that some kind of exception should be thrown
		/// </summary>
		/// <param name="action">The method to invoke</param>
		/// <param name="message">The expected resulting message</param>
		public static void Throws(Action action, string message)
		{
			Assert.IsTrue(ThrowHelper(action, null, message, true),
				"Exception should be thrown with message");
		}

		/// <summary>
		/// Assert that a specific exception should be thrown
		/// </summary>
		/// <typeparam name="T">The expected exception</typeparam>
		/// <param name="action">The method to invoke</param>
		public static void Throws<T>(Action action)
			where T : Exception
		{
			Assert.IsTrue(ThrowHelper(action, typeof(T), null, true),
				"Exception of type {0} should be thrown", typeof(T));
		}

		/// <summary>
		/// Assert that a specific exception should be thrown
		/// </summary>
		/// <typeparam name="T">The expected exception</typeparam>
		/// <param name="action">The method to invoke</param>
		/// <param name="message">The expected resulting message</param>
		public static void Throws<T>(Action action, string message)
			where T : Exception
		{
			Assert.IsTrue(ThrowHelper(action, typeof(T), message, true),
				"Exception of type {0} should be thrown with message", typeof(T));
		}

		/// <summary>
		/// Assert that no exception should thrown
		/// </summary>
		/// <param name="action">The method to invoke</param>
		public static void DoesNotThrow(Action action)
		{
			Assert.IsTrue(ThrowHelper(action, null, null, false),
				"Exception should not be thrown");
		}

		/// <summary>
		/// Assert that no exception should thrown
		/// </summary>
		/// <param name="action">The method to invoke</param>
		/// <param name="message">The expected resulting message</param>
		public static void DoesNotThrow(Action action, string message)
		{
			Assert.IsTrue(ThrowHelper(action, null, message, false),
				"Exception should not be thrown with message {0}", message);
		}

		/// <summary>
		/// Assert that a specific exception should not be thrown
		/// </summary>
		/// <typeparam name="T">The expected exception</typeparam>
		/// <param name="action">The method to invoke</param>
		public static void DoesNotThrow<T>(Action action)
			where T : Exception
		{
			Assert.IsTrue(ThrowHelper(action, typeof(T), null, false),
				"Exception of type {0} should not be thrown", typeof(T));
		}

		/// <summary>
		/// Assert that a specific exception should not be thrown
		/// </summary>
		/// <typeparam name="T">The expected exception</typeparam>
		/// <param name="action">The method to invoke</param>
		/// <param name="message">The expected resulting message</param>
		public static void DoesNotThrow<T>(Action action, string message)
			where T : Exception
		{
			Assert.IsTrue(ThrowHelper(action, typeof(T), message, false),
				"Exception of type {0} should not be thrown with message {1}", typeof(T), message);
		}


		/// <summary>
		/// Assert that a result is in the requested range
		/// </summary>
		/// <param name="result">The resulting value</param>
		/// <param name="expected">The expected value to test around</param>
		/// <param name="fudge">The fudge factor to provide a range</param>
		public static void InRange(double result, double expected, double fudge)
		{
			Assert.IsTrue(Math.Abs(result - expected) < fudge,
				string.Format("{0} was not in the range of {1} +/- {2}", result, expected, fudge));
		}

		/// <summary>
		/// Assert that a result is not in the requested range
		/// </summary>
		/// <param name="result">The resulting value</param>
		/// <param name="expected">The expected value to test around</param>
		/// <param name="fudge">The fudge factor to provide a range</param>
		public static void NotInRange(double result, double expected, double fudge)
		{
			Assert.IsFalse(Math.Abs(result - expected) < fudge,
				string.Format("{0} was in the range of {1} +/- {2}", result, expected, fudge));
		}

		/// <summary>
		/// Assert correct usage of double.IsNaN
		/// </summary>
		/// <remarks>
		/// double x = double.NaN;
		/// (double.NaN == x) == false!
		/// </remarks>
		/// <param name="result">The double value to test</param>
		public static void IsNaN(double result)
		{
			Assert.IsTrue(double.IsNaN(result),
				string.Format("{0} was not equal to double.NaN", result));
		}

		/// <summary>
		/// Assert that resulting value is greater than expected value
		/// </summary>
		/// <typeparam name="T">The type of item to compare.</typeparam>
		/// <param name="low">The low value to compare against</param>
		/// <param name="result">The resulting value to compare</param>
		/// <param name="high">The high value to compare against</param>
		public static void InBetween<T>(T low, T result, T high)
			where T : IComparable
		{
			Assert.IsTrue((result.CompareTo(low) > 0) & (result.CompareTo(high) < 0),
				string.Format("{0} was not between {1} and {2}", result, low, high));
		}
        
        /// <summary>
		/// Assert that resulting value is greater than expected value
		/// </summary>
        /// <typeparam name="T">The type of item to compare.</typeparam>
        /// <param name="result">The resulting value to compare</param>
		/// <param name="expected">The value to compare against</param>
		public static void GreaterThan<T>(T result, T expected)
			where T : IComparable
		{
			Assert.IsTrue(result.CompareTo(expected) > 0, 
				string.Format("{0} was not greater than {1}", result, expected));
		}

		/// <summary>
		/// Assert that resulting value is less expected value
		/// </summary>
		/// <typeparam name="T">The type of item to compare.</typeparam>
		/// <param name="result">The resulting value to compare</param>
		/// <param name="expected">The value to compare against</param>
		public static void LessThan<T>(T result, T expected)
			where T : IComparable
		{
			Assert.IsTrue(result.CompareTo(expected) < 0,
				string.Format("{0} was not less than {1}", result, expected));
		}


        /// <summary>
        /// Helper function to DRY out some of the throw asserts
        /// </summary>
        /// <param name="action">That action to test for exceptions</param>
        /// <param name="exception">The type of exception to check for or null</param>
        /// <param name="message">The exception message to check for or null</param>
        /// <param name="shouldThrow">True if the method should throw, false otherwise</param>
        /// <returns>True if the method threw an exception and met all the constraints</returns>
        private static bool ThrowHelper(Action action, Type exception, string message, bool shouldThrow)
        {
            bool throws = false;		// we haven't thrown yet
            bool passes = !shouldThrow;	// if we don't throw, we pass by default

            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                throws = true;

                // If either of these fields are not set, they are not compared
                // and are passed.  If they are set, we need to compare as
                // explicitly as the user defined (i.e. if we don't care about
                // the type, but we care about the message).
                passes  = (exception == null) ? true : exception == ex.GetType();
                passes &= (message == null)   ? true : message == ex.Message;
            }

            // If we should have thrown, then throwing is a positive result
            // if we should not have thrown, then throwing is a negative result
            return (shouldThrow) ? throws & passes : !throws | !passes;
        }
    }
}
