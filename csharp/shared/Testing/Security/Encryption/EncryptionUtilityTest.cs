using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.Core.Extensions;
using SharedAssemblies.Security.Encryption;


namespace SharedAssemblies.Security.UnitTests.Encryption
{
	/// <summary>
	/// test fixture for encryption
	/// </summary>
	[TestClass]
	public class EncryptionUtilityTest
	{
		#region Constant Test Data

		/// <summary>test keys for encyptions</summary>
		private const string _encryptionKeyBase64String =
			"XwhkiMchRaJzQe3b49cnMHBoWSc1ElUKRtmjToCfN2A=";

		/// <summary>test keys for encyptions</summary>
		private const string _encryptionKeyHexString =
			"5F086488C72145A27341EDDBE3D72730706859273512550A46D9A34E809F3760";

		/// <summary>test keys for encyptions</summary>
		private const string _validationKeyHexString =
			"F199DA38A6200DA4362B425E1EAC6D3507923F8496691456353B5B0DB7CC6E1E";

		/// <summary>test keys for encyptions</summary>
		private const string _dataToEncryptString =
			"Order#: 123456, CustID: 987654, Part#: C27-42B, Price: $18.95, Qty: 20, BackOrder: false";

		/// <summary>test keys for encyptions</summary>
		private const string _encryptedDataBase64String =
			"7ClgK5OaMRn82O8vUaBJH9vpIufZf6VpvU0loXrrb1jMzAhLiSBFoKqscFKXTLV3/4QnVlO1MFHVXPz+oa4W+"
			+ "uLYHBtbLAPahXrM1DSM2VsA+61El0lHdJ8aiCBTpau6";

		/// <summary>test keys for encyptions</summary>
		private const string _validationKeyBase64String =
			"8ZnaOKYgDaQ2K0JeHqxtNQeSP4SWaRRWNTtbDbfMbh4=";

		/// <summary>test keys for encyptions</summary>
		private const string _authenticationCodeBase64String =
			"O4cc7RlFb4v9AeDYEYD/U9D+2uM=";

		#endregion

		/// <summary>
		/// Gets or sets the test context which provides
		/// information about and functionality for the current test run.
		/// </summary>
		public TestContext TestContext { get; set; }

		#region GenerateRandomNumber tests

		/// <summary>
		/// random numbers with negative parameters
		/// </summary>
		[TestMethod]
		public void GenerateRandomNumberWithNegativeParameter()
		{
			byte[] returnData = EncryptionUtility.GenerateRandomByteToken(-1);
			Assert.IsNull(returnData);
		}

		/// <summary>
		/// rnadom numbers for 256 bit key
		/// </summary>
		[TestMethod]
		public void GenerateRandomNumberFor256BitKey()
		{
			byte[] returnData = EncryptionUtility.GenerateRandomByteToken(32);
			Assert.IsNotNull(returnData);
			Assert.AreEqual(32, returnData.Length);
		}

		/// <summary>
		/// random numbers for 8192 key
		/// </summary>
		[TestMethod]
		public void GenerateRandomNumberFor8192BitKey()
		{
			byte[] returnData = EncryptionUtility.GenerateRandomByteToken(1024);
			Assert.IsNotNull(returnData);
			Assert.AreEqual(1024, returnData.Length);
		}

		#endregion

		#region GenerateRandom Extra Method Tests

		/// <summary>
		/// random numbers with negative parameters
		/// </summary>
		[TestMethod]
		public void GenerateRandomStringToken_WithBadParameters_Fails()
		{
			Assert.IsNull(EncryptionUtility.GenerateRandomStringToken(-1));
			Assert.IsNull(EncryptionUtility.GenerateRandomStringToken(0));
		}

		/// <summary>
		/// rnadom numbers for 256 bit key
		/// </summary>
		[TestMethod]
		public void GenerateRandomStringToken_WithGoodParameters_Succeeds()
		{
			const int size = 32;
			var result = EncryptionUtility.GenerateRandomStringToken(size);
			Assert.IsNotNull(result);
			Assert.AreEqual(size, result.Length);
		}

		/// <summary>
		/// rnadom numbers for 256 bit key
		/// </summary>
		[TestMethod]
		public void GenerateRandomStringToken_IsWebSafe()
		{
			var result = EncryptionUtility.GenerateRandomStringToken(32);
			Assert.IsNotNull(result);
			Assert.IsFalse(result.Contains('+'));
			Assert.IsFalse(result.Contains('='));
			Assert.IsFalse(result.Contains('/'));
		}

		/// <summary>
		/// random numbers for 8192 key
		/// </summary>
		[TestMethod]
		public void GenerateRandomInteger_AlwaysSucceeds()
		{
			var result1 = EncryptionUtility.GenerateRandomInteger();
			var result2 = EncryptionUtility.GenerateRandomInteger();
			var result3 = EncryptionUtility.GenerateRandomInteger();

			CollectionAssert.AllItemsAreUnique(new[] { result1, result2, result3 });
		}

		#endregion

		#region Encrypt tests

		/// <summary>
		/// encrypt data with all parameters
		/// </summary>
		[TestMethod]
		public void EncryptDataWithAllParametersAsNull()
		{
			byte[] returnData = EncryptionUtility.Encrypt((byte[]) null, null);
			Assert.IsNull(returnData);
		}

		/// <summary>
		/// enrypt data with key as null
		/// </summary>
		[TestMethod]
		public void EncryptDataWithEncryptionKeyAsNull()
		{
			string returnData = EncryptionUtility.Encrypt(_dataToEncryptString, null);
			Assert.IsNull(returnData);
		}

		/// <summary>
		/// encrypt data with data as null
		/// </summary>
		[TestMethod]
		public void EncryptDataWithDataAsNull()
		{
			byte[] encryptionKey = HexUtility.Convert(_encryptionKeyHexString);
			byte[] returnData = EncryptionUtility.Encrypt(null, encryptionKey);
			Assert.IsNull(returnData);
		}

		/// <summary>
		/// encrypt data successfully
		/// </summary>
		[TestMethod]
		public void EncryptDataSuccessfully()
		{
			byte[] encryptionKey = HexUtility.Convert(_encryptionKeyHexString);
			byte[] inputData = Encoding.UTF8.GetBytes(_dataToEncryptString);
			byte[] returnData = EncryptionUtility.Encrypt(inputData, encryptionKey);
			Assert.IsNotNull(returnData);
			Assert.IsTrue(returnData.Length != 0);
			string returnDataBase64String = Convert.ToBase64String(returnData);
			Assert.AreEqual(_encryptedDataBase64String, returnDataBase64String);
		}

		#endregion

		#region Decrypt tests

		/// <summary>
		/// Decrypt data with all parameters as null
		/// </summary>
		[TestMethod]
		public void DecryptDatawithAllParametersAsNull()
		{
			string returnData = EncryptionUtility.Decrypt((string) null, null);
			Assert.IsNull(returnData);
		}

		/// <summary>
		/// Decrypt data with encryption key as null
		/// </summary>
		[TestMethod]
		public void DecryptDataWithEncryptionKeyAsNull()
		{
			string returnData = EncryptionUtility.Decrypt(_encryptedDataBase64String, null);
			Assert.IsNull(returnData);
		}

		/// <summary>
		/// Decrypt data with data as null
		/// </summary>
		[TestMethod]
		public void DecryptDataWithDataAsNull()
		{
			byte[] encryptionKey = HexUtility.Convert(_encryptionKeyHexString);
			byte[] returnData = EncryptionUtility.Decrypt(null, encryptionKey);
			Assert.IsNull(returnData);
		}

		/// <summary>
		/// Decrypt data successfully
		/// </summary>
		[TestMethod]
		public void DecryptDataSuccessfully()
		{
			string returnData = EncryptionUtility.Decrypt(_encryptedDataBase64String,
			                                              _encryptionKeyBase64String);
			Assert.IsNotNull(returnData);
			Assert.IsTrue(returnData.Length != 0);
			Assert.AreEqual(_dataToEncryptString, returnData);
		}

		#endregion

		#region GenerateAuthenticationCode tests

		/// <summary>
		/// Generate authentication code
		/// </summary>
		[TestMethod]
		public void GenerateAuthenticationCodeWithAllParametersAsNull()
		{
			byte[] authenticationCode = EncryptionUtility
				.GenerateAuthenticationCode((byte[]) null, null);
			Assert.IsNull(authenticationCode);
		}

		/// <summary>
		/// Generate authentication code
		/// </summary>
		[TestMethod]
		public void GenerateAuthenticationCodeWithValidationKeyAsNull()
		{
			string authenticationCode = EncryptionUtility
				.GenerateAuthenticationCode(_dataToEncryptString, null);
			Assert.IsNull(authenticationCode);
		}

		/// <summary>
		/// Generate authentication code
		/// </summary>
		[TestMethod]
		public void GenerateAuthenticationCodeWithDataAsNull()
		{
			string authenticationCode = EncryptionUtility
				.GenerateAuthenticationCode(null, _validationKeyHexString);
			Assert.IsNull(authenticationCode);
		}

		/// <summary>
		/// Generate authentication code
		/// </summary>
		[TestMethod]
		public void GenerateAuthenticationCodeSuccessfully()
		{
			string authenticationCode = EncryptionUtility
				.GenerateAuthenticationCode(_dataToEncryptString, _validationKeyBase64String);
			Assert.IsNotNull(authenticationCode);
			Assert.IsTrue(authenticationCode.Length != 0);
			Assert.AreEqual(_authenticationCodeBase64String, authenticationCode);
		}

		/// <summary>
		/// Generate authentication code
		/// </summary>
		[SuppressMessage("SharedAssemblies.Tools.StyleCopRules.ReadabilityAnalyzer",
			"ST2002:MethodCannotExceedMaxLines", Justification = "Unit Test - Ok.")]
		[TestMethod]
		public void GenerateAuthenticationCodeRfc2202TestCases()
		{
			// These are test cases that can be found at: http://www.faqs.org/rfcs/rfc2202.html
			bool returnValue = Rfc2202TestCase(
				"0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b",
				"Hi There", "b617318655057264e28bc0b6fb378c8ef146be00");
			Assert.IsTrue(returnValue);

			SoapHexBinary shb = new SoapHexBinary(new UTF8Encoding(false).GetBytes("Jefe"));
			returnValue = Rfc2202TestCase(shb.ToString(),
			                              "what do ya want for nothing?",
			                              "effcdf6ae5eb2fa2d27416d5f184df9c259a7c79");
			Assert.IsTrue(returnValue);

			byte[] dataBytes = new byte[50];
			for (int i = 0; i < dataBytes.Length; i++)
			{
				dataBytes[i] = 0xdd;
			}
			returnValue = Rfc2202TestCase(
				"aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
				dataBytes, "125d7342b9ac11cd91a39af48aa17b4f63f175d3");
			Assert.IsTrue(returnValue);

			dataBytes = new byte[50];
			for (int i = 0; i < dataBytes.Length; i++)
			{
				dataBytes[i] = 0xcd;
			}
			returnValue = Rfc2202TestCase(
				"0102030405060708090a0b0c0d0e0f10111213141516171819",
				dataBytes, "4c9007f4026250c6bc8414f9bf50c86c2d7235da");
			Assert.IsTrue(returnValue);

			returnValue = Rfc2202TestCase(
				"0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c",
				"Test With Truncation", "4c1a03424b55e07fe7f27be1d58bb9324a9a5a04");
			Assert.IsTrue(returnValue);

			byte[] keyBytes = new byte[80];
			for (int i = 0; i < keyBytes.Length; i++)
			{
				keyBytes[i] = 0xaa;
			}
			shb = new SoapHexBinary(keyBytes);
			returnValue = Rfc2202TestCase(shb.ToString(),
			                              "Test Using Larger Than Block-Size Key - Hash Key First",
			                              "aa4ae5e15272d00e95705637ce8a3b55ed402112");
			Assert.IsTrue(returnValue);

			returnValue = Rfc2202TestCase(shb.ToString(),
                  "Test Using Larger Than Block-Size Key and Larger Than One Block-Size Data",
                  "e8e99d0f45237d786d6bbaa7965c7808bbff1a91");

			Assert.IsTrue(returnValue);
		}

		#endregion

		#region Helper methods and classes

		/// <summary>
		/// test Rfc2202
		/// </summary>
		/// <param name="keyHexString">key hex string</param>
		/// <param name="data">data to encrypt</param>
		/// <param name="digestHexString">digest string</param>
		/// <returns>true or false</returns>
		private static bool Rfc2202TestCase(string keyHexString, string data, string digestHexString)
		{
			bool result = false;

			if (!string.IsNullOrEmpty(keyHexString) && !string.IsNullOrEmpty(data)
			    && !string.IsNullOrEmpty(digestHexString))
			{
				byte[] input = Encoding.UTF8.GetBytes(data);
				byte[] key = SoapHexBinary.Parse(keyHexString).Value;
				byte[] digest = SoapHexBinary.Parse(digestHexString).Value;
				byte[] authenticationCode = EncryptionUtility.GenerateAuthenticationCode(input, key);
				result = digest.SequenceEqual(authenticationCode);
			}

			return result;
		}

		/// <summary>
		/// test Rfc2202
		/// </summary>
		/// <param name="keyHexString">key hex string</param>
		/// <param name="data">data to encrypt</param>
		/// <param name="digestHexString">digest string</param>
		/// <returns>true or false</returns>
		private static bool Rfc2202TestCase(string keyHexString, byte[] data, string digestHexString)
		{
			bool result = false;

			if (!string.IsNullOrEmpty(keyHexString) && !data.IsNullOrEmpty()
			    && !string.IsNullOrEmpty(digestHexString))
			{
				byte[] key = SoapHexBinary.Parse(keyHexString).Value;
				byte[] digest = SoapHexBinary.Parse(digestHexString).Value;
				byte[] authenticationCode = EncryptionUtility.GenerateAuthenticationCode(data, key);
				result = digest.SequenceEqual(authenticationCode);
			}

			return result;
		}

		#endregion
	}
}