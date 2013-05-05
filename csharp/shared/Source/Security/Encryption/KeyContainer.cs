
namespace SharedAssemblies.Security.Encryption
{
    /// <summary>
    /// Used to manage all the keys used during an encryption process
    /// </summary>
    public class KeyContainer
    {
        /// <summary>
        /// The key used to encrypt secure data
        /// </summary>
        public byte[] EncryptionKey { get; set; }

        /// <summary>
        /// The key used to sign the encrypted secure data
        /// </summary>
        public byte[] SigningKey { get; set; }
    }
}