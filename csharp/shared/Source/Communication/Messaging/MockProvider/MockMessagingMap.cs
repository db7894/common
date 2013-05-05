using System.Collections.Generic;


namespace SharedAssemblies.Communication.Messaging.MockProvider
{
	/// <summary>
	/// Basic message queue dictionary that lists all queued messages for a provider.
	/// </summary>
	/// <typeparam name="T">The type to assign to the map</typeparam>
	public class MockMessagingMap<T> where T : class, new()
	{
		private Dictionary<string, T> _queue = new Dictionary<string, T>();


		/// <summary>
		/// Allows you to access the message queue for a sender or receiver based on the host name and the destination or source
		/// </summary>
		/// <param name="host">The host URL string.</param>
		/// <param name="destination">The destination or source queue or topic name.</param>
		/// <returns>The queue of messages.</returns>
		public T this[string host, string destination]
		{
			get
			{
				var key = host + '|' + destination;
				T result = null;

				if (!_queue.TryGetValue(key, out result))
				{
					result = new T();
					_queue.Add(key, result);
				}

				return result;
			}
		}


		/// <summary>
		/// Clear all queues and the dictionary.  Yes, could just clear the dictionary, but to be explicit...
		/// </summary>
		public void Clear()
		{
			_queue.Clear();
		}
	}
}
