
namespace SharedAssemblies.Security.Authentication.Entities
{
    /// <summary>
    /// Represnts the types of sessions provided by the library
    /// </summary>
    public enum SessionType
    {
        /// <summary>
        /// Represents an authenticated session that will last for a given time
        /// limit unless refreshed. The authentication source can be used over
        /// and over again.
        /// </summary>
        Session = 1,

        /// <summary>
        /// Represents an authenticated session that will last only as long as
        /// it takes for a the full authentication to be completed. The authentication
        /// source can be used only once.
        /// </summary>
        SingleSignOn = 3,

        /// <summary>
        /// Represents an streaming session that will last for a given time unless refreshed.
        /// </summary>
        StreamingSession = 2,
    }
}