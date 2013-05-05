
namespace SharedAssemblies.General.Caching.Provider
{
	/// <summary>
	/// A collection of options that control the behavior of the
	/// cache provider.
	/// </summary>
	public class ProviderOptions
	{
		/// <summary>
		/// A flag that indicates if values should be saved
		/// asynchrounously if possible.
		/// </summary>
		public bool Asynchronous { get; set; }

		/// <summary>
		/// The resource that we are possibly connecting to
		/// </summary>
		public string Resource { get; set; }

		/// <summary>
		/// The username (if needed) to connect to the given resource
		/// </summary>
		public string Username { get; set; }

		/// <summary>
		/// The password (if needed) to connect to the given resource
		/// </summary>
		public string Password { get; set; }
	}
}
