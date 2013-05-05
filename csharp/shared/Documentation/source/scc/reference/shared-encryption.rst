============================================================
Security --- Low Level Encryption Helpers
============================================================
:Author: Galen Collins <gcollins@bashwork.com>
:Assembly: SharedAssemblies.Security
:Namespace: SharedAssemblies.Security.Encryption
:Date: |today|

.. module:: SharedAssemblies.Security.Encryption
   :synopsis: Encryption - Low Level Encryption
   :platform: Windows, .Net

.. highlight:: csharp

Introduction
------------------------------------------------------------

The security library is a set of class collections that help with common security tasks such as authentication, encryption, and
nonces (one-use tokens).

Class Library
------------------------------------------------------------
*What follows is a listing of the available classes in the SA.S.Encryption namespace*

.. class:: HexUtility

   This is a simple utility to convert to and from byte arrays and
   hex strings.

   **Example**

   The HexUtility exposes two methods that can be seen below::

       /// This will convert a byte array to a packed hex string
       string hexString = HexUtility.Convert(new byte [] { 0x01, 0x02, 0x03, 0x04 });
       
       /// This will convert the hex string back into a byte array
       byte [] hexArray = HexUtility.Convert(hexString);

.. class:: HashUtility

   This is a simple utility to generate a unique hash for a
   given type of string.

   **Example**

   The HashUtility exposes two methods overloaded a few ways::

       /// This will compute the hash of a byte array
	   byte [] input = new byte [] { 0x01, 0x02, 0x03, 0x04 };
       byte [] hash = HashUtility.ComputeHash(input);

       /// This will compute the hash of a string
	   string password = "password";
       byte [] hash = HashUtility.ComputeHash(password);

       /// This will compute the hash of some serializable class
	   SomeSerializeableType type = new SomeSerializeableType();
       byte [] hash = HashUtility.ComputeHash(type);

       /// This will compute the hash of some nullable struct
	   DateTime? date = null;
       byte [] hash = HashUtility.ComputeStructHash(date);

       /// This will compute the hash of some struct
	   DateTime date = DateTime.Now();
       byte [] hash = HashUtility.ComputeStructHash(date);
       

.. class:: EncryptionUtility

   This is basically a facade around the low level encryption routines in .Net.
   It makes sure that the best encryption practices are followed while exposing
   a collection of convenience routines.

   The library also supports a collection of true random generators formed using
   .Net's RNG provider (using entropy from the system). This assures that the
   randomly generated material is not subject to prediction attacks. 

   **Examples**

   The following is an example of the random generators supplied by the library::

       int numberOfBytes = 16;

       // The result is a random byte array
       byte [] randomBytes = GenerateRandomByteToken(numberOfBytes);

       // The result is a random base64 string
       string randomString = GenerateRandomStringToken(numberOfBytes);

       // The result is a random integer value
       int randomInt = GenerateRandomInteger();

   The following is an example of generating authentication codes for given data::

       byte [] validationKey = HashUtility.ComputeHash(password) 
       byte [] byteData = Encoding.Default.GetBytes(data); 

       var result = GenerateAuthenciationCode(byteData, validationKey);

.. class:: AuthenticationEncryption

   This is a facade around creating very strong encrypted components. It does this
   by first encrypting the input data and then signing it to assure the data has
   not been tampered with.  Although this may be overkill for simply storing data
   in a database, when dealing with external parties it is recommended practice.

   **Example**

   What follows is an example encryption session using this library::

       // create a encrypted token
       string encryptedText = EncryptThenAppendAuthenticationCode(inputData,
           encryptionKey, validationKey);

       //
       // Send data to third party for usage / Wait for data to return
       //

       // validate token was not altered, then decrypt
       string decryptedText = ValidateAuthenticationCodeThenDecrypt(encryptedText,
           encryptionKey, validationKey);

.. class:: SimpleEncryption

   This is a facade around Bashwork's encryption scheme. What follows is a brief
   discussion of the steps taken in this process:

   1. Retrieve the master key for decryption (which should be secured)
   2. Retrieve a number of encrypted key components
   3. Select N of these (predetermined and N should be greater than 30)
   4. Decrypt those components, join them together, and hash them
   5. Use this key as the encryption and decryption key

   In order to make this easier, the simple encryption utility exposes a
   number of key container sources (database, file, registry, etc) as well
   as a mocked encryption scheme that can be plugged in while testing.

   **Example**

   What follows is an example of using the simple encryption utility::

       // first build your data access
       var connection = Configuration["EncryptionConnection"];
       var database = new SqlEncryptionKeyDao(ClientProviderType.SqlServer, connection);

       // Then create your key manager provider
       var identifiers = new List<int> { 12, 27, 349 };
       var manager = new SqlKeyManager(database, identifiers);

       // Then construct and initialize the simple encryption manager
       var encryption = new SimpleEncryption(manager);
       if (encryption.Initialize(database))
       {
		   // The default encrypt returns a base64 string for easy use
           var encrypted = encryption.Encrypt("Successul Encryption");
           var decrypted = encryption.Decrypt(encrypted);
           Console.WriteLine(encrypted);
           Console.WriteLine(decrypted);
       }

	   // You can encrypt and decrypt to byte arrays for performance
	   var encrypted = encryption.EncryptToByte("password");
	   var decrypted = encryption.Decrypt(encrypted);

	   // You can also add extra keys to the encrypt and decrypt process
	   var extraKeys = new byte [] { 0x00, 0x01, 0x02 };
	   var extraEncrypted = encryption.Encrypt("password", extraKeys);
	   var decryptEncrypted = encryption.Decrypt(extraEncrypted, extraKeys);

.. class:: INonceDao

   This is a simple utility that allows one to securely store some data
   that needs to be passed to another party at some later time and then
   be immediately deleted. This can be used to pass some session state to
   another web process, pass encrypted data, etc.

   **Example**

   What follows is an example of using the nonce utility::

       // Initialize the datastore
       var venderId = Configuration["VendorId"];
       var connection = Configuration["NonceConnection"];
	   var nonce = new SqlNonceDao(ClientProviderType.SqlServer, connection, vendorId);

	   // Then store your data
	   var secret = "Some Secret Data";
	   var key1 = nonce.Store(secret);

	   // You can also store serializable data
	   var serializeable = new SomeSerializeableType();
	   var key2 = nonce.Store(serializeable);

	   // Then the other party just needs that unique key to retrieve
	   var secret1 = nonce.Retrieve(key1);

	   // Although They will need to know the type to deserialize
	   var secret2 = nonce.Retrieve<SomeSerializeableType>(key2);

Further Reading
------------------------------------------------------------

*What follows is a collection of material to further your knowledge of encryption*

`NIST Publications <http://csrc.nist.gov/publications/PubsSPs.html>`_

`Guidelines for Implementing Cryptography in the Federal Government <http://csrc.nist.gov/publications/nistpubs/800-21-1/sp800-21-1_Dec2005.pdf>`_

`Recommendations for Key Management <http://csrc.nist.gov/publications/nistpubs/800-57/sp800-57-Part1-revised2_Mar08-2007.pdf>`_

For more information, see the `API Reference <../../../../Api/index.html>`_.
