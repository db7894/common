using System;
using System.Collections;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.Security.Encryption;


namespace SharedAssemblies.Security.UnitTests.Encryption
{
	/// <summary>
	/// test fixture for authentication encryption
	/// </summary>
	[TestClass]
	public class AuthenticatedEncryptionTest
	{
		#region Constant Test Data

		/// <summary>encryption tests</summary>
		private const string _encryptionKeyBase64String =
			"XwhkiMchRaJzQe3b49cnMHBoWSc1ElUKRtmjToCfN2A=";

        /// <summary>encryption tests</summary>
        private const string _encryptionKeyHexString =
			"5F086488C72145A27341EDDBE3D72730706859273512550A46D9A34E809F3760";

        /// <summary>encryption tests</summary>
        private const string _validationKeyHexString =
			"F199DA38A6200DA4362B425E1EAC6D3507923F8496691456353B5B0DB7CC6E1E";

        /// <summary>encryption tests</summary>
        private const string _dataToEncryptString =
			"Order#: 123456, CustID: 987654, Part#: C27-42B, Price: $18.95, Qty: 20, BackOrder: false";

        /// <summary>encryption tests</summary>
		private const string _authenticatedEncryptedDataBase64String =
			"7ClgK5OaMRn82O8vUaBJH9vpIufZf6VpvU0loXrrb1jMzAhLiSBFoKqscFKXTLV3/4QnVlO1MFHVXPz+oa4W+"
			+ "uLYHBtbLAPahXrM1DSM2VsA+61El0lHdJ8aiCBTpau6yFo9xMeHFTcy216EBZiK4n+vLsg=";

        /// <summary>encryption tests</summary>
        private const string _validationKeyBase64String =
			"8ZnaOKYgDaQ2K0JeHqxtNQeSP4SWaRRWNTtbDbfMbh4=";

        /// <summary>encryption tests</summary>
        private const string _authenticationCodeBase64String =
			"O4cc7RlFb4v9AeDYEYD/U9D+2uM=";

		#endregion

		/// <summary>
		/// Validate authentication code with all parameters as null
		/// </summary>
		[TestMethod]
		public void ValidateAuthenticationCodeWithAllParametersAsNull()
		{
			bool returnValue = AuthenticatedEncryption.ValidateAuthenticationCode(
				(string)null, null, null);
			Assert.IsFalse(returnValue);
		}

		/// <summary>
		/// Validate authentication code with data as null
		/// </summary>
		[TestMethod]
		public void ValidateAuthenticationCodeWithDataAsNull()
		{
			bool returnValue = AuthenticatedEncryption.ValidateAuthenticationCode(
				null, _authenticationCodeBase64String, _validationKeyBase64String);
			Assert.IsFalse(returnValue);
		}

		/// <summary>
		/// validate authentication code with HMA as null
		/// </summary>
		[TestMethod]
		public void ValidateAuthenticationCodeWithHmacAsNull()
		{
			bool returnValue = AuthenticatedEncryption.ValidateAuthenticationCode(
				_dataToEncryptString, null, _validationKeyBase64String);
			Assert.IsFalse(returnValue);
		}

		/// <summary>
		/// Validate authentication code with validation key as null
		/// </summary>
		[TestMethod]
		public void ValidateAuthenticationCodeWithValidationKeyAsNull()
		{
			bool returnValue = AuthenticatedEncryption.ValidateAuthenticationCode(
				_dataToEncryptString, _authenticationCodeBase64String, null);
			Assert.IsFalse(returnValue);
		}

		/// <summary>
		/// Validate authentication code successfully
		/// </summary>
		[TestMethod]
		public void ValidateAuthenticationCodeSuccessfully()
		{
			bool returnValue = AuthenticatedEncryption.ValidateAuthenticationCode(
				_dataToEncryptString, _authenticationCodeBase64String, _validationKeyBase64String);

			Assert.IsTrue(returnValue);
		}


		/// <summary>
		/// Encrypt then append authentication code
		/// </summary>
		[TestMethod]
		public void EncryptThenAppendAuthenticationCodeWithAllParametersAsNull()
		{
			byte[] authenticatedEncryptedData = AuthenticatedEncryption.EncryptThenAppendAuthenticationCode(
				(byte[])null, null, null);
			Assert.IsNull(authenticatedEncryptedData);
		}

		/// <summary>
		/// Encrypt then append authentication code
		/// </summary>
		[TestMethod]
		public void EncryptThenAppendAuthenticationCodeWithDataAsNull()
		{
			byte[] validationKey = SoapHexBinary.Parse(_validationKeyHexString).Value;
			byte[] encryptionKey = SoapHexBinary.Parse(_encryptionKeyHexString).Value;
			byte[] authenticatedEncryptedData = AuthenticatedEncryption.EncryptThenAppendAuthenticationCode(
				null, encryptionKey, validationKey);
			Assert.IsNull(authenticatedEncryptedData);
		}

		/// <summary>
		/// Encrypt then append authentication code
		/// </summary>
		[TestMethod]
		public void EncryptThenAppendAuthenticationCodeWithEncryptionKeyAsNull()
		{
			string authenticatedEncryptedData =
				AuthenticatedEncryption.EncryptThenAppendAuthenticationCode(
					_dataToEncryptString, null, _validationKeyBase64String);
			Assert.IsNull(authenticatedEncryptedData);
		}

		/// <summary>
		/// Encrypt then append authentication code
		/// </summary>
		[TestMethod]
		public void EncryptThenAppendAuthenticationCodeWithValidationKeyAsNull()
		{
			string authenticatedEncryptedData = 
				AuthenticatedEncryption.EncryptThenAppendAuthenticationCode(
					_dataToEncryptString, _encryptionKeyBase64String, null);
			Assert.IsNull(authenticatedEncryptedData);
		}

		/// <summary>
		/// Encrypt then append authentication code
		/// </summary>
		[TestMethod]
		public void EncryptThenAppendAuthenticationCodeSuccessfully()
		{
			string authenticatedEncryptedData = 
				AuthenticatedEncryption.EncryptThenAppendAuthenticationCode(
					_dataToEncryptString, _encryptionKeyBase64String, _validationKeyBase64String);
			Assert.IsNotNull(authenticatedEncryptedData);
			Assert.IsTrue(authenticatedEncryptedData.Length != 0);
			Assert.AreEqual(_authenticatedEncryptedDataBase64String, authenticatedEncryptedData);
		}


		/// <summary>
		/// Validate authentication code then decrypt
		/// </summary>
		[TestMethod]
		public void ValidateAuthenticationCodeThenDecryptWithAllParametersAsNull()
		{
			byte[] data = null;
			byte[] encryptKey = null;
			byte[] validationKey = null;
			byte[] returnData = AuthenticatedEncryption.ValidateAuthenticationCodeThenDecrypt(
				data, encryptKey, validationKey);
			Assert.IsNull(returnData);
		}

		/// <summary>
		/// validate authentication code then decrypt
		/// </summary>
		[TestMethod]
		public void ValidateAuthenticationCodeThenDecryptWithDataAsNull()
		{
			byte[] encryptionKey = SoapHexBinary.Parse(_encryptionKeyHexString).Value;
			byte[] validationKey = SoapHexBinary.Parse(_validationKeyHexString).Value;
			byte[] returnData = AuthenticatedEncryption.ValidateAuthenticationCodeThenDecrypt(
				null, encryptionKey, validationKey);
			Assert.IsNull(returnData);
		}

		/// <summary>
		/// validate authentication code then decrypt
		/// </summary>
		[TestMethod]
		public void ValidateAuthenticationCodeThenDecryptWithEncryptionKeyAsNull()
		{
			byte[] validationKey = SoapHexBinary.Parse(_validationKeyHexString).Value;
			byte[] authenticatedEncryptedData = Convert.FromBase64String(
				_authenticatedEncryptedDataBase64String);
			byte[] returnData = AuthenticatedEncryption.ValidateAuthenticationCodeThenDecrypt(
				authenticatedEncryptedData, null, validationKey);
			Assert.IsNull(returnData);
		}

		/// <summary>
		/// validate authentication code then decrypt
		/// </summary>
		[TestMethod]
		public void ValidateAuthenticationCodeThenDecryptWithValidationKeyAsNull()
		{
			byte[] encryptionKey = SoapHexBinary.Parse(_encryptionKeyHexString).Value;
			byte[] authenticatedEncryptedData = Convert.FromBase64String(
				_authenticatedEncryptedDataBase64String);
			byte[] returnData = AuthenticatedEncryption.ValidateAuthenticationCodeThenDecrypt(
				authenticatedEncryptedData, encryptionKey, null);
			Assert.IsNull(returnData);
		}

		/// <summary>
		/// validate authentication code then decrypt
		/// </summary>
		[TestMethod]
		public void ValidateAuthenticationCodeThenDecryptSuccessfully()
		{
			string returnData = AuthenticatedEncryption.ValidateAuthenticationCodeThenDecrypt(
				_authenticatedEncryptedDataBase64String, _encryptionKeyBase64String, 
				_validationKeyBase64String);

			Assert.IsNotNull(returnData);
			Assert.IsTrue(returnData.Length != 0);
			Assert.AreEqual(_dataToEncryptString, returnData);
		}

		/// <summary>
		/// Sign and encrypt data set bit randomly
		/// </summary>
		[TestMethod]
		public void SignAndEncryptDataSetBitRandomlyDecryptAndFailValidation()
		{
			byte[] encryptionKey = SoapHexBinary.Parse(_encryptionKeyHexString).Value;
			byte[] validationKey = SoapHexBinary.Parse(_validationKeyHexString).Value;

			for (int byteChangeCount = 1; byteChangeCount < 50; byteChangeCount++)
			{
				BitArray ba = new BitArray(Convert.FromBase64String(_authenticatedEncryptedDataBase64String));
				int rn = EncryptionUtility.GenerateRandomInteger() % ba.Count;
				ba[rn] = !ba[rn];
				byte[] newEncryptedData = new byte[ba.Count / 8];
				ba.CopyTo(newEncryptedData, 0);
				byte[] returnData = AuthenticatedEncryption.ValidateAuthenticationCodeThenDecrypt(
					newEncryptedData, encryptionKey, validationKey);
				Assert.IsNull(returnData);
			}
		}
	}
}
