using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.Security.Encryption;


namespace SharedAssemblies.Security.UnitTests.Encryption
{
	/// <summary>
	/// test fixture class
	/// </summary>
	[TestClass]
	public class WebConfigEncryptionTest
	{
        /// <summary>encryption key string</summary>
		private readonly string _encryptionKeyHexString =
			"5F086488C72145A27341EDDBE3D72730706859273512550A46D9A34E809F3760";

        /// <summary>Validation key string</summary>
		private readonly string _validationKeyHexString =
			"F199DA38A6200DA4362B425E1EAC6D3507923F8496691456353B5B0DB7CC6E1E";


		/// <summary>
		/// Gets or sets the test context which provides
		/// information about and functionality for the current test run.
		/// </summary>
		public TestContext TestContext { get; set; }

		#region GetMachineKey tests

		/// <summary>
		/// Get machine key decryption key
		/// </summary>
		[TestMethod]
		public void GetMachineKeyDecryptionKey()
		{
			byte[] decryptionKey = WebConfigMachineKey.GetMachineKeyDecryptionKey();
			Assert.IsNotNull(decryptionKey);
			byte[] expectedDecryptionKey = SoapHexBinary.Parse(_encryptionKeyHexString).Value;
			bool returnValue = expectedDecryptionKey.SequenceEqual(decryptionKey);
			Assert.IsTrue(returnValue);
		}

		/// <summary>
		/// Get machine key validation key
		/// </summary>
		[TestMethod]
		public void GetMachineKeyValidationKey()
		{
			byte[] validationKey = WebConfigMachineKey.GetMachineKeyValidationKey();
			Assert.IsNotNull(validationKey);

			byte[] expectedValidationKey = SoapHexBinary.Parse(_validationKeyHexString).Value;
			bool returnValue = expectedValidationKey.SequenceEqual(validationKey);
			Assert.IsTrue(returnValue);
		}

		#endregion
	}
}
