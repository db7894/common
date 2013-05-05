
namespace SharedAssemblies.General.Caching
{
    /// <summary>
    /// Interface for cache item expiration strategy.
    /// </summary>
    /// <typeparam name="TItem">The type of item in the cache.</typeparam>
    public interface IExpirationStrategy<TItem>
    {
        /// <summary>
        /// Evaluates the cached item to determine if it is expired or not.
        /// </summary>
        /// <param name="item">The item to evaluate for expiration.</param>
        /// <returns>True if expired, false otherwise.</returns>
        bool IsExpired(CachedValue<TItem> item);
    }
}
