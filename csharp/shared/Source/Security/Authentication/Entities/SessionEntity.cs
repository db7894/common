using System;

namespace SharedAssemblies.Security.Authentication.Entities
{
    /// <summary>
    /// Represents a single logon session
    /// </summary>
    public class SessionEntity
    {
        /// <summary>
        /// Represents the type of session the user is using
        /// </summary>
        public SessionType Type { get; set; }

        /// <summary>
        /// Unique identifier for the given vender / application
        /// </summary>
        public string VenderIdentifier { get; set; }

        /// <summary>
        /// Unique identifier for the given user
        /// </summary>
        public string LoginIdentifier { get; set; }

        /// <summary>
        /// The ip-address of the machine that last attmepted to login
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// The session value
        /// </summary>
        public string SessionValue { get; set; }

        /// <summary>
        /// Should session value be encrypted
        /// </summary>
        public bool ShouldEncrypt { get; set; }

		/// <summary>
		/// The token
		/// </summary>
		public string Token { get; set; }

		/// <summary>
		/// The time this session expires
		/// </summary>
		public DateTime ExpireTime { get; set; }

		/// <summary>
		/// If the token is expired
		/// </summary>
		public bool IsExpired { get; set; }

    }
}