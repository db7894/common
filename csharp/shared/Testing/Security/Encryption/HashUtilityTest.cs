﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.Security.Encryption;

namespace SharedAssemblies.Security.UnitTests.Encryption
{
	/// <summary>
	/// HashUtility test fixture
	/// </summary>
	[TestClass]
	public class HashUtilityTest
	{
		#region With Pepper

		/// <summary>
		/// Tests that the hash utility works correctly with binary arrays,
		/// for ideal and edge cases.
		/// </summary>
		[TestMethod]
		public void HashUtility_ComputeHash_WithBinary_Succeeds()
		{
			byte[] input = new byte[] { 0x12, 0x34, 0x56, 0x78, 0x9A };
			var expected = new byte[]
			{
				0x05, 0x8d, 0x4c, 0xd6, 0xd7, 0xe8, 0x41, 0x9a,
				0xcf, 0xa8, 0x2f, 0xc0, 0x89, 0x23, 0xb7, 0xac,
				0x70, 0xd4, 0x76, 0x03, 0xf6, 0x99, 0xd0, 0x78,
				0x1f, 0x19, 0x36, 0xd7, 0xfd, 0x84, 0xa0, 0xba,
			};
			var actual = HashUtility.ComputeHash(input);
			CollectionAssert.AreEqual(actual, expected);

			byte[] input2 = new byte[] { };
			var expected2 = new byte[]
			{
				0xe3, 0xb0, 0xc4, 0x42, 0x98, 0xfc, 0x1c, 0x14,
				0x9a, 0xfb, 0xf4, 0xc8, 0x99, 0x6f, 0xb9, 0x24,
				0x27, 0xae, 0x41, 0xe4, 0x64, 0x9b, 0x93, 0x4c,
				0xa4, 0x95, 0x99, 0x1b, 0x78, 0x52, 0xb8, 0x55,
			};
			var actual2 = HashUtility.ComputeHash(input2);
			CollectionAssert.AreEqual(actual2, expected2);

			byte[] input3 = null;
			var actual3 = HashUtility.ComputeHash(input3);
			Assert.IsNull(actual3);
		}

		/// <summary>
		/// Tests that the hash utility works correctly with strings,
		/// for ideal and edge cases.
		/// </summary>
		[TestMethod]
		public void HashUtility_ComputeHash_WithString_Succeeds()
		{
			string input = "SomeDataThatNeedsToBeHashed";
			var expected = new byte[]
			{
				0xdc, 0x78, 0xb7, 0xa7, 0x5b, 0x87, 0xd3, 0x01,
				0x77, 0xe9, 0x0f, 0x1c, 0xf5, 0x87, 0x21, 0x82,
				0xf5, 0x35, 0x2c, 0x50, 0xb0, 0x41, 0xb3, 0xf4,
				0xf1, 0x51, 0xbe, 0x09, 0x46, 0x73, 0x51, 0xdc,
			};
			var actual = HashUtility.ComputeHash(input);
			CollectionAssert.AreEqual(actual, expected);

			string input2 = string.Empty;
			var expected2 = new byte[]
			{
				0xe3, 0xb0, 0xc4, 0x42, 0x98, 0xfc, 0x1c, 0x14,
				0x9a, 0xfb, 0xf4, 0xc8, 0x99, 0x6f, 0xb9, 0x24,
				0x27, 0xae, 0x41, 0xe4, 0x64, 0x9b, 0x93, 0x4c,
				0xa4, 0x95, 0x99, 0x1b, 0x78, 0x52, 0xb8, 0x55,
			};
			var actual2 = HashUtility.ComputeHash(input2);
			CollectionAssert.AreEqual(actual2, expected2);

			string input3 = null;
			var actual3 = HashUtility.ComputeHash(input3);
			Assert.IsNull(actual3);
		}

		/// <summary>
		/// Tests that the hash utility works correctly with types,
		/// for ideal and edge cases.
		/// </summary>
		[TestMethod]
		public void HashUtility_ComputeHash_WithType_Succeeds()
		{
			List<string> input = new List<string> { "first", "second", "third" };
			var expected = new byte[]
			{
				0xc0, 0x40, 0x32, 0x94, 0xf6, 0x47, 0x6e, 0x0f,
				0xa1, 0xbb, 0xad, 0xe1, 0xbc, 0x8d, 0x93, 0x6f,
				0x95, 0xcb, 0x2c, 0x8a, 0x48, 0x61, 0x5a, 0xad,
				0x16, 0x27, 0x41, 0xfc, 0xa3, 0x60, 0x34, 0xff,
			};
			var actual = HashUtility.ComputeHash(input);
			CollectionAssert.AreEqual(actual, expected);

			List<string> input2 = new List<string>();
			var expected2 = new byte[]
			{
				0x7a, 0x31, 0x53, 0xa6, 0xa3, 0xae, 0x34, 0x06,
				0x08, 0xb7, 0x47, 0xfc, 0xfc, 0x43, 0x89, 0xfa,
				0x78, 0xae, 0x38, 0xb0, 0x97, 0xdf, 0x8d, 0x5b,
				0x02, 0x0a, 0x40, 0xce, 0x07, 0x08, 0x68, 0x7f,
			};
			var actual2 = HashUtility.ComputeHash(input2);
			CollectionAssert.AreEqual(actual2, expected2);

			List<string> input3 = null;
			var actual3 = HashUtility.ComputeHash(input3);
			Assert.IsNull(actual3);
		}

		/// <summary>
		/// Tests that the hash utility works correctly with types,
		/// for ideal and edge cases.
		/// </summary>
		[TestMethod]
		public void HashUtility_ComputeHash_WithStruct_Succeeds()
		{
			DateTime input = new DateTime(2010, 12, 12);
			var expected = new byte[]
			{
				0x04, 0x36, 0xef, 0x8d, 0x5a, 0x56, 0x56, 0x01,
				0xf7, 0x43, 0xc9, 0xf4, 0x29, 0x8b, 0xdf, 0x4c,
				0xaa, 0xba, 0x74, 0x82, 0x9a, 0x9d, 0x15, 0xb3,
				0x6c, 0x9f, 0x89, 0x45, 0x83, 0xaa, 0x67, 0x8d,
			};
			var actual = HashUtility.ComputeStructHash(input);
			CollectionAssert.AreEqual(actual, expected);

			DateTime input2 = new DateTime();
			var expected2 = new byte[]
			{
				0x3e, 0x3e, 0x72, 0xc7, 0xb3, 0x59, 0x8b, 0x48,
				0x82, 0xf8, 0xa8, 0x60, 0xd9, 0x78, 0x9b, 0xba,
				0x15, 0x8a, 0x40, 0xea, 0xdc, 0x09, 0x74, 0x0e,
				0xd3, 0x84, 0x3e, 0x2b, 0x17, 0x25, 0x96, 0x78,
			};
			var actual2 = HashUtility.ComputeStructHash(input2);
			CollectionAssert.AreEqual(actual2, expected2);

			DateTime? input3 = new DateTime(2010, 12, 12);
			var expected3 = new byte[]
			{
				0x04, 0x36, 0xef, 0x8d, 0x5a, 0x56, 0x56, 0x01,
				0xf7, 0x43, 0xc9, 0xf4, 0x29, 0x8b, 0xdf, 0x4c,
				0xaa, 0xba, 0x74, 0x82, 0x9a, 0x9d, 0x15, 0xb3,
				0x6c, 0x9f, 0x89, 0x45, 0x83, 0xaa, 0x67, 0x8d,
			};
			var actual3 = HashUtility.ComputeStructHash(input3);
			CollectionAssert.AreEqual(actual3, expected3);

			DateTime? input4 = null;
			var actual4 = HashUtility.ComputeStructHash(input4);
			Assert.IsNull(actual4);
		}

		#endregion

		#region With Salt

		/// <summary>
		/// Tests that the hash utility works correctly with binary arrays,
		/// for ideal and edge cases.
		/// </summary>
		[TestMethod]
		public void HashUtility_ComputeHashWithEmptySalt_WithBinary_Succeeds()
		{
			var salt  = new byte[] {};
			var input = new byte[] { 0x12, 0x34, 0x56, 0x78, 0x9A };
			var expected = new byte[]
			{
				0x05, 0x8d, 0x4c, 0xd6, 0xd7, 0xe8, 0x41, 0x9a,
				0xcf, 0xa8, 0x2f, 0xc0, 0x89, 0x23, 0xb7, 0xac,
				0x70, 0xd4, 0x76, 0x03, 0xf6, 0x99, 0xd0, 0x78,
				0x1f, 0x19, 0x36, 0xd7, 0xfd, 0x84, 0xa0, 0xba,
			};
			var actual = HashUtility.ComputeHash(input, salt);
			CollectionAssert.AreEqual(actual, expected);
		}

		/// <summary>
		/// Tests that the hash utility works correctly with binary arrays,
		/// for ideal and edge cases.
		/// </summary>
		[TestMethod]
		public void HashUtility_ComputeHashWithSalt_WithBinary_Succeeds()
		{
			var salt  = new byte[] { 0x00, 0x01, 0x02, 0x03 };
			var input = new byte[] { 0x12, 0x34, 0x56, 0x78, 0x9A };
			var expected = new byte[]
			{
				0x54, 0xa8, 0x70, 0xd7, 0x34, 0x9f, 0x95, 0x0a,
				0x1f, 0xea, 0x47, 0x3d, 0xbd, 0xff, 0xb0, 0xfd,
				0x2d, 0xda, 0x4d, 0xd1, 0x55, 0xe2, 0x49, 0xe0,
				0x0b, 0x29, 0xdb, 0xe2, 0xc7, 0x2a, 0xa1, 0xac,
			};
			var actual = HashUtility.ComputeHash(input, salt);
			CollectionAssert.AreEqual(actual, expected);
		}

		/// <summary>
		/// Tests that the hash utility works correctly with strings,
		/// for ideal and edge cases.
		/// </summary>
		[TestMethod]
		public void HashUtility_ComputeHashWithSalt_WithString_Succeeds()
		{
			var salt  = new byte[] { 0x00, 0x01, 0x02, 0x03 };
			var input = "SomeDataThatNeedsToBeHashed";
			var expected = new byte[]
			{
				0xad, 0x2c, 0x56, 0x6b, 0xbd, 0xf3, 0xe8, 0x86,
				0xac, 0xe9, 0x9b, 0xf5, 0xbd, 0x45, 0x6e, 0x5b,
				0xde, 0x54, 0x08, 0xb1, 0x52, 0x9a, 0xe1, 0xce,
				0x00, 0x26, 0x13, 0xae, 0x28, 0xb0, 0xb2, 0x78,
			};
			var actual = HashUtility.ComputeHash(input, salt);
			CollectionAssert.AreEqual(actual, expected);
		}

		/// <summary>
		/// Tests that the hash utility works correctly with types,
		/// for ideal and edge cases.
		/// </summary>
		[TestMethod]
		public void HashUtility_ComputeHashWithSalt_WithType_Succeeds()
		{
			var salt  = new byte[] { 0x00, 0x01, 0x02, 0x03 };
			var input = new List<string> { "first", "second", "third" };
			var expected = new byte[]
			{
				0xee, 0x23, 0xb2, 0x4e, 0x15, 0x65, 0x86, 0xe9,
				0xdb, 0x45, 0xaf, 0xc7, 0x1a, 0x4b, 0xb6, 0x8b,
				0x47, 0x0e, 0x6d, 0x0c, 0x14, 0xfa, 0xf2, 0x69,
				0x96, 0xd4, 0x07, 0xba, 0xf9, 0xd5, 0x1f, 0xf2,
			};
			var actual = HashUtility.ComputeHash(input, salt);
			CollectionAssert.AreEqual(actual, expected);
		}

		/// <summary>
		/// Tests that the hash utility works correctly with types,
		/// for ideal and edge cases.
		/// </summary>
		[TestMethod]
		public void HashUtility_ComputeHashWithSalt_WithStruct_Succeeds()
		{
			var salt  = new byte[] { 0x00, 0x01, 0x02, 0x03 };
			var input = new DateTime(2010, 12, 12);
			var expected = new byte[]
			{
				0x99, 0x55, 0x50, 0xd3, 0xad, 0x4e, 0x39, 0x0f,
				0x81, 0x03, 0x37, 0x90, 0xbe, 0x92, 0x53, 0xeb,
				0x5c, 0x51, 0xfe, 0xe0, 0x50, 0x13, 0x99, 0x3c,
				0x88, 0x62, 0x41, 0x15, 0xb6, 0x77, 0xcd, 0xe4,
			};
			var actual = HashUtility.ComputeStructHash(input, salt);
			CollectionAssert.AreEqual(actual, expected);
		}

		#endregion
	}
}
