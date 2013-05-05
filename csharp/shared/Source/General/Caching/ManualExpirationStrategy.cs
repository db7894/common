namespace SharedAssemblies.General.Caching
{
	/// <summary>
	/// Items in the cache never automatically expire with this strategy.
	/// </summary>
	/// <typeparam name="TCacheItem">Type of item stored in cache.</typeparam>
	public sealed class ManualExpirationStrategy<TCacheItem> : CacheExpirationStrategy<TCacheItem>
		where TCacheItem : class
	{
		/// <summary>
		/// Creates a manual expiration strategy where 
		/// </summary>
		public ManualExpirationStrategy() : base(item => false)
		{
		}
	}
}
