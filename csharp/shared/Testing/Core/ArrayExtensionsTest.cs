using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.Core.Containers;
using SharedAssemblies.Core.Extensions;


namespace SharedAssemblies.Core.UnitTests
{
	/// <summary>
	/// The set of unit tests for the ArrayExtensions class
	/// </summary>
	[TestClass]
	public class ArrayExtensionsTest
	{
		/// <summary>
		/// Gets or sets the test context which provides
		/// information about and functionality for the current test run.
		/// </summary>
		public TestContext TestContext { get; set; }


		/// <summary>
		/// Tests the string NullSafeLength extension method.
		/// </summary>
		[TestMethod]
		public void NullSafeLength_ReturnsZero_ForNull()
		{
			int[] target = null;

			Assert.AreEqual(0, target.NullSafeLength());
		}


		/// <summary>
		/// Tests the string NullSafeLength extension method.
		/// </summary>
		[TestMethod]
		public void NullSafeLength_ReturnsZero_ForEmpty()
		{
			int[] target = new int[0];

			Assert.AreEqual(0, target.NullSafeLength());
		}

		/// <summary>
		/// Tests the string NullSafeLength extension method.
		/// </summary>
		[TestMethod]
		public void NullSafeLength_ReturnsLength_ForNonEmpty()
		{
			int[] target = new int[] { 1, 2, 3, 4, 5 };

			Assert.AreEqual(5, target.NullSafeLength());
		}

		/// <summary>
		/// Tests the string NullSafeCount extension method.
		/// </summary>
		[TestMethod]
		public void NullSafeCount_ReturnsZero_ForNull()
		{
			int[] target = null;

			Assert.AreEqual(0, target.NullSafeCount());
		}


		/// <summary>
		/// Tests the string NullSafeCount extension method.
		/// </summary>
		[TestMethod]
		public void NullSafeCount_ReturnsZero_ForEmpty()
		{
			int[] target = new int[0];

			Assert.AreEqual(0, target.NullSafeCount());
		}

		/// <summary>
		/// Tests the string NullSafeCount extension method.
		/// </summary>
		[TestMethod]
		public void NullSafeCount_ReturnsLength_ForNonEmpty()
		{
			int[] target = new int[] { 1, 2, 3, 4, 5 };

			Assert.AreEqual(5, target.NullSafeCount());
		}


		/// <summary>
		/// Tests the string NullSafeCount extension method.
		/// </summary>
		[TestMethod]
		public void NullSafe_ReturnsEmpty_ForNull()
		{
			int[] target = null;

			Assert.AreSame(EmptyArray<int>.Instance, target.NullSafe());
		}


		/// <summary>
		/// Tests the string NullSafeCount extension method.
		/// </summary>
		[TestMethod]
		public void NullSafe_ReturnsSelf_ForNonNull()
		{
			int[] target = new int[] { 1, 2, 3, 4, 5 };

			Assert.AreSame(target, target.NullSafe());
		}


		/// <summary>
		/// Tests that you can iterate over a NullSafe() on a null enumeration.
		/// </summary>
		[TestMethod]
		public void NullSafe_AllowsForEach_ForNull()
		{
			int count = 0;
			int[] target = null;

			foreach (var i in target.NullSafe())
			{
				++count;
			}

			Assert.AreEqual(0, count);
		}


		/// <summary>
		/// Tests that you can iterate over a NullSafe() on a non-null enumeration.
		/// </summary>
		[TestMethod]
		public void NullSafe_AllowsForEach_ForNonNull()
		{
			int count = 0;
			int[] target = new int[] { 1, 2, 3, 4, 5 };

			foreach (var i in target.NullSafe())
			{
				++count;
			}

			Assert.AreEqual(5, count);
		}


		/// <summary>
		/// Iteration over a null enumeration should report a null reference exception.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(System.NullReferenceException))]
		public void NonNullSafe_CrashesForEach_ForNull()
		{
			int count = 0;
			int[] target = null;

			foreach (var i in target)
			{
				count++;
			}
		}


		/// <summary>
		/// Tests the string to see if IsNullOrEmpty applies.
		/// </summary>
		[TestMethod]
		public void IsNullOrEmpty_ReturnsCorrectValue_OnVarious()
		{
			int[] nullArray = null;
			int[] emptyArray = new int[0];
			int[] testArray = new int[] { 1, 2, 3, 4, 5 };

			Assert.IsTrue(nullArray.IsNullOrEmpty());
			Assert.IsTrue(emptyArray.IsNullOrEmpty());
			Assert.IsFalse(testArray.IsNullOrEmpty());
		}


		/// <summary>
		/// Tests the string to see if IsNullOrEmpty applies.
		/// </summary>
		[TestMethod]
		public void IsNotNullOrEmpty_ReturnsCorrectValue_OnVarious()
		{
			int[] nullArray = null;
			int[] emptyArray = new int[0];
			int[] testArray = new int[] { 1, 2, 3, 4, 5 };

			Assert.IsFalse(nullArray.IsNotNullOrEmpty());
			Assert.IsFalse(emptyArray.IsNotNullOrEmpty());
			Assert.IsTrue(testArray.IsNotNullOrEmpty());
		}
	}
}
