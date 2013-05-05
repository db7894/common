using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SharedAssemblies.General.Testing;
using SharedAssemblies.Security.Encryption;
using SharedAssemblies.Security.Encryption.DataAccess;


namespace SharedAssemblies.Security.UnitTests.Encryption
{
	/// <summary>
	/// Test fixture for SimpleEncryption
	/// </summary>
	[TestClass]
	public class SimpleEncryptionTest
	{
		/// <summary>
		/// Handle to the database mock to use for testing
		/// </summary>
		private static IEncryptionKeyDao _database;

		/// <summary>
		/// The master encryption key to use for testing
		/// </summary>
		private static readonly byte[] _masterEncryptionKey = new byte[]
		{
			0xb5, 0xd1, 0x60, 0xb0, 0xb8, 0xb8, 0x7e, 0xb9,
			0x42, 0xb3, 0x5e, 0x81, 0xdc, 0xb8, 0x0c, 0x89,
			0x5e, 0x0f, 0x80, 0x64, 0x79, 0x6e, 0x84, 0xe4,
			0x62, 0x03, 0x8c, 0x76, 0x12, 0x3f, 0x93, 0x49,
		};

		/// <summary>
		/// The key to use for encrypting data
		/// </summary>
		private static readonly byte[] _encryptionKey = new byte[]
		{
			0x73, 0x31, 0x69, 0x70, 0x73, 0x54, 0x40, 0x41,
			0x20, 0xed, 0x4f, 0x86, 0xfa, 0xf1, 0x2d, 0x5e,
			0x56, 0x81, 0x42, 0xf2, 0x50, 0xcb, 0xd5, 0xc9,
			0x7c, 0x3a, 0xff, 0x32, 0x2f, 0x59, 0x8f, 0x37,
		};

		/// <summary>
		/// The key to use for signging data
		/// </summary>
		private static readonly byte[] _signingKey = new byte[]
		{	
			0x1d, 0xd5, 0x6e, 0x28, 0x92, 0x87, 0xb0, 0x27,
			0x4c, 0x03, 0xce, 0x37, 0xca, 0xad, 0xb0, 0xdc,
			0xbf, 0xc3, 0x8c, 0x06, 0xd9, 0x17, 0xa5, 0xd5,
			0x3b, 0xd9, 0xd1, 0xe6, 0x57, 0xda, 0x23, 0x38,
		};

		#region Test Setup

		/// <summary>
		/// The class SetUp method
		/// </summary>
		[TestInitialize]
		public void MyTestSetup()
		{
			InitializeDatabase(true);
		}

		#endregion

		#region ISimpleEncryption.Initialize Tests

		/// <summary>
		/// This tests that initialization will fail with a faulty key
		/// manager.
		/// </summary>
		[TestMethod]
		public void SimpleEncryption_WithBadKeyManager_DoesNotInitialize()
		{
			var manager = new QuickKeyManager(null);
			var encryption = new SimpleEncryption(manager);
			var result = encryption.Initialize(_database);

			Assert.IsFalse(result);
			Assert.IsFalse(encryption.IsInitialized);
		}

		/// <summary>
		/// This tests that initialization will succeed with a correctly
		/// constructed key manager.
		/// </summary>
		[TestMethod]
		public void SimpleEncryption_WithGoodKeyManager_Initializes()
		{
			var container = new KeyContainer
			{
				EncryptionKey = _encryptionKey,
				SigningKey = _signingKey,
			};
			var manager = new QuickKeyManager(container);
			var encryption = new SimpleEncryption(manager);
			var result = encryption.Initialize(_database);

			Assert.IsTrue(result);
			Assert.IsTrue(encryption.IsInitialized);
		}

		/// <summary>
		/// This tests that an uninitialized key manager will not allow
		/// the simple encryption to ever work correctly (i.e. it will always
		/// return null);
		/// </summary>
		[TestMethod]
		public void SimpleEncryption_WithoutInitialization_DoesFunction()
		{
			var manager = new QuickKeyManager(null);
			var encryption = new SimpleEncryption(manager);
			var result = encryption.Initialize(_database);

			Assert.IsFalse(result);
			Assert.IsFalse(encryption.IsInitialized);

			var input = "Data To Encrypt";
			var encrypted = encryption.Encrypt(input);
			Assert.IsNull(encrypted);
			var decrypted = encryption.Decrypt(encrypted);
			Assert.IsNull(decrypted);

			var extra = new List<IEnumerable<byte>> { null };
			string input2 = null;
			var encrypted2 = encryption.Encrypt(input2, extra);
			Assert.IsNull(encrypted2);
			var decrypted2 = encryption.Decrypt(encrypted2, extra);
			Assert.IsNull(decrypted2);
		}

		#endregion

		#region Encryption and Decryption Tests

		/// <summary>
		/// This tests that simple encryption encrypts and decrypts
		/// successfully in a loop. When this was tested, we averaged
		/// 45 uS per encrypt/decrypt loop, so we should always be
		/// below that.
		/// </summary>
		[TestMethod]
		public void SimpleEncryption_EncryptionLoop_DoesntTakeTooLong()
		{
			var stopwatch = new System.Diagnostics.Stopwatch();
			var encryption = GenerateSimpleEncryption();
			const string input = "abcdefghijklmnopqrstuvwxyz0123456789";

			stopwatch.Start();
			for (int id = 0; id < 1000; ++id)
			{
				encryption.Decrypt(encryption.Encrypt(input));
			}
			stopwatch.Stop();
			AssertEx.LessThan(stopwatch.ElapsedMilliseconds, 50);
		}

		/// <summary>
		/// This tests that simple encryption encrypts and decrypts
		/// successfully in a loop.
		/// </summary>
		[TestMethod]
		public void SimpleEncryption_EncryptWithGoodData_Succeeds()
		{
			for (int id = 0; id < 10; ++id)
			{
				var input = string.Format("{0}: Data To Encrypt", id);
				var encryption = GenerateSimpleEncryption();
				var encrypted = encryption.Encrypt(input);
				Assert.IsNotNull(encrypted);
				var decrypted = encryption.Decrypt(encrypted);
				Assert.AreEqual(input, decrypted);
			}
		}

		/// <summary>
		/// This tests that simple encryption can handle faulty
		/// data for encryption and decryption without error.
		/// </summary>
		[TestMethod]
		public void SimpleEncryption_EncryptWithBadData_Fails()
		{
			var input = string.Empty;
			var encryption = GenerateSimpleEncryption();
			var encrypted = encryption.Encrypt(input);
			Assert.IsNull(encrypted);
			var decrypted = encryption.Decrypt(encrypted);
			Assert.IsNull(decrypted);

			string input2 = null;
			var encrypted2 = encryption.Encrypt(input2);
			Assert.IsNull(encrypted2);
			var decrypted2 = encryption.Decrypt(encrypted2);
			Assert.IsNull(decrypted2);
		}

		#endregion

		#region Encryption and Decryption Tests

		/// <summary>
		/// This tests that simple encryption encrypts and decrypts
		/// successfully in a loop to binary data.
		/// </summary>
		[TestMethod]
		public void SimpleEncryptionBytes_EncryptWithGoodData_Succeeds()
		{
			for (int id = 0; id < 10; ++id)
			{
				var input = string.Format("{0}: Data To Encrypt", id);
				var encryption = GenerateSimpleEncryption();
				var encrypted = encryption.EncryptToByte(input);
				Assert.IsNotNull(encrypted);
				var decrypted = encryption.Decrypt(encrypted);
				Assert.AreEqual(input, decrypted);
			}
		}

		/// <summary>
		/// This tests that simple encryption can handle faulty
		/// data for encryption and decryption without error for
		/// binary data.
		/// </summary>
		[TestMethod]
		public void SimpleEncryptionBytes_EncryptWithBadData_Fails()
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
		public void SimpleEncryption_ExtraEncryptWithGoodData_Succeeds()
		{
			var extra = new List<IEnumerable<byte>>
			{
				new List<byte> { 0x12, 0x34, 0x56, 0x78, },
				new byte[] { 0x90, 0xab, 0xcb, 0xef, }
			};

			for (int id = 0; id < 10; ++id)
			{
				var input = string.Format("{0}: Data To Encrypt", id);
				var encryption = GenerateSimpleEncryption();
				var encrypted = encryption.Encrypt(input, extra);
				Assert.IsNotNull(encrypted);
				var decrypted = encryption.Decrypt(encrypted, extra);
				Assert.AreEqual(input, decrypted);
			}
		}

		/// <summary>
		/// This tests that simple encryption can handle faulty
		/// data for encryption and decryption without error with
		/// extra keys.
		/// </summary>
		[TestMethod]
		public void SimpleEncryption_ExtraEncryptWithBadData_Fails()
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
		public void SimpleEncryption_ExtraEncryptWithBadKeys_Succeeds()
		{
			var extra = new List<IEnumerable<byte>> { null };

			var input = string.Format("{0}: Data To Encrypt", 22);
			var encryption = GenerateSimpleEncryption();
			var encrypted = encryption.Encrypt(input, extra);
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
		public void SimpleEncryption_ExtraEncryptWithBadDataAndKeys_Fails()
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
		public void SimpleEncryptionByte_ExtraEncryptWithGoodData_Succeeds()
		{
			var extra = new List<IEnumerable<byte>>
			{
				new List<byte> { 0x12, 0x34, 0x56, 0x78, },
				new byte[] { 0x90, 0xab, 0xcb, 0xef, }
			};

			for (int id = 0; id < 10; ++id)
			{
				var input = string.Format("{0}: Data To Encrypt", id);
				var encryption = GenerateSimpleEncryption();
				var encrypted = encryption.EncryptToByte(input, extra);
				Assert.IsNotNull(encrypted);
				var decrypted = encryption.Decrypt(encrypted, extra);
				Assert.AreEqual(input, decrypted);
			}
		}

		/// <summary>
		/// This tests that simple encryption can handle faulty
		/// data for encryption and decryption without error with
		/// extra keys.
		/// </summary>
		[TestMethod]
		public void SimpleEncryptionByte_ExtraEncryptWithBadData_Fails()
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
		public void SimpleEncryptionByte_ExtraEncryptWithBadKeys_Succeeds()
		{
			var extra = new List<IEnumerable<byte>> { null };

			var input = string.Format("{0}: Data To Encrypt", 22);
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
		public void SimpleEncryptionByte_ExtraEncryptWithBadDataAndKeys_Fails()
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
			var container = new KeyContainer
			{
				EncryptionKey = _encryptionKey,
				SigningKey = _signingKey,
			};
			var manager = new QuickKeyManager(container);
			var encryption = new SimpleEncryption(manager);
			encryption.Initialize(_database);

			return encryption;
		}
        
		/// <summary>
		/// Helper method to generate valid key containers using the
		/// given master key.
		/// </summary>
		/// <param name="count">The number of key containers to generate</param>
		/// <returns>A collection of key containers</returns>
		private static IEnumerable<KeyContainer> GenerateKeyContainer(int count)
		{
			return Enumerable.Range(0, count).Select(id =>
				new KeyContainer
				{
					EncryptionKey = _encryptionKey,
					SigningKey = _signingKey,
				}).ToList();
		}

        /// <summary>
        /// Initialize the database
        /// </summary>
        /// <param name="populate">True if should populate</param>
		private static void InitializeDatabase(bool populate)
		{
			var mock = new Mock<IEncryptionKeyDao>();

			mock.Setup(x => x.GetMasterKeyFromDatabase())
				.Returns(_masterEncryptionKey);

			mock.Setup(x => x.GetEncryptionKeysFromDatabase(It.IsAny<IEnumerable<int>>()))
				.Returns<IEnumerable<KeyContainer>>(list => GenerateKeyContainer(list.Count()));

			_database = mock.Object;
		}

		#endregion
	}
}
