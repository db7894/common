namespace SharedAssemblies.General.Threading
{
	/// <summary>
	/// Generic consumer interface for the consumer hierarchy of classes
	/// </summary>
	public interface IConsumer
	{
		/// <summary>
		/// Returns true if there is a bucket attached to the consumer
		/// </summary>
		/// <returns>True if attached to a bucket</returns>
		bool IsAttached { get; }

		/// <summary>
		/// Returns true if currently in a consuming mode.
		/// </summary>
		/// <returns>True if consuming</returns>
		bool IsConsuming { get; }

		/// <summary>
		/// Begins the consuming process, cannot be called if already consuming
		/// </summary>
		/// <returns>True if start is successful</returns>
		bool Start();

		/// <summary>
		/// Begins the consuming process, cannot be called if already consuming
		/// </summary>
		/// <returns>True if stop is successful</returns>
		bool Stop();
	}
}