using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;

namespace SharedAssemblies.General.Caching.Cleaning
{
	/// <summary>
	/// The main janitor used to periodically clean all the caches.
	/// </summary>
	public sealed class CacheJanitor
	{
		private readonly Task _janitor;
		private TimeSpan SleepTime = TimeSpan.MaxValue;
		private static Lazy<CacheJanitor> _instance = new Lazy<CacheJanitor>(() => new CacheJanitor());
		private readonly ConcurrentDictionary<int, ICleanupStrategy> _registration;
		private readonly IDictionary<int, TimeSpan> _timeouts = new Dictionary<int, TimeSpan>();

		/// <summary>
		/// Get the singleton instance of the janitor
		/// </summary>
		public static CacheJanitor Instance { get { return _instance.Value; } }
		private CacheJanitor()
		{
			_janitor = new Task(CleanupTask, TaskCreationOptions.LongRunning);
			_registration = new ConcurrentDictionary<int, ICleanupStrategy>();
		}

		/// <summary>
		/// Register a new cache to be cleaned with the specified strategy
		/// </summary>
		/// <typeparam name="Key">The key type of the cache</typeparam>
		/// <typeparam name="Value">The value type of the cache</typeparam>
		/// <param name="cache">The actual cache to key off of</param>
		/// <param name="strategy">The strategy to use for this cache</param>
		public bool Register<Key, Value>(ICache<Key, Value> cache, ICleanupStrategy strategy)
		{
			_registration.AddOrUpdate(cache.GetHashCode(), strategy, (k, v) => strategy);
			SleepTime = (strategy.Options.Frequency.CompareTo(SleepTime) < 0)
				? strategy.Options.Frequency // faster expiration time
				: SleepTime;

			if (_janitor.Status == TaskStatus.Created)
			{	// start once we have some work to do
				_janitor.Start();
			}

			return true;
		}

		/// <summary>
		/// Unregister an existing cache to be cleaned with the specified strategy
		/// </summary>
		/// <typeparam name="Key">The key type of the cache</typeparam>
		/// <typeparam name="Value">The value type of the cache</typeparam>
		/// <param name="cache">The actual cache to key off of</param>
		public bool Unregister<Key, Value>(ICache<Key, Value> cache)
		{
			ICleanupStrategy unused;

			// TODO: adjust frequency and stop if not being used
			return _registration.TryRemove(cache.GetHashCode(), out unused);
		}
		
		/// <summary>
		/// The main processing task for the cleanup janitor
		/// </summary>
		private void CleanupTask()
		{
			while (true)
			{
				var sleep = SleepTime; // in case we are changed between calls
				Thread.Sleep(sleep);

				foreach (var strategy in _registration)
				{
					if (UpdateTimeouts(strategy, sleep))
					{
						strategy.Value.PerformCleanup();
					}
				}				
			}
		}			
				
		/// <summary>
		/// A helper method to check if a cleanup is ready to run, or if we just
		/// need to update that cleanup's expiration timer.
		/// </summary>
		/// <param name="strategy">The cleanup strategy to run</param>
		/// <param name="sleep">The amount of time we slept</param>
		/// <returns>true if we should run, false otherwise</returns>
		private bool UpdateTimeouts(KeyValuePair<int, ICleanupStrategy> strategy, TimeSpan sleep)
		{
			var timeout = strategy.Value.Options.Frequency;			
			var exists = _timeouts.TryGetValue(strategy.Key, out timeout);
			var result = (timeout.CompareTo(TimeSpan.Zero) <= 0);

			if (result || !exists)
			{	// if we have expired or if we are new, reset our values
				_timeouts[strategy.Key] = strategy.Value.Options.Frequency;
			}
			else
			{	// otherwise, keep trying to expire
				_timeouts[strategy.Key] -= sleep;
			}

			return result;
		}
	}
}

