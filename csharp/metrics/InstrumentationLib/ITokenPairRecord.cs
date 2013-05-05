
namespace InstrumentationLib
{
	public interface ITokenPairRecord : IOutputRecord
	{
		Token T1 { get; set; }
		Token T2 { get; set; }
		long Ts { get; set; }

		/// <summary>
		/// Implmentations must ALWAYS return T2.Value
		/// </summary>
		ulong TokenContext { get; }
	}
}
