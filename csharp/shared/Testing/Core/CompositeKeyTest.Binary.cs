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
	public class CompositeKeyTest
	{
		/// <summary>
		/// Gets or sets the test context which provides
		/// information about and functionality for the current test run.
		/// </summary>
		public TestContext TestContext { get; set; }

		[TestMethod]
		public void CompositeKeyConstructor()
		{
			var key = new CompositeKey<string, int>("A", 7);

			Assert.AreEqual("A", key.Primary);
			Assert.AreEqual(7, key.Secondary);
		}

		[TestMethod]
		public void PrimaryPropertyTest()
		{
			var key = new CompositeKey<string, int>("A", 7);

			Assert.AreEqual("A", key.Primary);
			Assert.AreEqual(7, key.Secondary);
		}

		[TestMethod]
		public void CompareToLessPrimaryTest()
		{
			var smallerKey = new CompositeKey<string, int>("A", 7);
			var largerKey = new CompositeKey<string, int>("Z", 1);

			Assert.IsTrue(smallerKey.CompareTo(largerKey) < 0);
		}

		[TestMethod]
		public void CompareToLessSecondaryTest()
		{
			var smallerKey = new CompositeKey<string, int>("A", 1);
			var largerKey = new CompositeKey<string, int>("A", 9);

			Assert.IsTrue(smallerKey.CompareTo(largerKey) < 0);
		}

		[TestMethod]
		public void CompareToEqualsTest()
		{
			var smallerKey = new CompositeKey<string, int>("A", 13);
			var largerKey = new CompositeKey<string, int>("A", 13);

			Assert.IsTrue(smallerKey.CompareTo(largerKey) == 0);
		}

		[TestMethod]
		public void CompareToGreaterPrimaryTest()
		{
			var smallerKey = new CompositeKey<string, int>("Z", 1);
			var largerKey = new CompositeKey<string, int>("A", 7);

			Assert.IsTrue(smallerKey.CompareTo(largerKey) > 0);
		}

		[TestMethod]
		public void CompareToGreaterSecondaryTest()
		{
			var smallerKey = new CompositeKey<string, int>("X", 9);
			var largerKey = new CompositeKey<string, int>("X", 1);

			Assert.IsTrue(smallerKey.CompareTo(largerKey) > 0);
		}

		[TestMethod]
		public void CompareToLessPrimaryObjectTest()
		{
			var smallerKey = new CompositeKey<string, int>("A", 7);
			object largerKey = new CompositeKey<string, int>("Z", 1);

			Assert.IsTrue(smallerKey.CompareTo(largerKey) < 0);
		}

		[TestMethod]
		public void CompareToLessSecondaryObjectTest()
		{
			var smallerKey = new CompositeKey<string, int>("A", 1);
			object largerKey = new CompositeKey<string, int>("A", 9);

			Assert.IsTrue(smallerKey.CompareTo(largerKey) < 0);
		}

		[TestMethod]
		public void CompareToEqualsObjectTest()
		{
			var smallerKey = new CompositeKey<string, int>("A", 13);
			object largerKey = new CompositeKey<string, int>("A", 13);

			Assert.IsTrue(smallerKey.CompareTo(largerKey) == 0);
		}

		[TestMethod]
		public void CompareToGreaterPrimaryObjectTest()
		{
			var smallerKey = new CompositeKey<string, int>("Z", 1);
			object largerKey = new CompositeKey<string, int>("A", 7);

			Assert.IsTrue(smallerKey.CompareTo(largerKey) > 0);
		}

		[TestMethod]
		public void CompareToGreaterSecondaryObjectTest()
		{
			var smallerKey = new CompositeKey<string, int>("X", 9);
			object largerKey = new CompositeKey<string, int>("X", 1);

			Assert.IsTrue(smallerKey.CompareTo(largerKey) > 0);
		}

		[TestMethod]
		public void EqualsObjectTest()
		{
			var key = new CompositeKey<string, int>("A", 13);
			object objectKey = new CompositeKey<string, int>("A", 13);

			Assert.IsTrue(key.Equals(objectKey));
		}

		[TestMethod]
		public void NotEqualsObjectTest()
		{
			var key = new CompositeKey<string, int>("A", 13);
			object objectKey = new CompositeKey<string, int>("A", 15);

			Assert.IsFalse(key.Equals(objectKey));
		}

		[TestMethod]
		public void EqualsTest()
		{
			var key = new CompositeKey<string, int>("A", 13);
			var otherKey = new CompositeKey<string, int>("A", 13);

			Assert.IsTrue(key.Equals(otherKey));
		}

		[TestMethod]
		public void NotEqualsTest()
		{
			var key = new CompositeKey<string, int>("A", 13);
			var otherKey = new CompositeKey<string, int>("A", 15);

			Assert.IsFalse(key.Equals(otherKey));
		}

		[TestMethod]
		public void GetHashCodeSameKeysAreSameTest()
		{
			var key = new CompositeKey<string, int>("A", 13);
			var otherKey = new CompositeKey<string, int>("A", 13);

			Assert.AreEqual(key.GetHashCode(), otherKey.GetHashCode());
		}

		[TestMethod]
		public void GetHashCodeDifferentKeysAreMostLikelyDifferentTest()
		{
			var key = new CompositeKey<string, int>("A", 13);
			var otherKey = new CompositeKey<string, int>("A", 14);

			Assert.AreNotEqual(key.GetHashCode(), otherKey.GetHashCode());
		}

		/// <summary>
		/// Dictionary uses hash codes and equality.
		/// </summary>
		[TestMethod]
		public void PerformDictionaryTest()
		{
			var target = new Dictionary<CompositeKey<string, int>, CompositeKey<string, int>>();

			for (int i = 0; i < 1000; i++)
			{
				var key = CompositeKeyFactory.Create(i.ToString(), i);
				target.Add(key, key);
			}

			for (int i = 0; i < 1000; i++)
			{
				var key = CompositeKeyFactory.Create(i.ToString(), i);
				CompositeKey<string, int> value;

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
			var target = new SortedDictionary<CompositeKey<string, int>, CompositeKey<string, int>>();

			for (int i = 0; i < 1000; i++)
			{
				var key = CompositeKeyFactory.Create(i.ToString(), i);
				target.Add(key, key);
			}

			for (int i = 0; i < 1000; i++)
			{
				var key = CompositeKeyFactory.Create(i.ToString(), i);
				CompositeKey<string, int> value;

				Assert.IsTrue(target.TryGetValue(key, out value));
				Assert.AreEqual(key.Primary, value.Primary);
				Assert.AreEqual(key.Secondary, value.Secondary);
			}
		}
	}
}
