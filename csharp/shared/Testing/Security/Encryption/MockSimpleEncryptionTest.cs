using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.Security.Encryption;
using SharedAssemblies.Security.Encryption.DataAccess;


namespace SharedAssemblies.Security.UnitTests.Encryption
{
	/// <summary>
	/// Test fixture for SimpleEncryption
	/// </summary>
	[TestClass]
	public class MockSimpleEncryptionTest
	{
		/// <summary>
		/// Handle to the database mock to use for testing
		/// </summary>
		private static IEncryptionKeyDao _database = null;

		#region ISimpleEncryption.Initialize Tests

		/// <summary>
		/// This tests that initialization will fail with a faulty key
		/// manager.
		/// </summary>
		[TestMethod]
		public void MockSimpleEncryption_WithAnyKeyManager_Initializes()
		{
			var encryption = new MockSimpleEncryption();
			var result = encryption.Initialize(_database);

			Assert.IsTrue(result);
			Assert.IsTrue(encryption.IsInitialized);
		}

		#endregion

		#region Encryption and Decryption Tests

		/// <summary>
		/// This tests that simple encryption encrypts and decrypts
		/// successfully in a loop.
		/// </summary>
		[TestMethod]
		public void MockSimpleEncryption_Encrypt_ReturnsSameData()
		{
			string input = "some test data";
			var encryption = GenerateSimpleEncryption();
			var encrypted = encryption.Encrypt(input);
			Assert.IsNotNull(encrypted);
			Assert.AreEqual(input, encrypted);
			var decrypted = encryption.Decrypt(encrypted);
			Assert.AreEqual(input, decrypted);
		}

		#endregion

		#region Encryption and Decryption Tests

		/// <summary>
		/// This tests that simple encryption encrypts and decrypts
		/// successfully in a loop to binary data.
		/// </summary>
		[TestMethod]
		public void MockSimpleEncryptionBytes_Encrypt_ReturnsSameData()
		{
			string input = "some test data";
			var encryption = GenerateSimpleEncryption();
			var encrypted = encryption.EncryptToByte(input);
			Assert.IsNotNull(encrypted);
			var decrypted = encryption.Decrypt(encrypted);
			Assert.AreEqual(input, decrypted);
		}

		/// <summary>
		/// This tests that simple encryption can handle faulty
		/// data for encryption and decryption without error for
		/// binary data.
		/// </summary>
		[TestMethod]
		public void MockSimpleEncryptionBytes_EncryptWithBadData_Fails()
		{
			string input = null;
			var encryption = GenerateSimpleEncryption();
			var encrypted = encryption.EncryptToByte(input);
			Assert.IsNull(encrypted);
			var decrypted = encryption.Decrypt(encrypted);
			Assert.IsNull(decrypted);

			string input2 = string.Empty;
			var encrypted2 = encryption.EncryptToByte(input2);
			Assert.IsNull(encrypted2);
			var decrypted2 = encryption.Decrypt(encrypted2);
			Assert.IsNull(decrypted2);
		}

		#endregion

		#region ExtraEncryption and ExtraDecryption Tests

		/// <summary>
		/// This tests that simple encryption encrypts and decrypts
		/// successfully in a loop with extra keys.
		/// </summary>
		[TestMethod]
		public void MockSimpleEncryption_ExtraEncrypt_ReturnsSameData()
		{
			var extra = new List<IEnumerable<byte>>
			{
				new List<byte> { 0x12, 0x34, 0x56, 0x78, },
				new byte[] { 0x90, 0xab, 0xcb, 0xef, }
			};

			string input = "some test data";
			var encryption = GenerateSimpleEncryption();
			var encrypted = encryption.Encrypt(input, extra);
			Assert.IsNotNull(encrypted);
			Assert.AreEqual(input, encrypted);
			var decrypted = encryption.Decrypt(encrypted, extra);
			Assert.AreEqual(input, decrypted);
		}

		/// <summary>
		/// This tests that simple encryption can handle faulty
		/// data for encryption and decryption without error with
		/// extra keys.
		/// </summary>
		[TestMethod]
		public void MockSimpleEncryption_ExtraEncryptWithBadData_Fails()
		{
			var extra = new List<IEnumerable<byte>>
			{
				new List<byte> { 0x12, 0x34, 0x56, 0x78, },
				new byte[] { 0x90, 0xab, 0xcb, 0xef, }
			};

			var input = string.Empty;
			var encryption = GenerateSimpleEncryption();
			var encrypted = encryption.Encrypt(input, extra);
			Assert.IsNull(encrypted);
			var decrypted = encryption.Decrypt(encrypted, extra);
			Assert.IsNull(decrypted);

			string input2 = null;
			var encrypted2 = encryption.Encrypt(input2, extra);
			Assert.IsNull(encrypted2);
			var decrypted2 = encryption.Decrypt(encrypted2, extra);
			Assert.IsNull(decrypted2);
		}

		/// <summary>
		/// This tests that simple encryption encrypts and decrypts
		/// successfully with extra keys that may be faulty.
		/// </summary>
		[TestMethod]
		public void MockSimpleEncryption_ExtraEncryptWithBadKeys_ReturnsSameData()
		{
			var extra = new List<IEnumerable<byte>> { null };

			string input = "some test data";
			var encryption = GenerateSimpleEncryption();
			var encrypted = encryption.Encrypt(input, extra);
			Assert.IsNotNull(encrypted);
			Assert.AreEqual(input, encrypted);
			var decrypted = encryption.Decrypt(encrypted, extra);
			Assert.AreEqual(input, decrypted);
		}

		/// <summary>
		/// This tests that simple encryption can handle faulty
		/// data for encryption and decryption without error with
		/// extra keys that are faulty.
		/// </summary>
		[TestMethod]
		public void MockSimpleEncryption_ExtraEncryptWithBadDataAndKeys_Fails()
		{
			var extra = new List<IEnumerable<byte>> { null };

			var input = string.Empty;
			var encryption = GenerateSimpleEncryption();
			var encrypted = encryption.Encrypt(input, extra);
			Assert.IsNull(encrypted);
			var decrypted = encryption.Decrypt(encrypted, extra);
			Assert.IsNull(decrypted);

			string input2 = null;
			var encrypted2 = encryption.Encrypt(input2, extra);
			Assert.IsNull(encrypted2);
			var decrypted2 = encryption.Decrypt(encrypted2, extra);
			Assert.IsNull(decrypted2);
		}

		#endregion

		#region ExtraEncryption and ExtraDecryption With Binary Tests

		/// <summary>
		/// This tests that simple encryption encrypts and decrypts
		/// successfully in a loop with extra keys.
		/// </summary>
		[TestMethod]
		public void MockSimpleEncryptionByte_ExtraEncrypt_ReturnsSameData()
		{
			var extra = new List<IEnumerable<byte>>
			{
				new List<byte> { 0x12, 0x34, 0x56, 0x78, },
				new byte[] { 0x90, 0xab, 0xcb, 0xef, }
			};

			string input = "some test data";
			var encryption = GenerateSimpleEncryption();
			var encrypted = encryption.EncryptToByte(input, extra);
			Assert.IsNotNull(encrypted);
			var decrypted = encryption.Decrypt(encrypted, extra);
			Assert.AreEqual(input, decrypted);
		}

		/// <summary>
		/// This tests that simple encryption can handle faulty
		/// data for encryption and decryption without error with
		/// extra keys.
		/// </summary>
		[TestMethod]
		public void MockSimpleEncryptionByte_ExtraEncryptWithBadData_Fails()
		{
			var extra = new List<IEnumerable<byte>>
			{
				new List<byte> { 0x12, 0x34, 0x56, 0x78, },
				new byte[] { 0x90, 0xab, 0xcb, 0xef, }
			};

			var input = string.Empty;
			var encryption = GenerateSimpleEncryption();
			var encrypted = encryption.EncryptToByte(input, extra);
			Assert.IsNull(encrypted);
			var decrypted = encryption.Decrypt(encrypted, extra);
			Assert.IsNull(decrypted);

			string input2 = null;
			var encrypted2 = encryption.EncryptToByte(input2, extra);
			Assert.IsNull(encrypted2);
			var decrypted2 = encryption.Decrypt(encrypted2, extra);
			Assert.IsNull(decrypted2);
		}

		/// <summary>
		/// This tests that simple encryption encrypts and decrypts
		/// successfully with extra keys that may be faulty.
		/// </summary>
		[TestMethod]
		public void MockSimpleEncryptionByte_ExtraEncryptWithBadKeys_ReturnsSameData()
		{
			var extra = new List<IEnumerable<byte>> { null };

			string input = "some test data";
			var encryption = GenerateSimpleEncryption();
			var encrypted = encryption.EncryptToByte(input, extra);
			Assert.IsNotNull(encrypted);
			var decrypted = encryption.Decrypt(encrypted, extra);
			Assert.AreEqual(input, decrypted);
		}

		/// <summary>
		/// This tests that simple encryption can handle faulty
		/// data for encryption and decryption without error with
		/// extra keys that are faulty.
		/// </summary>
		[TestMethod]
		public void MockSimpleEncryptionByte_ExtraEncryptWithBadDataAndKeys_Fails()
		{
			var extra = new List<IEnumerable<byte>> { null };

			var input = string.Empty;
			var encryption = GenerateSimpleEncryption();
			var encrypted = encryption.EncryptToByte(input, extra);
			Assert.IsNull(encrypted);
			var decrypted = encryption.Decrypt(encrypted, extra);
			Assert.IsNull(decrypted);

			string input2 = null;
			var encrypted2 = encryption.EncryptToByte(input2, extra);
			Assert.IsNull(encrypted2);
			var decrypted2 = encryption.Decrypt(encrypted2, extra);
			Assert.IsNull(decrypted2);
		}

		#endregion

		#region Private Helper Factories

		/// <summary>
		/// Helper method to generate an initialized simple encryption
		/// populated with a key manaber.
		/// </summary>
		/// <returns>The initialized simple encryption handler</returns>
		private static ISimpleEncryption GenerateSimpleEncryption()
		{
			var encryption = new MockSimpleEncryption();
			encryption.Initialize(_database);

			return encryption;
		}

		#endregion
	}
}
