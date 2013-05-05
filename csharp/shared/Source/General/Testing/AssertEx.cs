using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.Core.Extensions;

namespace SharedAssemblies.General.Testing
{
	/// <summary>
	/// A collection of Asserts that are not included in MSTest
	/// but are included in other unit test frameworks
	/// </summary>
	[SuppressMessage("SharedAssemblies.Tools.StyleCopRules.SafetyAndPerformanceAnalyzer",
		"ST5005:NoEmptyCatchBlocks",
		Justification = "Only catches to check for differences in behaviors for unit testing.")]
	[SuppressMessage("SharedAssemblies.Tools.StyleCopRules.SafetyAndPerformanceAnalyzer",
		"ST5007:NoCatchSystemException",
		Justification = "Only used by unit tests to verify an exception is thrown.")]
	public static class AssertEx
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
			var failText = "Exception should have been thrown";
			Assert.IsTrue(ThrowHelper(action, null, null, true), failText);
		}

		/// <summary>
		/// Assert that some kind of exception should be thrown
		/// </summary>
		/// <param name="action">The method to invoke</param>
		/// <param name="message">The expected resulting message</param>
		/// <param name="failText">The warning text displayed on failure</param>
		public static void Throws(Action action, string message, string failText=null)
		{
			failText = failText
					?? string.Format("Exception should have been thrown with message {0}", message);
			Assert.IsTrue(ThrowHelper(action, null, message, true), failText);
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
				"Exception of type {0} should have been thrown", typeof(T));
		}

		/// <summary>
		/// Assert that a specific exception should be thrown
		/// </summary>
		/// <typeparam name="T">The expected exception</typeparam>
		/// <param name="action">The method to invoke</param>
		/// <param name="message">The expected resulting message</param>
		/// <param name="failText">The warning text displayed on failure</param>
		public static void Throws<T>(Action action, string message, string failText = null)
			where T : Exception
		{
			failText = failText
					?? string.Format("Exception of type {0} should have been thrown with message", typeof(T));
			Assert.IsTrue(ThrowHelper(action, typeof(T), message, true), failText);
		}

		/// <summary>
		/// Assert that no exception should thrown
		/// </summary>
		/// <param name="action">The method to invoke</param>
		public static void DoesNotThrow(Action action)
		{
			var failText = "Exception should not have been thrown";
			Assert.IsTrue(ThrowHelper(action, null, null, false), failText);
		}

		/// <summary>
		/// Assert that no exception should thrown
		/// </summary>
		/// <param name="action">The method to invoke</param>
		/// <param name="message">The expected resulting message</param>
		/// <param name="failText">The warning text displayed on failure</param>
		public static void DoesNotThrow(Action action, string message, string failText = null)
		{
			failText = failText
					?? string.Format("Exception should not have been thrown with message {0}", message);
			Assert.IsTrue(ThrowHelper(action, null, message, false), failText);
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
				"Exception of type {0} should not have been thrown", typeof(T));
		}

		/// <summary>
		/// Assert that a specific exception should not be thrown
		/// </summary>
		/// <typeparam name="T">The expected exception</typeparam>
		/// <param name="action">The method to invoke</param>
		/// <param name="message">The expected resulting message</param>
		/// <param name="failText">The warning text displayed on failure</param>
		public static void DoesNotThrow<T>(Action action, string message, string failText = null)
			where T : Exception
		{
			failText = failText
					?? string.Format("Exception of type {0} should not have been thrown with message {1}", typeof(T), message);
			Assert.IsTrue(ThrowHelper(action, typeof(T), message, false), failText);
		}

		/// <summary>
		/// Assert that a result is in the requested range
		/// </summary>
		/// <param name="result">The resulting value</param>
		/// <param name="expected">The expected value to test around</param>
		/// <param name="fudge">The fudge factor to provide a range</param>
		/// <param name="failText">The warning text displayed on failure</param>
		public static void InRange(double result, double expected, double fudge, string failText = null)
		{
			failText = failText
					?? string.Format("{0} was not in the range of {1} +/- {2}", result, expected, fudge);
			Assert.IsTrue(Math.Abs(result - expected) < fudge, failText);
		}

		/// <summary>
		/// Assert that a result is not in the requested range
		/// </summary>
		/// <param name="result">The resulting value</param>
		/// <param name="expected">The expected value to test around</param>
		/// <param name="fudge">The fudge factor to provide a range</param>
		/// <param name="failText">The warning text displayed on failure</param>
		public static void NotInRange(double result, double expected, double fudge, string failText = null)
		{
			failText = failText
					?? string.Format("{0} was in the range of {1} +/- {2}", result, expected, fudge);
			Assert.IsFalse(Math.Abs(result - expected) < fudge, failText);
		}

		/// <summary>
		/// Assert correct usage of double.IsNaN
		/// </summary>
		/// <remarks>
		/// double x = double.NaN;
		/// (double.NaN == x) == false!
		/// </remarks>
		/// <param name="result">The double value to test</param>
		/// <param name="failText">The warning text displayed on failure</param>
		public static void IsNaN(double result, string failText = null)
		{
			failText = failText
					?? string.Format("{0} was not equal to double.NaN", result);
			Assert.IsTrue(double.IsNaN(result), failText);
		}

		/// <summary>
		/// Assert that resulting value is greater than expected value
		/// </summary>
		/// <typeparam name="T">The type of item to compare.</typeparam>
		/// <param name="low">The low value to compare against</param>
		/// <param name="result">The resulting value to compare</param>
		/// <param name="high">The high value to compare against</param>
		/// <param name="failText">The warning text displayed on failure</param>
		public static void InBetween<T>(T low, T result, T high, string failText = null)
			where T : IComparable
		{
			failText = failText
					?? string.Format("{0} was not between {1} and {2}", result, low, high);
			Assert.IsTrue((result.CompareTo(low) > 0) & (result.CompareTo(high) < 0), failText);
		}
        
        /// <summary>
		/// Assert that resulting value is greater than expected value
		/// </summary>
        /// <typeparam name="T">The type of item to compare.</typeparam>
        /// <param name="result">The resulting value to compare</param>
		/// <param name="expected">The value to compare against</param>
		/// <param name="failText">The warning text displayed on failure</param>
		public static void GreaterThan<T>(T result, T expected, string failText = null)
			where T : IComparable
		{
			failText = failText
					?? string.Format("{0} was not greater than {1}", result, expected);
			Assert.IsTrue(result.CompareTo(expected) > 0, failText);
		}

		/// <summary>
		/// Assert that resulting value is less expected value
		/// </summary>
		/// <typeparam name="T">The type of item to compare.</typeparam>
		/// <param name="result">The resulting value to compare</param>
		/// <param name="expected">The value to compare against</param>
		/// <param name="failText">The warning text displayed on failure</param>
		public static void LessThan<T>(T result, T expected, string failText = null)
			where T : IComparable
		{
			failText = failText
					?? string.Format("{0} was not less than {1}", result, expected);
			Assert.IsTrue(result.CompareTo(expected) < 0, failText);
		}

		/// <summary>
		/// Check if two arbitrary types are equal
		/// </summary>
		/// <typeparam name="TValue">The type to compare with</typeparam>
		/// <param name="expected">The expected result value</param>
		/// <param name="actual">The actual result value</param>
		/// <param name="failText">The warning text displayed on failure</param>
		public static void AreEqual<TValue>(TValue expected, TValue actual, string failText = null)
		{
			string encodede = expected.ToDataContractJson<TValue>();
			string encodeda = actual.ToDataContractJson<TValue>();

			if (failText != null)
			{
				Assert.AreEqual(encodede, encodeda, failText);
			}
			else
			{
				Assert.AreEqual(encodede, encodeda);
			}
		}

		/// <summary>
		/// Check if two arbitrary types are not equal
		/// </summary>
		/// <typeparam name="TValue">The type to compare with</typeparam>
		/// <param name="expected">The expected result value</param>
		/// <param name="actual">The actual result value</param>
		/// <param name="failText">The warning text displayed on failure</param>
		public static void AreNotEqual<TValue>(TValue expected, TValue actual, string failText = null)
		{
			string encodede = expected.ToDataContractJson<TValue>();
			string encodeda = actual.ToDataContractJson<TValue>();

			if (failText != null)
			{
				Assert.AreNotEqual(encodede, encodeda, failText);
			}
			else
			{
				Assert.AreNotEqual(encodede, encodeda);
			}
		}

		/// <summary>
		/// Assert that the two methods have the same result
		/// </summary>
		/// <typeparam name="T">The type of item to compare.</typeparam>
		/// <param name="call1">The first method to compare against</param>
		/// <param name="call2">The second method to compare against</param>
		public static void AreBehaviorsEqual<T>(Func<T> call1, Func<T> call2)
		{
			T result1 = default(T);
			T result2 = default(T);
			Exception exception1 = null;
			Exception exception2 = null;

			try
			{
				result1 = call1();
			}
			catch (Exception ex)
			{
				exception1 = ex;
			}

			try
			{
				result2 = call2();
			}
			catch (Exception ex)
			{
				exception2 = ex;
			}

			AreEqual(result1, result2);

			Assert.AreEqual(
				((exception1 == null) ? true : false),
				((exception2 == null) ? true : false));

			if ((exception1 != null) && (exception2 != null))
			{
				Assert.AreEqual(exception1.Message, exception2.Message);
			}
		}

		/// <summary>
		/// Assert that the two methods do not have the same result
		/// </summary>
		/// <param name="call1">The first method to compare against</param>
		/// <param name="call2">The second method to compare against</param>
		/// <typeparam name="T">The return type of the functions to compare</typeparam>
		public static void AreBehaviorsNotEqual<T>(Func<T> call1, Func<T> call2)
		{
			bool pass = true;

			try
			{
				AreBehaviorsEqual(call1, call2);
				pass = false;
			}
			catch (Exception)
			{
			}
			Assert.IsTrue(pass, "Behaviours are the same between methods");
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
