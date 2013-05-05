using System.Collections.Generic;

namespace SharedAssemblies.Core.Containers
{		
	/// <summary>
	/// Helper factory to simplify creating <see cref="KeyValuePairFactory"/>
	/// </summary>
	public static class KeyValuePairFactory
	{
		/// <summary>
		/// Factory to infer the type parameters and create a <see cref="KeyValuePairFactory"/>
		/// </summary>
		/// <typeparam name="T1">The first type to infer</typeparam>
		/// <typeparam name="T2">The second type to infer</typeparam>
		/// <param name="key">The key for the pair</param>
		/// <param name="value">The value for the pair</param>
		/// <returns>The populated pair</returns>
		public static KeyValuePair<T1, T2> Create<T1, T2>(T1 key, T2 value)
		{
			return new KeyValuePair<T1, T2>(key, value);
		}
	}
}
