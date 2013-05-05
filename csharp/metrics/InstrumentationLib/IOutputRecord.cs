
namespace InstrumentationLib
{
	/// <summary>
	/// Output record interface.
	/// </summary>
	public interface IOutputRecord
	{
		/// <summary>
		/// The length of the marshaled data.
		/// </summary>
		int Length { get; }

		/// <summary>
		/// The offset of the marshaled data.
		/// NOTE:  Do not try to implement a class where offset is not constant.
		/// </summary>
		int Offset { get; }

		/// <summary>
		/// Marshals the data.
		/// </summary>
		/// <returns>The marshaled data.</returns>
		byte[] ToByteArray();

	}
}
