using System;
using System.Collections.Generic;


namespace SharedAssemblies.Core.Containers
{
	/// <summary>
	/// A read-only wrapper for a dictionary implementation.
	/// </summary>
	/// <typeparam name="TKey">The key type</typeparam>
	/// <typeparam name="TValue">The value type</typeparam>
	public sealed class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
	{
		private readonly IDictionary<TKey, TValue> _wrappedDictionary;

		/// <summary>
		/// Gets the number of items in the dictionary.
		/// </summary>
		public int Count
		{
			get { return _wrappedDictionary.Count; }
		}

		/// <summary>
		/// Gets whether the dictionary is read-only.
		/// </summary>
		public bool IsReadOnly
		{
			get { return true; }
		}

		/// <summary>
		/// Gets a collection of the keys in the dictionary.
		/// </summary>
		public ICollection<TKey> Keys
		{
			get { return _wrappedDictionary.Keys; }
		}

		/// <summary>
		/// Gets a collection of values stored in the dictionary.
		/// </summary>
		public ICollection<TValue> Values
		{
			get { return _wrappedDictionary.Values; }
		}

		/// <summary>
		/// Indexer which retrieves a value in the dictionary by using the key specified.  If this indexer is used as a setter, 
		/// will throw InvalidOperationException since this wrapper is read-only.
		/// </summary>
		/// <param name="key">The key of the item to find in the dictionary.</param>
		/// <returns>The value of the item, if found.</returns>
		public TValue this[TKey key]
		{
			get { return _wrappedDictionary[key]; }
			set { throw new InvalidOperationException("ReadOnlyDictionary only supports read operations."); }
		}

		/// <summary>
		/// Construct a read-only dictionary as a wrapper for an existing dictionary.
		/// </summary>
		/// <param name="wrappedDictionary">The mutable dictionary to wrap.</param>
		public ReadOnlyDictionary(IDictionary<TKey, TValue> wrappedDictionary)
		{
			if (wrappedDictionary == null)
			{
				throw new ArgumentNullException("wrappedDictionary");
			}
			
			_wrappedDictionary = wrappedDictionary;
		}

		/// <summary>
		/// Adds a key and value to the dictionary, always throws InvalidOperationException since this wrapper is read-only.
		/// </summary>
		/// <param name="key">The key to add.</param>
		/// <param name="value">The value to add.</param>
		public void Add(TKey key, TValue value)
		{
			throw new InvalidOperationException("ReadOnlyDictionary only supports read operations.");
		}

		/// <summary>
		/// Returns true if the dictionary contains the key specified.
		/// </summary>
		/// <param name="key">The key to check.</param>
		/// <returns>True if the dictionary contains the key specified.</returns>
		public bool ContainsKey(TKey key)
		{
			return _wrappedDictionary.ContainsKey(key);
		}

		/// <summary>
		/// Removes a key from the dictionary, always throws InvalidOperationException since this wrapper is read-only.
		/// </summary>
		/// <param name="key">The key of the item to remove from the dictionary.</param>
		/// <returns>True if the key was removed.</returns>
		public bool Remove(TKey key)
		{
			throw new InvalidOperationException("ReadOnlyDictionary only supports read operations.");
		}

		/// <summary>
		/// Attempts to get the item in the dictionary with the specified key.  Will return true if the item exists and
		/// the item will be in the out parameter, or false if the item does not exist.
		/// </summary>
		/// <param name="key">The key of the item to find.</param>
		/// <param name="value">The item itself, if the key was found.</param>
		/// <returns>True if the item was found, false otherwise.</returns>
		public bool TryGetValue(TKey key, out TValue value)
		{
			return _wrappedDictionary.TryGetValue(key, out value);
		}

		/// <summary>
		/// Adds a KeyValuePair to the dictionary, always throws InvalidOperationException since this wrapper is read-only.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void Add(KeyValuePair<TKey, TValue> item)
		{
			throw new InvalidOperationException("ReadOnlyDictionary only supports read operations.");
		}

		/// <summary>
		/// Clears all items from the dictionary, always throws InvalidOperationException since this wrapper is read-only.
		/// </summary>
		public void Clear()
		{
			throw new InvalidOperationException("ReadOnlyDictionary only supports read operations.");
		}

		/// <summary>
		/// Returns true if the dictionary contains the KeyValuePair specified, false otherwise.
		/// </summary>
		/// <param name="item">The item to find in the dictionary.</param>
		/// <returns>True if the item exists in the dictionary, false otherwise.</returns>
		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			return _wrappedDictionary.Contains(item);
		}

		/// <summary>
		/// Copies the KeyValuePairs from the dictionary to an array starting at the specified index.
		/// </summary>
		/// <param name="array">The array to copy into.</param>
		/// <param name="arrayIndex">The starting index in the array to begin copy.</param>
		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			_wrappedDictionary.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Removes an item from the dictionary, always throws InvalidOperationException since this wrapper is read-only.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		/// <returns>True if remove was successful.</returns>
		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			throw new InvalidOperationException("ReadOnlyDictionary only supports read operations.");
		}

		/// <summary>
		/// Gets a generic enumerator for the KeyValuePairs in the dictionary.
		/// </summary>
		/// <returns>An generic enumerator of KeyValuePairs.</returns>
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return _wrappedDictionary.GetEnumerator();
		}

		/// <summary>
		/// Gets an enumerator for the KeyValuePairs in the dictionary.
		/// </summary>
		/// <returns>An enumerator for KeyValuePairs.</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return ((System.Collections.IEnumerable)_wrappedDictionary).GetEnumerator();
		}
	}
}
