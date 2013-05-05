using System.Collections.Generic;
using SharedAssemblies.Core.Containers;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace SharedAssemblies.Core.UnitTests
{    
    /// <summary>
    /// This is a test class for CompositeKeyTest and is intended
    /// to contain all CompositeKeyTest Unit Tests
    /// </summary>
	[TestClass]
	public class CompositeKeyTernaryTest
	{
    	/// <summary>
    	/// Gets or sets the test context which provides
    	/// information about and functionality for the current test run.
    	/// </summary>
    	public TestContext TestContext { get; set; }

		[TestMethod]
		public void CompositeKeyConstructor()
		{
			var key = new CompositeKey<string, int, double>("A", 7, 3.14);

			Assert.AreEqual("A", key.Primary);
			Assert.AreEqual(7, key.Secondary);
			Assert.AreEqual(3.14, key.Ternary);
		}

		[TestMethod]
		public void PrimaryPropertyTest()
		{
			var key = new CompositeKey<string, int, double>("A", 7, 3.14);

			Assert.AreEqual("A", key.Primary);
			Assert.AreEqual(7, key.Secondary);
			Assert.AreEqual(3.14, key.Ternary);
		}

		[TestMethod]
		public void CompareToLessPrimaryTest()
		{
			var smallerKey = new CompositeKey<string, int, double>("A", 7, 1.1);
			var largerKey = new CompositeKey<string, int, double>("Z", 1, 2.2);

			Assert.IsTrue(smallerKey.CompareTo(largerKey) < 0);
		}

		[TestMethod]
		public void CompareToLessSecondaryTest()
		{
			var smallerKey = new CompositeKey<string, int, double>("A", 1, 1.1);
			var largerKey = new CompositeKey<string, int, double>("A", 9, 2.2);

			Assert.IsTrue(smallerKey.CompareTo(largerKey) < 0);
		}

		[TestMethod]
		public void CompareToLessTernaryTest()
		{
			var smallerKey = new CompositeKey<string, int, double>("A", 1, 1.1);
			var largerKey = new CompositeKey<string, int, double>("A", 1, 2.2);

			Assert.IsTrue(smallerKey.CompareTo(largerKey) < 0);
		}

		[TestMethod]
		public void CompareToEqualsTest()
		{
			var smallerKey = new CompositeKey<string, int, double>("A", 13, 1.1);
			var largerKey = new CompositeKey<string, int, double>("A", 13, 1.1);

			Assert.IsTrue(smallerKey.CompareTo(largerKey) == 0);
		}

		[TestMethod]
		public void CompareToGreaterPrimaryTest()
		{
			var smallerKey = new CompositeKey<string, int, double>("Z", 1, 2.2);
			var largerKey = new CompositeKey<string, int, double>("A", 7, 1.1);

			Assert.IsTrue(smallerKey.CompareTo(largerKey) > 0);
		}

		[TestMethod]
		public void CompareToGreaterSecondaryTest()
		{
			var smallerKey = new CompositeKey<string, int, double>("X", 9, 2.2);
			var largerKey = new CompositeKey<string, int, double>("X", 1, 1.1);

			Assert.IsTrue(smallerKey.CompareTo(largerKey) > 0);
		}

		[TestMethod]
		public void CompareToGreaterTernaryTest()
		{
			var smallerKey = new CompositeKey<string, int, double>("X", 1, 2.2);
			var largerKey = new CompositeKey<string, int, double>("X", 1, 1.1);

			Assert.IsTrue(smallerKey.CompareTo(largerKey) > 0);
		}

		[TestMethod]
		public void CompareToLessPrimaryObjectTest()
		{
			var smallerKey = new CompositeKey<string, int, double>("A", 7, 1.1);
			object largerKey = new CompositeKey<string, int, double>("Z", 1, 2.2);

			Assert.IsTrue(smallerKey.CompareTo(largerKey) < 0);
		}

		[TestMethod]
		public void CompareToLessSecondaryObjectTest()
		{
			var smallerKey = new CompositeKey<string, int, double>("A", 1, 1.1);
			object largerKey = new CompositeKey<string, int, double>("A", 9, 2.2);

			Assert.IsTrue(smallerKey.CompareTo(largerKey) < 0);
		}

		[TestMethod]
		public void CompareToLessTernaryObjectTest()
		{
			var smallerKey = new CompositeKey<string, int, double>("A", 1, 1.1);
			object largerKey = new CompositeKey<string, int, double>("A", 1, 2.2);

			Assert.IsTrue(smallerKey.CompareTo(largerKey) < 0);
		}

		[TestMethod]
		public void CompareToEqualsObjectTest()
		{
			var smallerKey = new CompositeKey<string, int, double>("A", 13, 1.1);
			object largerKey = new CompositeKey<string, int, double>("A", 13, 1.1);

			Assert.IsTrue(smallerKey.CompareTo(largerKey) == 0);
		}

		[TestMethod]
		public void CompareToGreaterPrimaryObjectTest()
		{
			var smallerKey = new CompositeKey<string, int, double>("Z", 1, 2.2);
			object largerKey = new CompositeKey<string, int, double>("A", 7, 1.1);

			Assert.IsTrue(smallerKey.CompareTo(largerKey) > 0);
		}

		[TestMethod]
		public void CompareToGreaterSecondaryObjectTest()
		{
			var smallerKey = new CompositeKey<string, int, double>("X", 9, 2.2);
			object largerKey = new CompositeKey<string, int, double>("X", 1, 1.1);

			Assert.IsTrue(smallerKey.CompareTo(largerKey) > 0);
		}

		[TestMethod]
		public void CompareToGreaterTernaryObjectTest()
		{
			var smallerKey = new CompositeKey<string, int, double>("X", 1, 2.2);
			object largerKey = new CompositeKey<string, int, double>("X", 1, 1.1);

			Assert.IsTrue(smallerKey.CompareTo(largerKey) > 0);
		}

		[TestMethod]
		public void EqualsObjectTest()
		{
			var key = new CompositeKey<string, int, double>("A", 13, 2.2);
			object objectKey = new CompositeKey<string, int, double>("A", 13, 2.2);

			Assert.IsTrue(key.Equals(objectKey));
		}

		[TestMethod]
		public void NotEqualsTernaryObjectTest()
		{
			var key = new CompositeKey<string, int, double>("A", 1, 2.3);
			object objectKey = new CompositeKey<string, int, double>("A", 1, 2.2);

			Assert.IsFalse(key.Equals(objectKey));
		}

		[TestMethod]
		public void NotEqualsSecondaryObjectTest()
		{
			var key = new CompositeKey<string, int, double>("A", 2, 2.2);
			object objectKey = new CompositeKey<string, int, double>("A", 1, 2.2);

			Assert.IsFalse(key.Equals(objectKey));
		}

		[TestMethod]
		public void NotEqualsPrimaryObjectTest()
		{
			var key = new CompositeKey<string, int, double>("A", 1, 2.2);
			object objectKey = new CompositeKey<string, int, double>("Z", 1, 2.2);

			Assert.IsFalse(key.Equals(objectKey));
		}

		[TestMethod]
		public void EqualsTest()
		{
			var key = new CompositeKey<string, int, double>("A", 13, 1.1);
			var otherKey = new CompositeKey<string, int, double>("A", 13, 1.1);

			Assert.IsTrue(key.Equals(otherKey));
		}

		[TestMethod]
		public void NotEqualsPrimaryTest()
		{
			var key = new CompositeKey<string, int, double>("A", 13, 1.1);
			var otherKey = new CompositeKey<string, int, double>("B", 13, 1.1);

			Assert.IsFalse(key.Equals(otherKey));
		}

		[TestMethod]
		public void NotEqualsSecondaryTest()
		{
			var key = new CompositeKey<string, int, double>("A", 13, 1.1);
			var otherKey = new CompositeKey<string, int, double>("A", 15, 1.1);

			Assert.IsFalse(key.Equals(otherKey));
		}

		[TestMethod]
		public void NotEqualsTernaryTest()
		{
			var key = new CompositeKey<string, int, double>("A", 15, 1.1);
			var otherKey = new CompositeKey<string, int, double>("A", 15, 2.2);

			Assert.IsFalse(key.Equals(otherKey));
		}

		[TestMethod]
		public void GetHashCodeSameKeysAreSameTest()
		{
			var key = new CompositeKey<string, int, double>("A", 13, 1.1);
			var otherKey = new CompositeKey<string, int, double>("A", 13, 1.1);

			Assert.AreEqual(key.GetHashCode(), otherKey.GetHashCode());
		}

		[TestMethod]
		public void GetHashCodeDifferentKeysAreMostLikelyDifferentTest()
		{
			var key = new CompositeKey<string, int, double>("A", 13, 1.1);
			var otherKey = new CompositeKey<string, int, double>("A", 14, 1.2);

			Assert.AreNotEqual(key.GetHashCode(), otherKey.GetHashCode());
		}

		/// <summary>
		/// Dictionary uses hash codes and equality.
		/// </summary>
		[TestMethod]
		public void PerformDictionaryTest()
		{
			var target = new Dictionary<CompositeKey<string, int, double>, CompositeKey<string, int, double>>();

			for (int i = 0; i < 1000; i++)
			{
				var key = CompositeKeyFactory.Create(i.ToString(), i, (double)i);
				target.Add(key, key);
			}

			for (int i = 0; i < 1000; i++)
			{
				var key = CompositeKeyFactory.Create(i.ToString(), i, (double)i);
				CompositeKey<string, int, double> value;

				Assert.IsTrue(target.TryGetValue(key, out value));
				Assert.AreEqual(key.Primary, value.Primary);
				Assert.AreEqual(key.Secondary, value.Secondary);
			}
		}

		/// <summary>
		/// Sorted dictionary uses comparisons for binary tree
		/// </summary>
		[TestMethod]
		public void PerformSortedDictionaryTest()
		{
			var target = new SortedDictionary<CompositeKey<string, int, double>, CompositeKey<string, int, double>>();

			for (int i = 0; i < 1000; i++)
			{
				var key = CompositeKeyFactory.Create(i.ToString(), i, (double)i);
				target.Add(key, key);
			}

			for (int i = 0; i < 1000; i++)
			{
				var key = CompositeKeyFactory.Create(i.ToString(), i, (double)i);
				CompositeKey<string, int, double> value;

				Assert.IsTrue(target.TryGetValue(key, out value));
				Assert.AreEqual(key.Primary, value.Primary);
				Assert.AreEqual(key.Secondary, value.Secondary);
			}
		}
	}
}
