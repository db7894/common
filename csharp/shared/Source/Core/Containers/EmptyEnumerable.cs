using System.Collections;
using System.Collections.Generic;
using System;


namespace SharedAssemblies.Core.Containers
{
	/// <summary>
	/// An alternate implementation of an empty IEnumerable{T}...
	/// </summary>
	/// <typeparam name="T">The type of enumerable collection</typeparam>
	[Obsolete("This is replaced by the Enumerable.Empty<T> method", false)]
	public sealed class EmptyEnumerable<T> : IEnumerable<T>
	{
		private static readonly EmptyEnumerator<T> _emptyEnumerator = EmptyEnumerator<T>.Instance;
		private static readonly EmptyEnumerable<T> _instance = new EmptyEnumerable<T>();

		/// <summary>
		/// singleton instance
		/// </summary>
		public static IEnumerable<T> Instance
		{
			get { return _instance; }
		}


		/// <summary>
		/// Private constructor to make singleton
		/// </summary>
		private EmptyEnumerable()
		{
		}


		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// An IEnumerator that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator<T> GetEnumerator()
		{
			return _emptyEnumerator;
		}


		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An IEnumerator object that can be used to iterate through the collection.
		/// </returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return _emptyEnumerator;
		}
	}
}
