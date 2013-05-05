using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.Core.Extensions;
using SharedAssemblies.General.Testing;

namespace SharedAssemblies.Core.UnitTests
{
    /// <summary>
    /// Test fixture for string extensions
    /// </summary>
    [TestClass]
	public class StringExtensionTest
    {
        /// <summary>
        /// A simple test string
        /// </summary>
        private const string _testString = "This is a test string";

        /// <summary>
        /// Test that isLengthAtLeast returns false when string is null
        /// </summary>
        [TestMethod]
        public void IsLengthAtLeast_ReturnsFalse_OnNullString()
        {
            string s = null;
            Assert.IsFalse(s.IsLengthAtLeast(4));
        }


        /// <summary>
        /// Test that IsLengthAtLeast() returns true when length less
        /// </summary>
        [TestMethod]
        public void IsLengthAtLeast_ReturnsTrue_WhenLengthLess()
        {
            for (int i = 0; i < _testString.Length; ++i)
            {
                Assert.IsTrue(_testString.IsLengthAtLeast(i));
            }
        }


        /// <summary>
        /// Test that IsLengthAtLeast returns true when lengths are equal
        /// </summary>
        [TestMethod]
        public void IsLengthAtLeast_ReturnsTrue_WhenLengthEqual()
        {
            Assert.IsTrue(_testString.IsLengthAtLeast(_testString.Length));
        }


        /// <summary>
        /// Test IsLengthAtLeast() returns false when legnth greater
        /// </summary>
        [TestMethod]
        public void IsLengthAtLeast_ReturnsFalse_WhenLengthGreater()
        {
            Assert.IsFalse(_testString.IsLengthAtLeast(_testString.Length + 1));
        }


        /// <summary>
        /// Test IsLengthAtLeast throws when length is negative
        /// </summary>
        [TestMethod]
        public void IsLengthAtLeast_ThrowsException_WhenLengthNegative()
        {
			AssertEx.Throws<ArgumentOutOfRangeException>(() =>
                _testString.IsLengthAtLeast(-1));
        }


        /// <summary>
        /// Test mask masks the last character when called explicit
        /// </summary>
        [TestMethod]
        public void Mask_MasksLastCharacters_WhenCalledWithExplicit()
        {
            string target   = "123-456-7890";
            string expected = "++++++++7890";

            Assert.AreEqual(expected, target.Mask('+', 4));
        }


        /// <summary>
        /// Test mask works whith default mask
        /// </summary>
        [TestMethod]
        public void Mask_MasksLastCharacters_WhenCalledWithDefaultMask()
        {
            string target   = "123-456-7890";
            string expected = "********7890";

            Assert.AreEqual(expected, target.Mask(4));
        }


        /// <summary>
        /// Test mask masks all when called with default visible
        /// </summary>
        [TestMethod]
        public void Mask_MasksLastCharacters_WhenCalledWithDefaultLength()
        {
            string target   = "123-456-7890";
            string expected = "XXXXXXXXXXXX";

            Assert.AreEqual(expected, target.Mask('X'));
        }


        /// <summary>
        /// test mask masks last chars with defaults
        /// </summary>
        [TestMethod]
        public void Mask_MasksLastCharacters_WhenCalledWithAllDefaults()
        {
            string target   = "123-456-7890";
            string expected = "************";

            Assert.AreEqual(expected, target.Mask());
        }


        /// <summary>
        /// test mask masks all when zero exposed
        /// </summary>
        [TestMethod]
        public void Mask_MasksAll_WhenCalledWithZeroExposed()
        {
            string target   = "123-456-7890";
            string expected = "************";

            Assert.AreEqual(expected, target.Mask(0));
        }


        /// <summary>
        /// test mask masks nothing when length is = to exposed length
        /// </summary>
        [TestMethod]
        public void Mask_MasksNothing_WhenCalledWithLengthEqualExposedLength()
        {
            string target = "123-456-7890";
            string expected = "123-456-7890";

            Assert.AreEqual(expected, target.Mask(12));
        }


        /// <summary>
        /// test mask masks nothing when called with exposed length greater than actual length
        /// </summary>
        [TestMethod]
        public void Mask_MasksNothing_WhenCalledWithExposedLengthGreaterThanLength()
        {
            string target = "123-456-7890";
            string expected = "123-456-7890";

            Assert.AreEqual(expected, target.Mask(15));
        }


        /// <summary>
        /// test mask throws argument exception when exposed length param is negative
        /// </summary>
        [TestMethod]
        public void Mask_ThrowsArgumentException_WhenExposedLengthNegative()
        {
            string target = "123-456-7890";

			AssertEx.Throws<ArgumentOutOfRangeException>(() =>
                target.Mask(-1));
        }


        /// <summary>
        /// Test resolve substitues all tokens when called with a valid dictionary
        /// </summary>
        [TestMethod]
        public void Resove_SubstitutesTokens_WhenCalledWithDictionary()
        {
            var tokens = new Dictionary<string, string>
                             {
                                 { "{FirstName}", "Agent" },
                                 { "{LastName}", "Smith" },
                                 { "{NotFound}", "Tada" }
                             };

            string target = "Hello, my name is {FirstName} {LastName}.  That's {FirstName} to " + 
                "you, but you can call me {NickName}, though I think you'll find that's not found.";

            string expected = "Hello, my name is Agent Smith.  That's Agent to " + 
                "you, but you can call me {NickName}, though I think you'll find that's not found.";

            Assert.AreEqual(expected, target.Resolve(tokens));
        }

        
        /// <summary>
        /// Test resolve substitues tokens when called with sorted list
        /// </summary>
        [TestMethod]
        public void Resove_SubstitutesTokens_WhenCalledWithSortedList()
        {
            var tokens = new SortedList<string, string>
                             {
                                 { "{FirstName}", "Agent" },
                                 { "{LastName}", "Smith" },
                                 { "{NotFound}", "Tada" }
                             };

            string target = "Hello, my name is {FirstName} {LastName}.  That's {FirstName} to " +
                "you, but you can call me {NickName}, though I think you'll find that's not found.";

            string expected = "Hello, my name is Agent Smith.  That's Agent to " +
                "you, but you can call me {NickName}, though I think you'll find that's not found.";

            Assert.AreEqual(expected, target.Resolve(tokens));
        }


		/// <summary>
		/// Tests the string NullSafeCount extension method.
		/// </summary>
		[TestMethod]
		public void NullSafeCount_ReturnsZero_ForNull()
		{
			string target = null;

			Assert.AreEqual(0, target.NullSafeCount());
		}
	
		
		/// <summary>
		/// Tests the string NullSafeCount extension method.
		/// </summary>
		[TestMethod]
		public void NullSafeCount_ReturnsZero_ForEmpty()
		{
			string target = string.Empty;

			Assert.AreEqual(0, target.NullSafeCount());
		}

		/// <summary>
		/// Tests the string NullSafeCount extension method.
		/// </summary>
		[TestMethod]
		public void NullSafeCount_ReturnsLength_ForNonEmpty()
		{
			string target = "12345";

			Assert.AreEqual(5, target.NullSafeCount());
		}


		/// <summary>
		/// Tests the string NullSafeCount extension method.
		/// </summary>
		[TestMethod]
		public void NullSafe_ReturnsEmpty_ForNull()
		{
			string target = null;

			Assert.AreSame(string.Empty, target.NullSafe());
		}

	
		/// <summary>
		/// Tests the string NullSafeCount extension method.
		/// </summary>
		[TestMethod]
		public void NullSafe_ReturnsSelf_ForNonNull()
		{
			string target = "12345";

			Assert.AreSame(target, target.NullSafe());
		}




		/// <summary>
		/// Tests that you can iterate over a NullSafe() on a null enumeration.
		/// </summary>
		[TestMethod]
		public void NullSafe_AllowsForEach_ForNull()
		{
			int count = 0;
			string target = null;

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
			string target = "12345";

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
			string target = null;

			foreach (var i in target)
			{
				count++;
			}
		}


		/// <summary>
		/// Make sure we can chain using NullSafe() safely.
		/// </summary>
		[TestMethod]
		public void NullSafe_WithTrim_ReturnsEmptyWithNull()
		{
			string target = null;

			Assert.AreSame(string.Empty, target.NullSafe().Trim());
		}


		/// <summary>
		/// Make sure we can chain using NullSafe() safely.
		/// </summary>
		[TestMethod]
		public void NullSafe_WithTrim_ReturnsTrimWithNonNull()
		{
			string target = "This is a test with trailing space.";
			string actual = target + "        ";

			Assert.AreEqual(target, actual.NullSafe().Trim());
			Assert.AreEqual(target, actual.Trim());
		}


		/// <summary>
		/// Tests the string NullSafeLength extension method.
		/// </summary>
		[TestMethod]
		public void NullSafeLength_ReturnsZero_ForNull()
		{
			string target = null;

			Assert.AreEqual(0, target.NullSafeLength());
		}


		/// <summary>
		/// Tests the string NullSafeLength extension method.
		/// </summary>
		[TestMethod]
		public void NullSafeLength_ReturnsZero_ForEmpty()
		{
			string target = string.Empty;

			Assert.AreEqual(0, target.NullSafeLength());
		}

		/// <summary>
		/// Tests the string NullSafeLength extension method.
		/// </summary>
		[TestMethod]
		public void NullSafeLength_ReturnsLength_ForNonEmpty()
		{
			string target = "12345";

			Assert.AreEqual(5, target.NullSafeLength());
		}


		/// <summary>
		/// Tests the string to see if IsNullOrEmpty applies.
		/// </summary>
		[TestMethod]
		public void IsNullOrEmpty_ReturnsCorrectValue_OnVarious()
		{
			string nullString = null;
			string emptyString = string.Empty;
			string testString = "Test";

			Assert.IsTrue(nullString.IsNullOrEmpty());
			Assert.IsTrue(emptyString.IsNullOrEmpty());
			Assert.IsFalse(testString.IsNullOrEmpty());
		}

	
		/// <summary>
		/// Tests the string to see if IsNotNullOrEmpty applies.
		/// </summary>
		[TestMethod]
		public void IsNotNullOrEmpty_ReturnsCorrectValue_OnVarious()
		{
			string nullString = null;
			string emptyString = string.Empty;
			string testString = "Test";

			Assert.IsFalse(nullString.IsNotNullOrEmpty());
			Assert.IsFalse(emptyString.IsNotNullOrEmpty());
			Assert.IsTrue(testString.IsNotNullOrEmpty());
		}


		/// <summary>
		/// Test mask masks the last character when called explicit
		/// </summary>
		[TestMethod]
		public void MaskAlphaNum_MasksLastCharacters_WhenCalledWithExplicit()
		{
			string target = "123-456-7890";
			string expected = "+++-+++-7890";

			Assert.AreEqual(expected, target.Mask('+', 4, MaskStyle.AlphaNumericOnly));
		}


		/// <summary>
		/// Test mask works whith default mask
		/// </summary>
		[TestMethod]
		public void MaskAlphaNum_MasksLastCharacters_WhenCalledWithDefaultMask()
		{
			string target = "123-456-7890";
			string expected = "***-***-7890";

			Assert.AreEqual(expected, target.Mask(4, MaskStyle.AlphaNumericOnly));
		}


		/// <summary>
		/// Test mask masks all when called with default visible
		/// </summary>
		[TestMethod]
		public void MaskAlphaNum_MasksLastCharacters_WhenCalledWithDefaultLength()
		{
			string target = "123-456-7890";
			string expected = "XXX-XXX-XXXX";

			Assert.AreEqual(expected, target.Mask('X', MaskStyle.AlphaNumericOnly));
		}


		/// <summary>
		/// test mask masks last chars with defaults
		/// </summary>
		[TestMethod]
		public void MaskAlphaNum_MasksLastCharacters_WhenCalledWithAllDefaults()
		{
			string target = "123-456-7890";
			string expected = "***-***-****";

			Assert.AreEqual(expected, target.Mask(MaskStyle.AlphaNumericOnly));
		}


		/// <summary>
		/// test mask masks all when zero exposed
		/// </summary>
		[TestMethod]
		public void MaskAlphaNum_MasksAll_WhenCalledWithZeroExposed()
		{
			string target = "123-456-7890";
			string expected = "***-***-****";

			Assert.AreEqual(expected, target.Mask(0, MaskStyle.AlphaNumericOnly));
		}


		/// <summary>
		/// test mask masks nothing when length is = to exposed length
		/// </summary>
		[TestMethod]
		public void MaskAlphaNum_MasksNothing_WhenCalledWithLengthEqualExposedLength()
		{
			string target = "123-456-7890";
			string expected = "123-456-7890";

			Assert.AreEqual(expected, target.Mask(12, MaskStyle.AlphaNumericOnly));
		}


		/// <summary>
		/// test mask masks nothing when called with exposed length greater than actual length
		/// </summary>
		[TestMethod]
		public void MaskAlphaNum_MasksNothing_WhenCalledWithExposedLengthGreaterThanLength()
		{
			string target = "123-456-7890";
			string expected = "123-456-7890";

			Assert.AreEqual(expected, target.Mask(15, MaskStyle.AlphaNumericOnly));
		}


		/// <summary>
		/// test mask throws argument exception when exposed length param is negative
		/// </summary>
		[TestMethod]
		public void MaskAlphaNum_ThrowsArgumentException_WhenExposedLengthNegative()
		{
			string target = "123-456-7890";

			AssertEx.Throws<ArgumentOutOfRangeException>(() =>
				target.Mask(-1));
		}

		/// <summary>
		/// A test for Truncate
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TruncateOnNullThrows()
		{
			string target = null;

			target.Truncate(30);
		}

		/// <summary>
		/// A test for Truncate
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void TruncateNegativeLengthThrows()
		{
			string target = "Hi there";

			target.Truncate(-30);
		}

		/// <summary>
		/// A test for Truncate
		/// </summary>
		[TestMethod]
		public void TruncateZeroLengthReturnsEllipsis()
		{
			string target = "Hi there";

			Assert.AreEqual("...", target.Truncate(0));
		}

		/// <summary>
		/// A test for Truncate
		/// </summary>
		[TestMethod]
		public void TruncateOnEmptyReturnsEmpty()
		{
			string target = string.Empty;

			Assert.AreEqual(target, target.Truncate(30));
		}

		/// <summary>
		/// A test for Truncate
		/// </summary>
		[TestMethod]
		public void TruncateOnSmallerStringReturnsSameString()
		{
			string target = "Small string";

			Assert.AreEqual(target, target.Truncate(30));
		}

		/// <summary>
		/// A test for Truncate
		/// </summary>
		[TestMethod]
		public void TruncateOnJustUnderStringReturnsSameString()
		{
			string target = "Small string";

			Assert.AreEqual(target, target.Truncate(target.Length + 1));
		}

		/// <summary>
		/// A test for Truncate
		/// </summary>
		[TestMethod]
		public void TruncateOnSameSizeStringReturnsSameString()
		{
			string target = "Small string";

			Assert.AreEqual(target, target.Truncate(target.Length));
		}

		/// <summary>
		/// A test for Truncate
		/// </summary>
		[TestMethod]
		public void TruncateOnLargetStringReturnsTruncatedString()
		{
			string target = "Small string";
			string actual = target.Truncate(5);

			Assert.AreNotEqual(target, target.Truncate(5));
			Assert.AreEqual("Small...", actual);
		}
	}
}
