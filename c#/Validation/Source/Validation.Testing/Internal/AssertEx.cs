using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bashwork.Validation.Tests.Internal
{
	/// <summary>
	/// A collection of Asserts that are not included in MSTest
	/// but are included in other unit test frameworks
	/// </summary>
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
				passes = (exception == null) ? true : exception == ex.GetType();
				passes &= (message == null) ? true : message == ex.Message;
			}

			// If we should have thrown, then throwing is a positive result
			// if we should not have thrown, then throwing is a negative result
			return (shouldThrow) ? throws & passes : !throws | !passes;
		}
	}
}
