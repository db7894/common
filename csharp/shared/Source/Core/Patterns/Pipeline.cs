using System.Collections;
using System.Collections.Generic;

namespace SharedAssemblies.Core.Patterns
{
	/// <summary>
	/// Base class used to implement a pipe and filter operation
	/// </summary>
	/// <typeparam name="TInput">The type of data to process</typeparam>
	public class Pipeline<TInput> : IEnumerable<IOperation<TInput>>
	{
		/// <summary>
		/// Handle to the list of operations to process
		/// </summary>
		private readonly List<IOperation<TInput>> _operations =
			new List<IOperation<TInput>>();

		/// <summary>
		/// Used to register another stage in the pipeline process
		/// </summary>
		/// <param name="operation">The operation to add to the pipelien</param>
		/// <returns>The current instance, used to chain</returns>
		public Pipeline<TInput> Register(IOperation<TInput> operation)
		{
			_operations.Add(operation);
			return this;
		}

		/// <summary>
		/// Execute all the stages in the pipeline
		/// </summary>
		public void Execute()
		{
			IEnumerable<TInput> current = new List<TInput>();
			foreach (IOperation<TInput> operation in _operations)
			{
				current = operation.Execute(current);
			}

			IEnumerator<TInput> enumerator = current.GetEnumerator();
			while (enumerator.MoveNext())
			{ 
				// this merely pulls the values through the pipeline
			}
		}
        
		/// <summary>
		/// Used to register another stage in the pipeline process
		/// </summary>
		/// <param name="operation">The operation to add to the pipelien</param>
		public void Add(IOperation<TInput> operation)
		{
			_operations.Add(operation);
		}

		/// <summary>
		/// Return the typed enumerator
		/// </summary>
		/// <returns>A type specific enumerator</returns>
		public IEnumerator<IOperation<TInput>> GetEnumerator()
		{
			return _operations.GetEnumerator();
		}

		/// <summary>
		/// Return the legacy enumerator that is object-level generic
		/// </summary>
		/// <returns>The basic IEnumerator</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
