using System.Collections;
using System.Collections.Generic;
using System;


namespace SharedAssemblies.Core.Containers
{
	/// <summary>
	/// interior class for an empty enumerator{T}.
	/// </summary>
	/// <typeparam name="T">Type of the enumerator</typeparam>
	[Obsolete("This is replaced by the Enumerable.Empty<T> method", false)]
	public sealed class EmptyEnumerator<T> : IEnumerator<T>
	{
		private static readonly EmptyEnumerator<T> _instance = new EmptyEnumerator<T>();


		/// <summary>
		/// Returns the singleton instance of the class.
		/// </summary>
		public static EmptyEnumerator<T> Instance
		{
			get { return _instance;  }
		}


		/// <summary>
		/// Gets the element in the collection at the current position of the enumerator.
		/// </summary>
		/// <returns>
		/// The element in the collection at the current position of the enumerator.
		/// </returns>
		public T Current
		{
			get { return default(T); }
		}


		/// <summary>
		/// Gets the current element in the collection.
		/// </summary>
		/// <returns>
		/// The current element in the collection.
		/// </returns>
		object IEnumerator.Current
		{
			get { return Current; }
		}


		/// <summary>
		/// Private constructor for singleton.
		/// </summary>
		private EmptyEnumerator()
		{			
		}


		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or 
		/// resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
		}


		/// <summary>
		/// Advances the enumerator to the next element of the collection.
		/// </summary>
		/// <returns>
		/// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
		/// </returns>
		public bool MoveNext()
		{
			return false;
		}


		/// <summary>
		/// Sets the enumerator to its initial position, which is before the first element in the collection.
		/// </summary>
		public void Reset()
		{
		}
	}
}
