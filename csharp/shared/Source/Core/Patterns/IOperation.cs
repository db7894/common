using System.Collections.Generic;


namespace SharedAssemblies.Core.Patterns
{
	/// <summary>
	/// Interface for a single pipeline operation
	/// </summary>
	/// <typeparam name="TInput">The type of data to process</typeparam>
	public interface IOperation<TInput>
	{
		/// <summary>
		/// Execute this stage of the pipeline
		/// </summary>
		/// <param name="input">The input data to process</param>
		/// <returns>The result of this stage of the pipeline</returns>
		IEnumerable<TInput> Execute(IEnumerable<TInput> input);
	}
}
