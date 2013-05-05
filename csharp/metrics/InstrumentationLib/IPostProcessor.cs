
namespace InstrumentationLib
{
	/// <summary>
	/// Post processing interface.
	/// </summary>
	/// <typeparam name="T">An IOutputRec implementation.</typeparam>
	public interface IPostProcessor<in T> where T : IOutputRecord
	{
		/// <summary>
		/// Initialization of the underlying implementation.
		/// </summary>
		/// <returns>true on success</returns>
		bool Open();

		/// <summary>
		/// Persists the IOutputRec.
		/// </summary>
		/// <param name="rec">The IOutputRec implementation.</param>
		/// <returns>true on success</returns>
		bool Write(T rec);

		/// <summary>
		/// Insures all data is persisted and readys the implementation
		/// for re-opening or disposal.
		/// </summary>
		void Close();
	}
}
