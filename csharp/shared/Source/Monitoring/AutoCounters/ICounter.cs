namespace SharedAssemblies.Monitoring.AutoCounters
{
	/// <summary>
	/// Interface for the instrumentation counter class
	/// </summary>
	public interface ICounter 
	{
		/// <summary>
		/// Enter a block of code to be instrumented.
		/// </summary>
		void StartBlock();

		/// <summary>
		/// Leave a block of code to be instrumented.
		/// <note>Deprecated, use <see cref="EndBlock(InstrumentationBlock)"/></note>
		/// </summary>
		void EndBlock();

		/// <summary>
		/// Leave a block of code to be instrumented.  
		/// </summary>
		/// <param name="block">Internal instrumentation data returned from <see cref="GetBlock"/></param>
		void EndBlock(InstrumentationBlock block);

		/// <summary>
		/// Returns a IDisposable InstrumentationBlock to automatically
		/// call Enter and Leave correctly to avoid screw-ups.
		/// </summary>
		/// <returns>An instrumentation block for this counter</returns>
		InstrumentationBlock GetBlock();
	}
}