using System;
using System.Collections;
using Moq.Language.Flow;

namespace SharedAssemblies.General.Testing
{
	/// <summary>
	/// A collection of extension methods to add missing functionality
	/// to the moq framework.
	/// </summary>
	public static class MoqExtensions
	{
		/// <summary>
		/// Given a collection of results, return them in a FIFO fashion
		/// on each call.
		/// </summary>
		/// <typeparam name="TObject">The mocked type object</typeparam>
		/// <typeparam name="TResult">The type of the result set</typeparam>
		/// <param name="setup">The mocked object to setup</param>
		/// <param name="results">The results to return in a FIFO</param>
		public static void ReturnsInOrder<TObject, TResult>(this ISetup<TObject, TResult> setup,
			params object[] results)
			where TObject : class
		{
			var queue = new Queue(results);
			setup.Returns(() => {
				var result = queue.Dequeue();
				if (result is Exception)
				{
					throw result as Exception;
				}
				return (TResult)result;
			});
		}
	}
}
