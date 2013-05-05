using System;
using SharedAssemblies.Core.Comparers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.Core.UnitTests.TestClasses;


namespace SharedAssemblies.Core.UnitTests
{
    /// <summary>
    /// This is a test class for ProjectionComparerTest and is intended
    /// to contain all ProjectionComparerTest Unit Tests
    /// </summary>
	[TestClass]
	public class ProjectionComparerTest
	{
		/// <summary>
		/// Verify the type returned from the factory was inferred from arguments
		/// </summary>
		[TestMethod]
		public void FactoryInfersTypes()
		{
			var result = ProjectionComparer.Create((int i) => i / 2.0);

			Assert.IsInstanceOfType(result, typeof(ProjectionComparer<int, double>));
		}

		/// <summary>
		/// Test non-equal project hash codes are equal
		/// </summary>
		[TestMethod]
		public void ProjectionHashCodesSame()
		{
			var first = new Pair { First = "Apple", Second = "Jacks" };
			var second = new Pair { First = "Apple", Second = "Sauce" };

			var target = ProjectionComparer.Create((Pair p) => p.First);

			Assert.AreEqual(target.GetHashCode(first), target.GetHashCode(second));
		}

		/// <summary>
		/// Test non-equal projection hash codes are not equal (most likely)
		/// </summary>
		[TestMethod]
		public void ProjectionHashCodesDifferent()
		{
			var first = new Pair { First = "Apple", Second = "Jacks" };
			var second = new Pair { First = "Apple", Second = "Sauce" };

			var target = ProjectionComparer.Create((Pair p) => p.Second);

			Assert.AreNotEqual(target.GetHashCode(first), target.GetHashCode(second));
		}

		/// <summary>
		/// Test projection on null argument throws exception
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ProjectionHashCodesNullObjectThrows()
		{
			var target = ProjectionComparer.Create((Pair p) => p.Second);

			target.GetHashCode(null);
		}

		/// <summary>
		/// Test projection on valid argument that returns null returns zero hash code.
		/// </summary>
		[TestMethod]
		public void ProjectionHashCodesNullProjectionReturnsZero()
		{
			var first = new Pair { First = null, Second = "Jacks" };

			var target = ProjectionComparer.Create((Pair p) => p.First);

			Assert.AreEqual(0, target.GetHashCode(first));
		}

		/// <summary>
		/// Test projections to equal fields are equal.
		/// </summary>
		[TestMethod]
		public void ProjectionEqualsTrue()
		{
			var first = new Pair { First = "Apple", Second = "Jacks" };
			var second = new Pair { First = "Apple", Second = "Sauce" };

			var target = ProjectionComparer.Create((Pair p) => p.First);

			Assert.IsTrue(target.Equals(first, second));
			Assert.IsTrue(target.Equals(second, first));
		}

		/// <summary>
		/// Test projects to non-equal fields are not equal.
		/// </summary>
		[TestMethod]
		public void ProjectionEqualsFalse()
		{
			var first = new Pair { First = "Apple", Second = "Jacks" };
			var second = new Pair { First = "Apple", Second = "Sauce" };

			var target = ProjectionComparer.Create((Pair p) => p.Second);

			Assert.IsFalse(target.Equals(first, second));
		}

		/// <summary>
		/// Test projections on null objects are equal
		/// </summary>
		[TestMethod]
		public void ProjectionEqualsBothNullTrue()
		{
			var target = ProjectionComparer.Create((Pair p) => p.First);

			Assert.IsTrue(target.Equals(null, null));
		}

		/// <summary>
		/// Test projections with only one null argument are not equal
		/// </summary>
		[TestMethod]
		public void ProjectionEqualsEitherNullFalse()
		{
			var first = new Pair { First = "Orange", Second = "Whip" };

			var target = ProjectionComparer.Create((Pair p) => p.Second);

			Assert.IsFalse(target.Equals(null, first));
			Assert.IsFalse(target.Equals(first, null));
		}

		/// <summary>
		/// Test projections with both null results are true
		/// </summary>
		[TestMethod]
		public void ProjectionEqualsBothNullFieldTrue()
		{
			var first = new Pair { First = null, Second = "Jacks" };
			var second = new Pair { First = null, Second = "Sauce" };

			var target = ProjectionComparer.Create((Pair p) => p.First);

			Assert.IsTrue(target.Equals(first, second));
		}

		/// <summary>
		/// Test projections with only one null result return false.
		/// </summary>
		[TestMethod]
		public void ProjectionEqualsEitherNullFieldFalse()
		{
			var first = new Pair { First = null, Second = "Jacks" };
			var second = new Pair { First = "Apple", Second = "Sauce" };

			var target = ProjectionComparer.Create((Pair p) => p.Second);

			Assert.IsFalse(target.Equals(first, second));
			Assert.IsFalse(target.Equals(second, first));
		}


		/// <summary>
		/// Test equal projections return zero.
		/// </summary>
		[TestMethod]
		public void ProjectionCompareZero()
		{
			var first = new Pair { First = "Apple", Second = "Jacks" };
			var second = new Pair { First = "Apple", Second = "Sauce" };

			var target = ProjectionComparer.Create((Pair p) => p.First);

			Assert.AreEqual(0, target.Compare(first, second));
			Assert.AreEqual(0, target.Compare(second, first));
		}

		/// <summary>
		/// Test non-equal projections return non-zero
		/// </summary>
		[TestMethod]
		public void ProjectionCompareNonZero()
		{
			var first = new Pair { First = "Apple", Second = "Jacks" };
			var second = new Pair { First = "Apple", Second = "Sauce" };

			var target = ProjectionComparer.Create((Pair p) => p.Second);

			Assert.AreEqual(-1, target.Compare(first, second));
			Assert.AreEqual(1, target.Compare(second, first));
		}

		/// <summary>
		/// Test projections on both null arguments return zero
		/// </summary>
		[TestMethod]
		public void ProjectionCompareBothNullZero()
		{
			var target = ProjectionComparer.Create((Pair p) => p.First);

			Assert.AreEqual(0, target.Compare(null, null));
		}

		/// <summary>
		/// Test projections with either argument null return non-zero.
		/// </summary>
		[TestMethod]
		public void ProjectionCompareEitherNullNonZero()
		{
			var first = new Pair { First = "Orange", Second = "Whip" };

			var target = ProjectionComparer.Create((Pair p) => p.Second);

			Assert.AreEqual(-1, target.Compare(null, first));
			Assert.AreEqual(1, target.Compare(first, null));
		}

		/// <summary>
		/// Test projections on valid objects with null results return zero
		/// </summary>
		[TestMethod]
		public void ProjectionCompareBothNullFieldZero()
		{
			var first = new Pair { First = null, Second = "Jacks" };
			var second = new Pair { First = null, Second = "Sauce" };

			var target = ProjectionComparer.Create((Pair p) => p.First);

			Assert.AreEqual(0, target.Compare(first, second));
		}

		/// <summary>
		/// Test projections with valid objects with only one null result returns non-zero.
		/// </summary>
		[TestMethod]
		public void ProjectionCompareEitherNullFieldNonZero()
		{
			var first = new Pair { First = null, Second = "Jacks" };
			var second = new Pair { First = "Apple", Second = "Sauce" };

			var target = ProjectionComparer.Create((Pair p) => p.Second);

			Assert.AreEqual(-1, target.Compare(first, second));
			Assert.AreEqual(1, target.Compare(second, first));
		}
	}
}
