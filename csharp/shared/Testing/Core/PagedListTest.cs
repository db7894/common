using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.Core.Containers;
using SharedAssemblies.Core.Extensions;


namespace SharedAssemblies.Core.UnitTests
{
	/// <summary>
	/// The set of unit tests for the PagedList class
	/// </summary>
	[TestClass]
	public class PagedListTest
	{
		/// <summary>
		/// Tests the string to see if the IEnumerableConstructor is working as expected.
		/// </summary>
		[TestMethod]
		public void Constructor_BeginIndex_IsSuccessful()
		{
			// Arrange 
			var expected = new List<int>() { 2, 4, 6, 8 };
			
			// Act
			var actual = new PagedList<int>(expected, 0, 1);

			// Assert
			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.HasNextPage);
			Assert.IsFalse(actual.HasPreviousPage);
			Assert.IsTrue(actual.PageCount == 4);
			Assert.IsTrue(actual.PageSize == 1);
			Assert.IsTrue(actual.PageNumber == 1);
			Assert.IsTrue(actual.IsFirstPage);
			Assert.IsFalse(actual.IsLastPage);
		}

		/// <summary>
		/// Tests the string to see if the various properties are working as expected.
		/// </summary>
		[TestMethod]
		public void Constructor_MiddleIndex_IsSuccessful()
		{
			// Arrange 
			var expected = new List<int>() { 2, 4, 6, 8 };

			// Act
			var actual = new PagedList<int>(expected, 2, 1);

			// Assert
			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.HasNextPage);
			Assert.IsTrue(actual.HasPreviousPage);
			Assert.IsTrue(actual.PageCount == 4);
			Assert.IsTrue(actual.PageSize == 1);
			Assert.IsTrue(actual.PageNumber == 3);
			Assert.IsFalse(actual.IsFirstPage);
			Assert.IsFalse(actual.IsLastPage);
		}

		/// <summary>
		/// Tests the string to see if the various properties are working as expected.
		/// </summary>
		[TestMethod]
		public void Constructor_EndIndex_IsSuccessful()
		{
			// Arrange 
			var expected = new List<int>() { 2, 4, 6, 8 };

			// Act
			var actual = new PagedList<int>(expected, 3, 1);

			// Assert
			Assert.IsNotNull(actual);
			Assert.IsFalse(actual.HasNextPage);
			Assert.IsTrue(actual.HasPreviousPage);
			Assert.IsTrue(actual.PageCount == 4);
			Assert.IsTrue(actual.PageSize == 1);
			Assert.IsTrue(actual.PageNumber == 4);
			Assert.IsFalse(actual.IsFirstPage);
			Assert.IsTrue(actual.IsLastPage);
		}

		/// <summary>
		/// Tests the string to see if the various properties are working as expected.
		/// </summary>
		[TestMethod]
		public void Constructor_OutOfRangeIndex_IsSuccessful()
		{
			// Arrange 
			var expected = new List<int>() { 2, 4, 6, 8 };

			// Act (request index out of range)
			var actual = new PagedList<int>(expected, 5, 1);

			// Assert
			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.HasNextPage);
			Assert.IsFalse(actual.HasPreviousPage);
			Assert.IsTrue(actual.PageCount == 4);
			Assert.IsTrue(actual.PageSize == 1);
			Assert.IsTrue(actual.PageNumber == 1);
			Assert.IsTrue(actual.IsFirstPage);
			Assert.IsFalse(actual.IsLastPage);
		}

		/// <summary>
		/// Tests the string to see if the IEnumerableConstructor is working as expected.
		/// </summary>
		[TestMethod]
		public void Constructor_NullInitializer_IsSuccessful()
		{
			// Arrange 
			List<int> expected = null;

			// Act
			var actual = new PagedList<int>(expected, 0, 1);

			// Assert
			Assert.IsNotNull(actual);
			Assert.IsFalse(actual.HasNextPage);
			Assert.IsFalse(actual.HasPreviousPage);
			Assert.IsTrue(actual.PageCount == 0);
			Assert.IsTrue(actual.PageSize == 1);
			Assert.IsTrue(actual.PageNumber == 1);
			Assert.IsTrue(actual.IsFirstPage);
			Assert.IsTrue(actual.IsLastPage);
		}

		/// <summary>
		/// Tests the string to see if the index bounds check is working.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void Constructor_NegativeIndex_Throws()
		{
			// Arrange 
			var expected = new List<int>() { 2, 4, 6, 8 };

			// Act
			var actual = new PagedList<int>(expected, -5, 1);

			// Assert
			// **Should throw above, which would pass this test.
		}

		/// <summary>
		/// Tests the string to see if the page size bounds check is working.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void Constructor_InvalidPageSize_Throws()
		{
			// Arrange 
			var expected = new List<int>() { 2, 4, 6, 8 };

			// Act
			var actual = new PagedList<int>(expected, 0, 0);

			// Assert
			// **Should throw above, which would pass this test.
		}
	}
}
