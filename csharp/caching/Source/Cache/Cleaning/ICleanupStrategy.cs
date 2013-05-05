using System;
using System.Threading.Tasks;

namespace SharedAssemblies.General.Caching.Cleaning
{
	/// <summary>
	/// Interface to a janitor which is used to clean a cache.
	/// </summary>
	public interface ICleanupStrategy
	{
		/// <summary>
		/// The options controlling how this janitor works
		/// </summary>
		CleanupOptions Options { get; set; }

		/// <summary>
		/// Performs the cleanup for the supplied janitor
		/// </summary>
		/// <returns>The result of the operation</returns>
		bool PerformCleanup();
	}
}
