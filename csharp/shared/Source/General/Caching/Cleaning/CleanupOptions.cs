using System;

namespace SharedAssemblies.General.Caching.Cleaning
{
    /// <summary>
    /// The options to control how the janitor functions
    /// </summary>
	public sealed class CleanupOptions
    {
		/// <summary>
		/// The default options used by the system if none are provided
		/// </summary>
		public static readonly CleanupOptions Default = new CleanupOptions
		{
			Frequency = TimeSpan.FromDays(1),
		};

		/// <summary>
		/// How often should the janitor clean up happen
		/// </summary>
		public TimeSpan Frequency { get; set; }
	}
}
