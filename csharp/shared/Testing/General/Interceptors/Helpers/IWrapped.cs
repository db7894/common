
namespace SharedAssemblies.General.Interceptors.UnitTests.Helpers
{
	/// <summary>
	/// Interface for a dummy wrappable class
	/// </summary>
	public interface IWrapped
	{
		/// <summary>
		/// Non returning no paramater method
		/// </summary>
		void NoReturnMethod();

		/// <summary>
		/// Non returning single paramater method
		/// </summary>
		/// <param name="input">Some data input</param>
		void NoReturnMethod(string input);

		/// <summary>
		/// Returning no paramater method
		/// </summary>
		/// <returns>Some return data</returns>
		string ReturnMethod();

		/// <summary>
		/// Returning single paramater method
		/// </summary>
		/// <param name="input">Some data input</param>
		/// <returns>Some return data</returns>
		string ReturnMethod(string input);
	}
}
