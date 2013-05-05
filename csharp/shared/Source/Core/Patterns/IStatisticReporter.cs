namespace SharedAssemblies.Core.Patterns
{
	/// <summary>
	/// Interface that specifies that the implementer can report statistics and clear them.
	/// </summary>
	public interface IStatisticReporter
	{
		/// <summary>
		/// Report the statistics to whatever source the implementer dictates.
		/// </summary>
		void ReportStatistics();

		/// <summary>
		/// Clears the statistics back to the default levels.
		/// </summary>
		void ClearStatistics();
	}
}
