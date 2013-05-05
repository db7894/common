using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using log4net;
using SharedAssemblies.Core.Extensions;

namespace SharedAssemblies.Security.Encryption.DataAccess
{
	/// <summary>
	/// Abstract base class representing a resource to retrieve
	/// bashwork encryption keys
	/// </summary>
	public abstract class AbstractKeyManager
	{
        /// <summary>
        /// Handle to this types logging instance
        /// </summary>
        private static readonly ILog _log =
            LogManager.GetLogger(typeof(AbstractKeyManager));

		/// <summary>
		/// The handle to the underlying keys
		/// </summary>
		private KeyContainer Container { get; set; }

		/// <summary>
		/// Perform any initialization needed to retrieve the keys
		/// </summary>
		/// <param name="database">The data access object used to retrieve the master key</param>
		/// <returns>true if successful, false otherwise</returns>
		[SuppressMessage("SharedAssemblies.Tools.StyleCopRules.SafetyAndPerformanceAnalyzer",
			"ST5005:NoEmptyCatchBlocks",
			Justification = "Initialization of the key manager failures should not bleed information.")]
		public bool Initialize(IEncryptionKeyDao database)
		{
			var result = false;

			try
			{
				var master = database.GetMasterKeyFromDatabase();
				var initial = InitializeKeys();

				Container = new KeyContainer
				{
					EncryptionKey = CombineSplitKeys(initial
						.Select(key => key.EncryptionKey), master),
					SigningKey = CombineSplitKeys(initial
						.Select(key => key.SigningKey), master),
				};

				// If we got two keys that are not null or empty (otherwise
				// they are 32 bytes each), then we succeeded.
				result = Container.EncryptionKey.IsNotNullOrEmpty()
						 && Container.SigningKey.IsNotNullOrEmpty();
			}

			catch (Exception exception)
			{
				// We do not want information about the keys to leak out
				// of this context, so we block all exceptions and simply 
				// return a failure instead.
                _log.Error("Exception getting Master Key", exception);
			}

			return result;
		}

		/// <summary>
		/// Retrieve the keys from the underlying resource
		/// </summary>
		/// <returns>The resulting encryption key</returns>
		public KeyContainer GetKey()
		{
			return Container;
		}

		/// <summary>
		/// Retrieve the keys from the underlying resource hashed with the
		/// supplied extra keys
		/// </summary>
		/// <param name="keys">The extra keys to hash the current keys with</param>
		/// <returns>The populated key container</returns>
		public KeyContainer GetKey(IEnumerable<IEnumerable<byte>> keys)
		{
			return new KeyContainer
			{
				EncryptionKey = CombineExtraKeys(Container.EncryptionKey, keys),
				SigningKey = CombineExtraKeys(Container.SigningKey, keys),
			};
		}
        
		/// <summary>
		/// Perform any initialization needed to retrieve the keys
		/// </summary>
		/// <returns>A collection of key components to create final key</returns>
		protected abstract IEnumerable<KeyContainer> InitializeKeys();
        
		/// <summary>
		/// Helper method to validate the resulting database keys.  In the current scheme
		/// we retrieve two sets of keys from two diffent sources (in this case two database
		/// tables). By having the relevant pieces in seperate locations, we make it harder
		/// to retrieve the final keys.
		/// </summary>
		/// <param name="keys">The collection of keys to combine</param>
		/// <param name="masterKey">The master decoding key</param>
		/// <returns>The validated keys</returns>
		private static byte[] CombineSplitKeys(IEnumerable<byte[]> keys, byte[] masterKey)
		{
			byte[] result = null;

			if (keys.All(key => key.IsNotNullOrEmpty()))
			{
				var combined = keys.Select(key =>
					EncryptionUtility.Decrypt(key, masterKey).AsEnumerable());
				result = CombineExtraKeys(null, combined);
			}

			return result;
		}

		/// <summary>
		/// Helper method to combine two byte streams together
		/// </summary>
		/// <param name="initial">The intial key to add keys to</param>
		/// <param name="keys">The collection of keys to add</param>
		/// <returns>The resulting combined and hashed key</returns>
		private static byte[] CombineExtraKeys(IEnumerable<byte> initial,
			IEnumerable<IEnumerable<byte>> keys)
		{
			byte[] accumulator = (initial != null) ? initial.ToArray() : null;
			var extraKey = keys.Aggregate((r, n) => r.Concat(n));

			if (extraKey != null)
			{
				accumulator = (initial != null)
					? initial.Concat(extraKey).ToArray()
					: extraKey.ToArray();
			}

			return HashUtility.ComputeHash(accumulator);
		}
	}
}