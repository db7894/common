using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.Security.Encryption;

namespace SharedAssemblies.Security.UnitTests.Encryption
{
	/// <summary>
	/// A collection of tests to test the functionality of the HexUtility.
	/// </summary>
	[TestClass]
	public class HexUtilityTest
	{
		/// <summary>
		/// Tests that we can convert a hex string to a binary hex array.
		/// </summary>
		[TestMethod]
		public void HexUtility_ConvertFromByteToString_Succeeds()
		{
			string input = "1234567890";
			byte[] expected = new byte[] { 0x12, 0x34, 0x56, 0x78, 0x90 };
			var actual = HexUtility.Convert(input);
			CollectionAssert.AreEqual(expected, actual);

			string input2 = "abCDefABCD";
			byte[] expected2 = new byte[] { 0xab, 0xcd, 0xef, 0xab, 0xcd };
			var actual2 = HexUtility.Convert(input2);
			CollectionAssert.AreEqual(expected2, actual2);

			string input3 = string.Empty;
			var actual3 = HexUtility.Convert(input3);
			Assert.IsNull(actual3);

			string input4 = null;
			var actual4 = HexUtility.Convert(input4);
			Assert.IsNull(actual4);
		}

		/// <summary>
		/// Tests that we can convert a binary hex array to a hex string.
		/// </summary>
		[TestMethod]
		public void HexUtility_ConvertFromStringToByte_Succeeds()
		{
			byte[] input = new byte[] { 0x12, 0x34, 0x56, 0x78, 0x90 };
			var expected = "1234567890";
			var actual = HexUtility.Convert(input);
			Assert.AreEqual(expected, actual);

			byte[] input2 = new byte[] { 0xab, 0xcd, 0xef, 0xab, 0xcd };
			string expected2 = "abcdefabcd";
			var actual2 = HexUtility.Convert(input2);
			Assert.AreEqual(expected2, actual2);

			byte[] input3 = new byte[] { };
			var actual3 = HexUtility.Convert(input3);
			Assert.IsNull(actual3);

			byte[] input4 = null;
			var actual4 = HexUtility.Convert(input4);
			Assert.IsNull(actual3);
		}
	}
}
