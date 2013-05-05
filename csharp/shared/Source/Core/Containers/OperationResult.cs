using System;

namespace SharedAssemblies.Core.Containers
{
	/// <summary>
	/// Represents the result of an operation along with an indication
	/// of its success.
	/// </summary>
	/// <typeparam name="TValue">The underlying type of the result value</typeparam>
	public class OperationResult<TValue>
	{
		/// <summary>
		/// true if the operation was successful, false otherwise
		/// </summary>
		public bool IsSuccessful { get; private set; }

		/// <summary>
		/// true if the operation was successful, false otherwise
		/// </summary>
		public Exception Message { get; private set; }

		/// <summary>
		/// The resulting outcome of the operation
		/// </summary>
		public TValue Value { get; private set; }

		/// <summary>
		/// Initializes a new instance of the OperationResult class
		/// </summary>
		/// <param name="success">The result of the operation</param>
		/// <param name="result">The outcome of the operation</param>
		/// <param name="message">An optional message describing the result</param>
		public OperationResult(bool success, TValue result, Exception message=null)
		{
			IsSuccessful = success;
			Value = result;
			Message = message;
		}
	}

	/// <summary>
	/// Companion object to the OperationResult instance type.
	/// </summary>
	public static class OperationResult
	{	
		/// <summary>
		/// A helper factory to initialize an OperationResult
		/// </summary>
		/// <typeparam name="TValue">The underlying type of the result value</typeparam>
		/// <param name="value">The payload result of the operation</param>
		/// <returns>An initialized OperationResult</returns>
		public static OperationResult<TValue> Success<TValue>(TValue value)
		{
			return new OperationResult<TValue>(true, value);
		}

		/// <summary>
		/// A helper factory to initialize an OperationResult
		/// </summary>
		/// <typeparam name="TValue">The underlying type of the result value</typeparam>
		/// <param name="message">An optional message describing the result</param>
		/// <returns>An initialized OperationResult</returns>
		public static OperationResult<TValue> Failure<TValue>(Exception message=null)
		{
			return new OperationResult<TValue>(false, default(TValue), message);
		}

		/// <summary>
		/// A helper factory to initialize an OperationResult
		/// </summary>
		/// <typeparam name="TValue">The underlying type of the result value</typeparam>
		/// <param name="result">The result of the operation</param>
		/// <param name="value">The payload result of the operation</param>
		/// <param name="message">An optional message describing the result</param>
		/// <returns>An initialized OperationResult</returns>
		public static OperationResult<TValue> Create<TValue>(bool result,
			TValue value, Exception message=null)
		{
			return new OperationResult<TValue>(result, value, message);
		}	
	}
}
