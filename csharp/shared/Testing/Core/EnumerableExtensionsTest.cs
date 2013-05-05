using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.Core.Containers;
using SharedAssemblies.Core.Extensions;


namespace SharedAssemblies.Core.UnitTests
{
	/// <summary>
	/// The set of unit tests for the EnumerableExtensions class
	/// </summary>
	[TestClass]
	public class EnumerableExtensionsTest
	{
		/// <summary>
		/// Gets or sets the test context which provides
		/// information about and functionality for the current test run.
		/// </summary>
		public TestContext TestContext { get; set; }


		/// <summary>
		/// Tests the string NullSafeCount extension method.
		/// </summary>
		[TestMethod]
		public void NullSafeCount_ReturnsZero_ForNull()
		{
			List<int> target = null;

			Assert.AreEqual(0, target.NullSafeCount());
		}


		/// <summary>
		/// Tests the string NullSafeCount extension method.
		/// </summary>
		[TestMethod]
		public void NullSafeCount_ReturnsZero_ForEmpty()
		{
			List<int> target = new List<int>();

			Assert.AreEqual(0, target.NullSafeCount());
		}

		/// <summary>
		/// Tests the string NullSafeCount extension method.
		/// </summary>
		[TestMethod]
		public void NullSafeCount_ReturnsLength_ForNonEmpty()
		{
			List<int> target = new List<int> { 1, 2, 3, 4, 5 };

			Assert.AreEqual(5, target.NullSafeCount());
		}


		/// <summary>
		/// Tests the string NullSafeCount extension method.
		/// </summary>
		[TestMethod]
		public void NullSafe_ReturnsEmpty_ForNull()
		{
			List<int> target = null;

			Assert.AreSame(Enumerable.Empty<int>(), target.NullSafe());
		}


		/// <summary>
		/// Tests the string NullSafeCount extension method.
		/// </summary>
		[TestMethod]
		public void NullSafe_ReturnsSelf_ForNonNull()
		{
			List<int> target = new List<int> { 1, 2, 3, 4, 5 };

			Assert.AreSame(target, target.NullSafe());
		}


		/// <summary>
		/// Tests that you can iterate over a NullSafe() on a null enumeration.
		/// </summary>
		[TestMethod]
		public void NullSafe_AllowsForEach_ForNull()
		{
			int count = 0;
			List<int> target = null;

			foreach(var i in target.NullSafe())
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
			List<int> target = new List<int> { 1, 2, 3, 4, 5 };

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
			List<int> target = null;

			foreach(var i in target)
			{
				count++;
			}
		}


		/// <summary>
		/// Tests the string to see if IsNullOrEmpty applies.
		/// </summary>
		[TestMethod]
		public void EnumeratedIsNullOrEmpty_ReturnsCorrectValue_OnVarious()
		{
			IEnumerable<int> nullCollection = null;
			IEnumerable<int> emptyCollection = new List<int>();
			IEnumerable<int> testCollection = new List<int>() { 1, 2, 3, 4, 5 };

			Assert.IsTrue(nullCollection.IsNullOrEmpty());
			Assert.IsTrue(emptyCollection.IsNullOrEmpty());
			Assert.IsFalse(testCollection.IsNullOrEmpty());
		}


		/// <summary>
		/// Tests the string to see if IsNullOrEmpty applies.
		/// </summary>
		[TestMethod]
		public void EnumeratedIsNotNullOrEmpty_ReturnsCorrectValue_OnVarious()
		{
			IEnumerable<int> nullCollection = null;
			IEnumerable<int> emptyCollection = new List<int>();
			IEnumerable<int> testCollection = new List<int>() { 1, 2, 3, 4, 5 };

			Assert.IsFalse(nullCollection.IsNotNullOrEmpty());
			Assert.IsFalse(emptyCollection.IsNotNullOrEmpty());
			Assert.IsTrue(testCollection.IsNotNullOrEmpty());
		}


		/// <summary>
		/// Tests the string to see if IsNullOrEmpty applies.
		/// </summary>
		[TestMethod]
		public void CollectionIsNullOrEmpty_ReturnsCorrectValue_OnVarious()
		{
			ICollection<int> nullCollection = null;
			ICollection<int> emptyCollection = new List<int>();
			ICollection<int> testCollection = new List<int>() { 1, 2, 3, 4, 5 };

			Assert.IsTrue(nullCollection.IsNullOrEmpty());
			Assert.IsTrue(emptyCollection.IsNullOrEmpty());
			Assert.IsFalse(testCollection.IsNullOrEmpty());
		}


		/// <summary>
		/// Tests the string to see if IsNullOrEmpty applies.
		/// </summary>
		[TestMethod]
		public void CollectionIsNotNullOrEmpty_ReturnsCorrectValue_OnVarious()
		{
			ICollection<int> nullCollection = null;
			ICollection<int> emptyCollection = new List<int>();
			ICollection<int> testCollection = new List<int>() { 1, 2, 3, 4, 5 };

			Assert.IsFalse(nullCollection.IsNotNullOrEmpty());
			Assert.IsFalse(emptyCollection.IsNotNullOrEmpty());
			Assert.IsTrue(testCollection.IsNotNullOrEmpty());
		}

		/// <summary>
		/// Tests the string to see if IsNullOrEmpty applies.
		/// </summary>
		[TestMethod]
		public void IsNullOrEmpty_ReturnsCorrectValue_OnVarious()
		{
			List<int> nullCollection = null;
			List<int> emptyCollection = new List<int>();
			List<int> testCollection = new List<int>() { 1, 2, 3, 4, 5 };

			Assert.IsTrue(nullCollection.IsNullOrEmpty());
			Assert.IsTrue(emptyCollection.IsNullOrEmpty());
			Assert.IsFalse(testCollection.IsNullOrEmpty());
		}


		/// <summary>
		/// Tests the string to see if IsNullOrEmpty applies.
		/// </summary>
		[TestMethod]
		public void IsNotNullOrEmpty_ReturnsCorrectValue_OnVarious()
		{
			List<int> nullCollection = null;
			List<int> emptyCollection = new List<int>();
			List<int> testCollection = new List<int>() { 1, 2, 3, 4, 5 };

			Assert.IsFalse(nullCollection.IsNotNullOrEmpty());
			Assert.IsFalse(emptyCollection.IsNotNullOrEmpty());
			Assert.IsTrue(testCollection.IsNotNullOrEmpty());
		}

		/// <summary>
		/// Tests the string to see if IsNullOrEmpty applies.
		/// </summary>
		[TestMethod]
		public void Shuffle_ReturnsRandomValue()
		{
			List<int> testCollection = new List<int>() { 1, 2, 3, 4, 5 };

			CollectionAssert.AreNotEqual(testCollection, testCollection.Shuffle().ToList());
		}

		/// <summary>
		/// Test for ToSet
		/// </summary>
		[TestMethod]
		public void ToSetRemovesDuplicates()
		{
			var list = new List<string> { "A", "B", "C", "D", "A", "B" };
			var target = list.ToHashSet();

			Assert.AreEqual(4, target.Count);
			Assert.IsTrue(target.Contains("A"));
			Assert.IsTrue(target.Contains("B"));
			Assert.IsTrue(target.Contains("C"));
			Assert.IsTrue(target.Contains("D"));
		}

		/// <summary>
		/// Test for ToSet
		/// </summary>
		[TestMethod]
		public void ToSetOnEmptyReturnsEmpty()
		{
			var list = new List<string>(0);
			var target = list.ToHashSet();

			Assert.AreEqual(0, target.Count);
		}

		/// <summary>
		/// Test for ToSet
		/// </summary>
		[TestMethod]
		public void ToSetOnEmptyWithSelectorReturnsEmpty()
		{
			var list = new List<string>(0);
			var target = list.ToHashSet(s => s);

			Assert.AreEqual(0, target.Count);
		}

		/// <summary>
		/// Test for ToSet
		/// </summary>
		[TestMethod]
		public void ToSetOnEmptyWithComparerReturnsEmpty()
		{
			var list = new List<string>(0);
			var target = list.ToHashSet(EqualityComparer<string>.Default);

			Assert.AreEqual(0, target.Count);
		}

		/// <summary>
		/// Test for ToSet
		/// </summary>
		[TestMethod]
		public void ToSetOnEmptyWithSelectorAndComparerReturnsEmpty()
		{
			var list = new List<string>(0);
			var target = list.ToHashSet(s => s, EqualityComparer<string>.Default);

			Assert.AreEqual(0, target.Count);
		}

		/// <summary>
		/// Test for ToSet
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ToSetOnNullThrows()
		{
			List<string> list = null;
			var target = list.ToHashSet();
		}

		/// <summary>
		/// Test for ToSet
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ToSetOnNullWithSelectorThrows()
		{
			List<string> list = null;
			var target = list.ToHashSet(s => s);
		}

		/// <summary>
		/// Test for ToSet
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ToSetOnNullWithComparerThrows()
		{
			List<string> list = null;
			var target = list.ToHashSet(EqualityComparer<string>.Default);
		}

		/// <summary>
		/// Test for ToSet
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ToSetOnNullWithSelectorAndComparerThrows()
		{
			List<string> list = null;
			var target = list.ToHashSet(s => s, EqualityComparer<string>.Default);
		}

		/// <summary>
		/// Test for ToSet
		/// </summary>
		[TestMethod]
		public void ToSetWithNullComparerUsesDefault()
		{
			IEqualityComparer<string> comparer = null;
			var list = new List<string> { "A", "AA", "A", "AB", "AA", "BB" };

			var target = list.ToHashSet(comparer);

			Assert.AreEqual(4, target.Count);
			Assert.IsTrue(target.Contains("A"));
			Assert.IsTrue(target.Contains("AA"));
			Assert.IsTrue(target.Contains("AB"));
			Assert.IsTrue(target.Contains("BB"));
		}

		/// <summary>
		/// Test for ToSet
		/// </summary>
		[TestMethod]
		public void ToSetWithNullComparerWithSelectorUsesDefault()
		{
			IEqualityComparer<int> comparer = null;
			var list = new List<string> { "A", "AA", "A", "AB", "AA", "BB" };

			var target = list.ToHashSet(s => s.Length, comparer);

			Assert.AreEqual(2, target.Count);
			Assert.IsTrue(target.Contains(1));
			Assert.IsTrue(target.Contains(2));
		}

		/// <summary>
		/// Test for ToSet
		/// </summary>
		[TestMethod]
		public void ToSetWithSelector()
		{
			var list = new List<string> { "A", "AA", "AAA", "B", "BB", "BBB" };
			var target = list.ToHashSet(l => l.Length);

			Assert.AreEqual(3, target.Count);
			Assert.IsTrue(target.Contains(1));
			Assert.IsTrue(target.Contains(2));
			Assert.IsTrue(target.Contains(3));
		}

		/// <summary>
		/// Test for ToSet
		/// </summary>
		[TestMethod]
		public void ToSetWithComparer()
		{
			var list = new List<string> { "A", "a", "AA", "Aa", "aA" };
			var target = list.ToHashSet(StringComparer.CurrentCultureIgnoreCase);

			Assert.AreEqual(2, target.Count);
			Assert.IsTrue(target.Contains("A"));
			Assert.IsTrue(target.Contains("AA"));
		}

		/// <summary>
		/// Test for ToSet
		/// </summary>
		[TestMethod]
		public void ToSetWithComparerAndSelector()
		{
			var list = new List<string> { "A", "a", "BA", "Ba", "bA" };
			var target = list.ToHashSet(s => s += '!', StringComparer.CurrentCultureIgnoreCase);

			Assert.AreEqual(2, target.Count);
			Assert.IsTrue(target.Contains("A!"));
			Assert.IsTrue(target.Contains("BA!"));
		}

		/// <summary>
		/// Test for ToSet
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ToSetOnNullSelectorThrows()
		{
			Func<string, int> selector = null;
			var list = new List<string> { "A", "AA", "AAA", "B", "BB", "BBB" };
			var target = list.ToHashSet(selector);
		}

		/// <summary>
		/// Test for ToSet
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ToSetOnNullSelectorWithComparerThrows()
		{
			Func<string, string> selector = null;
			var list = new List<string> { "A", "AA", "AAA", "B", "BB", "BBB" };
			var target = list.ToHashSet(selector, EqualityComparer<string>.Default);
		}

		/// <summary>
		/// Tests for range
		/// </summary>
		public void TestGetRangeOnString()
		{
			Assert.AreEqual(new Tuple<string, string>(null, null), new string[0].GetRange());
			Assert.AreEqual(new Tuple<string, string>("A", "A"), new [] { "A" }.GetRange());
			Assert.AreEqual(new Tuple<string, string>("A", "Z"), new[] { "Z", "A" }.GetRange());
		}

		/// <summary>
		/// Tests for range
		/// </summary>
		public void TestGetRangOnInt()
		{
			Assert.AreEqual(new Tuple<int, int>(0, 0), new int[0].GetRange());
			Assert.AreEqual(new Tuple<int, int>(3, 3), new[] { 3 }.GetRange());
			Assert.AreEqual(new Tuple<int, int>(-999, 9), new[] { 7, 9, -999 }.GetRange());
		}
	}
}
