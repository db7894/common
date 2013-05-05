using System;

namespace SharedAssemblies.General.Interceptors.UnitTests.Helpers
{
	/// <summary>
	/// A dummy implementation for default instance factories
	/// </summary>
	public class Wrapped : IWrapped
	{
		/// <summary>
		/// Non returning no paramater method
		/// </summary>
		public void NoReturnMethod()
		{
			throw new SystemException("NoReturnMethod");
		}

		/// <summary>
		/// Non returning single paramater method
		/// </summary>
		/// <param name="input">Some data input</param>
		public void NoReturnMethod(string input)
		{
		}

		/// <summary>
		/// Returning no paramater method
		/// </summary>
		/// <returns>Some return data</returns>
		public string ReturnMethod()
		{
			throw new SystemException("NoReturnMethod");
		}

		/// <summary>
		/// Returning single paramater method
		/// </summary>
		/// <param name="input">Some data input</param>
		/// <returns>Some return data</returns>
		public string ReturnMethod(string input)
		{
			return input.ToLower();
		}
	}
}
