namespace SharedAssemblies.General.Threading
{
	/// <summary>
	/// Generic consumer interface for the consumer hierarchy of classes
	/// </summary>
	/// <typeparam name="T">The type of item to consume.</typeparam>
	public interface IConsumer<T>
	{
		/// <summary>
		/// Returns true if currently in a consuming mode.
		/// </summary>
		/// <returns>True if consuming</returns>
		bool IsConsuming { get; }

		/// <summary>
		/// Returns true if the consumer has already stopped.
		/// </summary>
		bool IsStopped { get; }

		/// <summary>
		/// Returns the total depth of the consumer collections
		/// </summary>
		int Depth { get; }
	
		/// <summary>
		/// Begins the consuming process, cannot be called if already consuming
		/// </summary>
		void Start();

		/// <summary>
		/// Begins the consuming process, cannot be called if already consuming
		/// </summary>
		/// <returns>True if stop is successful</returns>
		bool Stop();

		/// <summary>
		/// Adds an item to be consumed.
		/// </summary>
		/// <param name="item">The item to add to the collection to be consumed.</param>
		void Add(T item);
	}
}