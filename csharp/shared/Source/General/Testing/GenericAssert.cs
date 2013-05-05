using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.Core.Extensions;

namespace SharedAssemblies.General.Testing
{
	/// <summary>
	/// A collection of asserts to be used on generic collections
	/// </summary>
	[SuppressMessage("SharedAssemblies.Tools.StyleCopRules.SafetyAndPerformanceAnalyzer",
		"ST5005:NoEmptyCatchBlocks",
		Justification = "Only catches to check for differences in behaviors for unit testing.")]
	[SuppressMessage("SharedAssemblies.Tools.StyleCopRules.SafetyAndPerformanceAnalyzer",
		"ST5007:NoCatchSystemException",
		Justification = "Only used by unit tests to verify an exception is thrown.")]
	[Obsolete("Use SharedAssemblies.General.Testing.AssertEx instead.", false)]
	public static class GenericAssert
	{
	    /// <summary>
	    /// Check if two arbitrary types are equal
	    /// </summary>
	    /// <typeparam name="TValue">The type to compare with</typeparam>
	    /// <param name="expected">The expected result value</param>
	    /// <param name="actual">The actual result value</param>
	    public static void AreEqual<TValue>(TValue expected, TValue actual)
	    {
			string encodede = expected.ToDataContractJson<TValue>();
			string encodeda = actual.ToDataContractJson<TValue>();
	        Assert.AreEqual(encodede, encodeda);
	    }

	    /// <summary>
	    /// Check if two arbitrary types are not equal
	    /// </summary>
	    /// <typeparam name="TValue">The type to compare with</typeparam>
		/// <param name="expected">The expected result value</param>
		/// <param name="actual">The actual result value</param>
	    public static void AreNotEqual<TValue>(TValue expected, TValue actual)
	    {
			string encodede = expected.ToDataContractJson<TValue>();
			string encodeda = actual.ToDataContractJson<TValue>();
	        Assert.AreNotEqual(encodede, encodeda);
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

			if (exception1 != null && exception2 != null)
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
	}
}
