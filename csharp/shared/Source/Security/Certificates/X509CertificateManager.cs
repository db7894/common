using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;

namespace SharedAssemblies.Security.Certificates
{
    /// <summary>
    /// Contains utility methods for working with certificates.
    /// </summary>
    public static class X509CertificateManager
    {
        /// <summary>
        /// Attempts to retrieve the certificates from the certificate store.
        /// Returns null if no certificates were found.
        /// If multiple certificates are found that match the key, the first in the list is returned.
        /// </summary>
        /// <param name="storeName">The store name to search for the certificate in.</param>
        /// <param name="location">The store location to search for the certificate.</param>
        /// <param name="key">The value to search for.</param>
        /// <param name="findBy">The certificate field to sea</param>
        /// <returns>The certificate, or null if not found.</returns>
        public static X509Certificate2Collection GetCertificates(StoreName storeName,
			StoreLocation location, object key, X509FindType findBy)
        {
            X509Store store = new X509Store(storeName, location);
            store.Open(OpenFlags.ReadOnly);

            return store.Certificates.Find(findBy, key, true);
        }

        /// <summary>
        /// Returns all certificates in the specified store.
        /// </summary>
        /// <param name="storeName">The store to get the certificates from</param>
        /// <param name="location">The store location to get the certificates from</param>
        /// <returns>The certificates in the specified store.</returns>
        public static X509Certificate2Collection GetCertificates(StoreName storeName,
			StoreLocation location)
        {
            X509Store store = new X509Store(storeName, location);
            store.Open(OpenFlags.ReadOnly);

            return store.Certificates;
        }

        /// <summary>
        /// Attempts to retrieve the certificate from the certificate store.
        /// Returns null if the certificate was not found.
        /// If multiple certificates are found that match the key, the first in the list is returned.
        /// </summary>
        /// <param name="storeName">The store name to search for the certificate in.</param>
        /// <param name="location">The store location to search for the certificate.</param>
        /// <param name="key">The value to search for.</param>
        /// <param name="findBy">The certificate field to sea</param>
        /// <returns>The certificate, or null if not found.</returns>
        public static X509Certificate2 GetCertificate(StoreName storeName, StoreLocation location,
			object key, X509FindType findBy)
        {
            X509Certificate2 certificate = null;

            var certificates = GetCertificates(storeName, location, key, findBy);

            if ((certificates != null) && (certificates.Count > 0))
            {
				certificate = certificates[0];
            }

            return certificate;
        }

        /// <summary>
        /// Attempts to retrieve the certificate from the specified .CER file.
        /// Returns null if the certificate was not found.
        /// </summary>
        /// <param name="cerFilename">The .CER file to open.</param>
        /// <returns>The certificate, or null if not found.</returns>
        public static X509Certificate2 GetCertificate(string cerFilename)
        {
            return new X509Certificate2(X509Certificate.CreateFromCertFile(cerFilename));
        }

        /// <summary>
        /// Attempts to retrieve the certificates from the .PFX file.
        /// Returns null if no certificates were found.
        /// </summary>
        /// <param name="pfxFilename">The .PFX file to load.</param>
        /// <param name="password">The password used to protect the .PFX file.</param>
        /// <returns>The certificates.</returns>
        public static X509Certificate2Collection GetCertificates(string pfxFilename, string password)
        {
            var certificates = new X509Certificate2Collection();
			certificates.Import(pfxFilename, password, X509KeyStorageFlags.DefaultKeySet);

			return certificates;
        }

        /// <summary>
        /// Attempts to retrieve the specified certificates from the .PFX file.
        /// Returns null if no certificates were found.
        /// </summary>
        /// <param name="pfxFilename">The .PFX file to load.</param>
        /// <param name="password">The password used to protect the .PFX file.</param>
        /// <param name="key">The value to search for.</param>
        /// <param name="findBy">The certificate field to search for</param>
        /// <returns>The certificates.</returns>
        public static X509CertificateCollection GetCertificates(string pfxFilename, string password,
			object key, X509FindType findBy)
        {
            X509Certificate2Collection store = GetCertificates(pfxFilename, password);

            return store.Find(findBy, key, true);
        }

        /// <summary>
        /// Attempts to retrieve the specified certificate from the .PFX file.
        /// Returns null if no certificate was found.
        /// If multiple certificates are found that match the key, the first in the list is returned.
        /// </summary>
        /// <param name="pfxFilename">The .PFX file to load.</param>
        /// <param name="password">The password used to protect the .PFX file.</param>
        /// <param name="key">The value to search for.</param>
        /// <param name="findBy">The certificate field to sea</param>
        /// <returns>The certificate, or null if not found.</returns>
        public static X509Certificate GetCertificate(string pfxFilename, string password,
			object key, X509FindType findBy)
        {
            X509Certificate certificate = null;

            var certificates = GetCertificates(pfxFilename, password, key, findBy);
            if ((certificates != null) && (certificates.Count > 0))
            {
				certificate = certificates[0];
            }

            return certificate;
        }


        /// <summary>
        /// Turns a certificate field (comma seperated key = value pairs) into a dictionary.
        /// Multiple values with the same key will have duplicate entries.
        /// </summary>
        /// <param name="field">The field to parse.</param>
        /// <returns>A dictionary of the key value pairs</returns>
        public static List<KeyValuePair<string, string>> CertFieldToList(string field)
        {
            // Parse the key/values out
            List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();

            List<string> pairs = BreakIntoKeyValueStrings(field);

            foreach (string pair in pairs)
            {
                int loc = pair.IndexOf('=');
                string key = pair.Substring(0, loc);
                string value = pair.Substring(loc + 1, pair.Length - loc - 1);

                if (value.StartsWith("\"") && value.EndsWith("\""))
                {
                    value = value.Substring(1, value.Length - 2);
                    value = value.Replace("\"\"", "\"");
                }

                list.Add(new KeyValuePair<string, string>(key, value));
            }

            return list;
        }

        /// <summary>
        /// Turns a certificate field (comma seperated key = value pairs) into a dictionary.
        /// Multiple values with the same key will be overwritten.
        /// </summary>
        /// <param name="field">The field to parse.</param>
        /// <returns>A dictionary of the key value pairs</returns>
        public static Dictionary<string, string> CertFieldToDictionary(string field)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            List<string> pairs = BreakIntoKeyValueStrings(field);

            foreach (string pair in pairs)
            {
                int loc = pair.IndexOf('=');
                string key = pair.Substring(0, loc);
                string value = pair.Substring(loc + 1, pair.Length - loc - 1);

                if (value.StartsWith("\"") && value.EndsWith("\""))
                {
                    value = value.Substring(1, value.Length - 2);
                    value = value.Replace("\"\"", "\"");
                }

                dict[key] = value;
            }

            return dict;
        }

        /// <summary>
        /// Installs the certificate into the specified store.
        /// </summary>
        /// <param name="certificate">The certificate to install.</param>
        /// <param name="storeName">The <see cref="StoreName"/> to install the certificate into.</param>
        /// <param name="location">The <see cref="StoreLocation"/> to install the certificate into.</param>
        public static void Install(X509Certificate certificate, StoreName storeName, StoreLocation location)
        {
            X509Store store = new X509Store(storeName, location);
            Install(certificate, store);
        }

        /// <summary>
        /// Installs the certificate into the specified store.
        /// </summary>
        /// <param name="certificate">The certificate to install.</param>
        /// <param name="store">The <see cref="X509Store"/> to install the certificate into.</param>
        public static void Install(X509Certificate certificate, X509Store store)
        {
            if (certificate != null)
            {
                X509Certificate2 cert2 = new X509Certificate2(certificate);
                Install(cert2, store);
            }
            else
            {
                throw new ArgumentNullException("certificate");
            }
        }

        /// <summary>
        /// Installs the certificate into the specified store.
        /// </summary>
        /// <param name="certificate">The certificate to install.</param>
        /// <param name="storeName">The <see cref="StoreName"/> to install the certificate into.</param>
        /// <param name="location">The <see cref="StoreLocation"/> to install the certificate into.</param>
        public static void Install(X509Certificate2 certificate, StoreName storeName, StoreLocation location)
        {
            X509Store store = new X509Store(storeName, location);
            Install(certificate, store);
        }

        /// <summary>
        /// Installs the certificate into the specified store.
        /// </summary>
        /// <param name="certificate">The certificate to install.</param>
        /// <param name="store">The <see cref="X509Store"/> to install the certificate into.</param>
        public static void Install(X509Certificate2 certificate, X509Store store)
        {
            store.Open(OpenFlags.ReadWrite);
            store.Add(certificate);
        }

        /// <summary>
        /// Encrypt input data using the specified certificate. 
        /// </summary>
        /// <param name="cert">x509 certificate</param>
        /// <param name="input">Data to encrypt</param>
        /// <returns>Encrypted data</returns>
        public static byte[] Encrypt(X509Certificate2 cert, byte[] input)
        {
            if (cert == null || input == null)
            {
                throw new ArgumentNullException((cert == null) ? "cert" : "input");
            }
            if (input.Length == 0)
            {
                throw new ArgumentException("input length is zero.");
            }
            if (!cert.HasPrivateKey)
            {
                throw new CryptographicException("Private key not contained within certificate.");
            }

            RSACryptoServiceProvider rsaPublic = (RSACryptoServiceProvider)cert.PublicKey.Key;

            if (rsaPublic == null)
            {
                throw new CryptographicException("Public key not contained within certificate.");
            }

            return rsaPublic.Encrypt(input, true);
        }

        /// <summary>
        /// Decrypt data using the specified certificate. 
        /// </summary>
        /// <param name="cert">x509 certificate</param>
        /// <param name="encryptedInput">Data to decrypt</param>
        /// <returns>Unencrypted data</returns>
        public static byte[] Decrypt(X509Certificate2 cert, byte[] encryptedInput)
        {
            if (cert == null || encryptedInput == null)
            {
                throw new ArgumentNullException((cert == null) ? "cert" : "encryptedInput");
            }
            if (encryptedInput.Length == 0)
            {
                throw new ArgumentException("encryptedInput length is zero.");
            }
            if (!cert.HasPrivateKey)
            {
                throw new CryptographicException("Private key not contained within certificate.");
            }

            RSACryptoServiceProvider rsaPrivate = (RSACryptoServiceProvider)cert.PrivateKey;
            return rsaPrivate.Decrypt(encryptedInput, true);
        }

        /// <summary>
        /// Break the string into key value strings
        /// </summary>
        /// <param name="field">The string to be parsed</param>
        /// <returns>A list of parsed strings</returns>
		/// <TODO>The format of this should be documented</TODO>
        private static List<string> BreakIntoKeyValueStrings(string field)
        {
            List<string> pairs = new List<string>();

            int pos = 0;
            int start = 0;
            bool inQuotes = false;
            while (pos < field.Length)
            {
                if (field[pos] == '"')
                {
                    if (inQuotes)
                    {
                        // Check to see if there are two quotes in a row
                        if (pos < field.Length - 1 && field[pos + 1] == '"')
                        {
                            pos++;
                        }
                        else
                        {
                            inQuotes = false;
                        }
                    }
                    else
                    {
                        inQuotes = true;
                    }
                }

                if (!inQuotes && field[pos] == ',')
                {
                    pairs.Add(field.Substring(start, pos - start).Trim());
                    start = pos + 1;
                }
                else if (!inQuotes && pos == field.Length - 1)
                {
                    pairs.Add(field.Substring(start, pos - start + 1).Trim());
                }
                pos++;
            }

            return pairs;
        }
    }
}