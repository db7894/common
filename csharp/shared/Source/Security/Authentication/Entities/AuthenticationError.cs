
namespace SharedAssemblies.Security.Authentication.Entities
{
	/// <summary>
	/// Values representing the different types of Errors while authenticating
	/// </summary>
	public enum AuthenticationError
	{
		/// <summary>
		/// Token not provided
		/// </summary>
		MissingToken,

		/// <summary>
		/// VendorId not provided
		/// </summary>
		MissingVendorId,

		/// <summary>
		/// Session is not valid
		/// </summary>
		InvalidSession,

		/// <summary>
		/// Invalid account for sesssion provided
		/// </summary>
		InvalidAccount,

		/// <summary>
		/// Invalid VendorId
		/// </summary>
		InvalidVendorId,

		/// <summary>
		/// Authentication Failure for any reason other than ones provided
		/// </summary>
		Other
	}
}
