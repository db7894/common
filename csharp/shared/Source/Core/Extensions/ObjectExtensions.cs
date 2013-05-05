using System;
using System.Linq;
using SharedAssemblies.Core.Conversions;

namespace SharedAssemblies.Core.Extensions
{
	/// <summary>
	/// To include functions for validation on basic data types i.e. if value is default or null
	/// </summary>    
	public static class ObjectExtensions
	{
		/// <summary>
		/// A helper method to perform a null reference check before operating
		/// on a given object.
		/// </summary>
		/// <param name="context">The object context to perform the null check on</param>
		/// <param name="message">The message to supply in the exception</param>
		public static void Guard(this object context, string message)
		{
			if (context == null)
			{
				throw new ArgumentNullException(message);
			}
		}

		/// <summary>
		/// Checks to see if the object has null default value for basic types
		/// </summary>
		/// <typeparam name="T">Type of object being passed</typeparam>
		/// <param name="value">Object whose value needs to be checked</param>
		/// <returns>true if the value is null default. Otherwise returns false</returns>
		public static bool IsDefault<T>(this T value)
		{
			return (Equals(value, default(T)));
		}

		/// <summary>
		/// Returns true if the value is null or is the DBNull.Value.
		/// </summary>
		/// <param name="value">The value to test for null or DBNull.Value</param>
		/// <returns>True if value is null or DBNull.Value</returns>
		public static bool IsNullOrDbNull(this object value)
		{
			return (value == null) || Equals(value, DBNull.Value);
		}

		/// <summary>
		/// Generically converts from one type to another type.  If the to type (T) is a string,
		/// it will invoke ToString, if the value is IConvertible, it will invoke ConvertTo, 
		/// if the type converting to is an enum, it will attempt to parse the enum, otherwise, 
		/// it will attempt a cast.  It will return the defaultValue if the value
		/// is a null reference or the DBNull.Value object.
		/// </summary>
		/// <typeparam name="T">The type to convert to</typeparam>
		/// <param name="value">The value to convert</param>
		/// <param name="defaultValue">The default value to use if null/DBNull.Value</param>
		/// <returns>The converted value in the converted type</returns>
		/// <exception>InvalidCastException if no conversion possible between the types</exception>
		public static T ToType<T>(this object value, T defaultValue)
		{
			return TypeConverter.ToType<T>(value, defaultValue);
		}

		/// <summary>
		/// <para>
		/// Generically converts from one type to another type.  If the to type (T) is a string,
		/// it will invoke ToString, if the value is IConvertible, it will invoke ConvertTo, 
		/// otherwise, it will attempt a cast.  It will return the default(T) if the value
		/// is a null reference or the DBNull.Value object.  
		/// </para>
		/// <para>
		/// Warning!  If T is a value type, then 
		/// that means it will have the default value, not NULL!  If you wnat a value type to 
		/// default to null, use ToNullableType<typeparamref name="T"/>.
		/// </para>
		/// </summary>
		/// <typeparam name="T">The type to convert to</typeparam>
		/// <param name="value">The value to convert</param>
		/// <returns>The converted value in the converted type</returns>
		/// <exception>InvalidCastException if no conversion possible between the types</exception>
		public static T ToType<T>(this object value)
		{
			return TypeConverter.ToType<T>(value, default(T));
		}

		/// <summary>
		/// <para>
		/// Attempts to convert from one type to another type.  If the to type (T) is a string,
		/// it will invoke ToString, if the value is IConvertible, it will invoke ConvertTo, 
		/// if the type converting to is an enum, it will attempt to parse the enum, otherwise, 
		/// it will attempt a cast.  It will return the defaultValue if the value
		/// is a null reference or the DBNull.Value object.
		/// </para>
		/// <para>
		/// If the parse fails, it will absorb the exception and return the default value.  
		/// However, it will NOT absorb an InvalidCastException which means the types were
		/// completely irreconcilable.
		/// </para>
		/// </summary>
		/// <typeparam name="T">The type to convert to</typeparam>
		/// <param name="value">The value to convert</param>
		/// <param name="defaultValue">The default value to use if null/DBNull.Value</param>
		/// <returns>The converted value in the converted type</returns>
		/// <exception>InvalidCastException if no conversion possible between the types</exception>
		public static T TryToType<T>(this object value, T defaultValue)
		{
			return TypeConverter.TryToType<T>(value, defaultValue);
		}

		/// <summary>
		/// <para>
		/// Attempts to convert from one type to another type.  If the to type (T) is a string,
		/// it will invoke ToString, if the value is IConvertible, it will invoke ConvertTo, 
		/// if the type converting to is an enum, it will attempt to parse the enum, otherwise, 
		/// it will attempt a cast.  It will return the defaultValue if the value
		/// is a null reference or the DBNull.Value object.
		/// </para>
		/// <para>
		/// If the parse fails, it will absorb the exception and return the default value.  
		/// However, it will NOT absorb an InvalidCastException which means the types were
		/// completely irreconcilable.
		/// </para>
		/// <para>
		/// Warning!  If T is a value type, then 
		/// that means it will have the default value, not NULL!  If you wnat a value type to 
		/// default to null, use ToNullableType<typeparamref name="T"/>.
		/// </para>
		/// </summary>
		/// <typeparam name="T">The type to convert to</typeparam>
		/// <param name="value">The value to convert</param>
		/// <returns>The converted value in the converted type</returns>
		/// <exception>InvalidCastException if no conversion possible between the types</exception>
		public static T TryToType<T>(this object value)
		{
			return TypeConverter.TryToType<T>(value, default(T));
		}

		/// <summary>
		/// Generically converts from one type to a nullable type.  If the to type (T) is a string,
		/// it will invoke ToString, if the value is IConvertible, it will invoke ConvertTo, 
		/// if the type converting to is an enum, it will attempt to parse the enum, otherwise, 
		/// it will attempt a cast.  It will return the defaultValue if the value
		/// is a null reference or the DBNull.Value object.
		/// </summary>
		/// <typeparam name="T">The type to convert to</typeparam>
		/// <param name="value">The value to convert</param>
		/// <returns>The converted value in the converted type</returns>
		/// <exception>InvalidCastException if no conversion possible between the types</exception>
		public static T? ToNullableType<T>(this object value)
			where T : struct 
		{
			return TypeConverter.ToNullableType<T>(value);
		}

		/// <summary>
		/// <para>
		/// Generically converts from one type to a nullable type.  If the to type (T) is a string,
		/// it will invoke ToString, if the value is IConvertible, it will invoke ConvertTo, 
		/// if the type converting to is an enum, it will attempt to parse the enum, otherwise, 
		/// it will attempt a cast.  It will return the defaultValue if the value
		/// is a null reference or the DBNull.Value object.
		/// </para>
		/// <para>
		/// If the parse fails, it will absorb the exception and return the default value.  
		/// However, it will NOT absorb an InvalidCastException which means the types were
		/// completely irreconcilable.
		/// </para>
		/// </summary>
		/// <typeparam name="T">The type to convert to</typeparam>
		/// <param name="value">The value to convert</param>
		/// <returns>The converted value in the converted type</returns>
		/// <exception>InvalidCastException if no conversion possible between the types</exception>
		public static T? TryToNullableType<T>(this object value)
			where T : struct
		{
			return TypeConverter.TryToNullableType<T>(value);
		}


		/// <summary>
		/// Checks to see if the object given in value equals any of the objects in the variable
		/// argument list.
		/// </summary>
		/// <param name="value">The value to compare against the list.</param>
		/// <param name="allOtherValues">The list of values to compare against.</param>
		/// <returns>True if the value given equals any of the values passed.</returns>
		public static bool EqualsAny(this object value, params object[] allOtherValues)
		{
			if (allOtherValues == null)
			{
				throw new ArgumentNullException("allOtherValues");
			}

			if (allOtherValues.Length == 0)
			{
				throw new ArgumentException("Variable parameter list must be at least length one",
											"allOtherValues");
			}

			// if we get through all of them, no matches.
			return allOtherValues.Any(v => Equals(value, v));
		}

		/// <summary>
		/// Basic implementation of With for lightweight "Maybe" processing.
		/// </summary>
		/// <typeparam name="TInput">Type of input value.</typeparam>
		/// <typeparam name="TResult">Type of result value.</typeparam>
		/// <param name="value">The value to evaluate.</param>
		/// <param name="evaluator">The method of evaluation.</param>
		/// <returns>The evaluation result or null if value is null.</returns>
		/// <remarks>This is now completely deprecated as an error in version 2.0</remarks>
		[Obsolete("Due to readability concerns, prefer Maybe() - has same functionality", true)]
		public static TResult With<TInput, TResult>(this TInput value, Func<TInput, TResult> evaluator)
			where TResult : class
			where TInput : class
		{
			return (value != null) ? evaluator(value) : null;
		}

		/// <summary>
		/// Basic implementation of Return for lightweight "Maybe" processing.  It should be noted that this will return
		/// the failure result only if the value (LHS of Return) is null, not if the evaluator returns null!
		/// </summary>
		/// <typeparam name="TInput">Type of input value.</typeparam>
		/// <typeparam name="TResult">Type of result value.</typeparam>
		/// <param name="value">The value to evaluate.</param>
		/// <param name="evaluator">The method of evaluation.</param>
		/// <param name="failureValue">The value to return if the input value is null.</param>
		/// <returns>The evaluation result or failureValue if value is null.</returns>
		/// <remarks>This is now completely deprecated as an error in version 2.0</remarks>
		[Obsolete("Due to readability concerns, prefer Maybe() - has same functionality", true)]
		public static TResult Return<TInput, TResult>(this TInput value, Func<TInput, TResult> evaluator, TResult failureValue) 
			where TInput : class
		{
			return (value != null) ? evaluator(value) : failureValue;
		}

		/// <summary>
		/// Basic implementation of Return for lightweight "Maybe" processing.  This version contains both a valueFailureResult and
		/// an evaluatorFailure result.  The former is used when the value being tested is null, the latter is used when the evaluator
		/// returns null.
		/// </summary>
		/// <typeparam name="TInput">Type of input value.</typeparam>
		/// <typeparam name="TResult">Type of result value.</typeparam>
		/// <param name="value">The value to evaluate.</param>
		/// <param name="evaluator">The method of evaluation.</param>
		/// <param name="valueFailureResult">The value to return if the input value is null.</param>
		/// <param name="evaluatorFailureResult">The value to return if the evaluator returns null.</param>
		/// <returns>The evaluation result or failureValue if value is null.</returns>
		/// <remarks>This is now completely deprecated as an error in version 2.0</remarks>
		[Obsolete("Due to readability concerns, prefer Maybe() - has same functionality", true)]
		public static TResult Return<TInput, TResult>(this TInput value, Func<TInput, TResult> evaluator, 
			TResult valueFailureResult, TResult evaluatorFailureResult)
				where TInput : class
				where TResult : class
		{
			return (value != null) 
				? (evaluator(value) ?? evaluatorFailureResult) 
				: valueFailureResult;
		}

        /// <summary>
        /// Basic implementation of Return for lightweight "Maybe" processing with a failure value of default(TResult).
        /// </summary>
        /// <typeparam name="TInput">Type of input value.</typeparam>
        /// <typeparam name="TResult">Type of result value.</typeparam>
        /// <param name="value">The value to evaluate.</param>
        /// <param name="evaluator">The method of evaluation.</param>
        /// <returns>The evaluation result or default(TResult) if value is null.</returns>
        /// <remarks>This is now completely deprecated as an error in version 2.0</remarks>
        [Obsolete("Due to readability concerns, prefer Maybe() - has same functionality", true)]
        public static TResult Return<TInput, TResult>(this TInput value, Func<TInput, TResult> evaluator)
            where TInput : class
        {
            return (value != null) ? evaluator(value) : default(TResult);
        }

        /// <summary>
        /// Basic implementation of With for lightweight "Maybe" processing.
        /// </summary>
        /// <typeparam name="TInput">Type of input value.</typeparam>
        /// <typeparam name="TResult">Type of result value.</typeparam>
        /// <param name="value">The value to evaluate.</param>
        /// <param name="evaluator">The method of evaluation.</param>
        /// <returns>The evaluation result or null if value is null.</returns>
        public static TResult Maybe<TInput, TResult>(this TInput value, Func<TInput, TResult> evaluator)
            where TInput : class
        {
            return (value != null) ? evaluator(value) : default(TResult);
        }

        /// <summary>
        /// Basic implementation of Return for lightweight "Maybe" processing.  It should be noted that this will return
        /// the failure result only if the value (LHS of Return) is null, not if the evaluator returns null!
        /// </summary>
        /// <typeparam name="TInput">Type of input value.</typeparam>
        /// <typeparam name="TResult">Type of result value.</typeparam>
        /// <param name="value">The value to evaluate.</param>
        /// <param name="evaluator">The method of evaluation.</param>
        /// <param name="failureValue">The value to return if the input value is null.</param>
        /// <returns>The evaluation result or failureValue if value is null.</returns>
        public static TResult Maybe<TInput, TResult>(this TInput value, Func<TInput, TResult> evaluator, TResult failureValue)
            where TInput : class
        {
            return (value != null) ? evaluator(value) : failureValue;
        }

        /// <summary>
        /// Basic implementation of With for lightweight "Maybe" processing.
        /// </summary>
        /// <typeparam name="TInput">Type of input value.</typeparam>
        /// <typeparam name="TResult">Type of result value.</typeparam>
        /// <param name="value">The value to evaluate.</param>
        /// <param name="evaluator">The method of evaluation.</param>
        /// <returns>The evaluation result or null if value is null.</returns>
        public static TResult MaybeValue<TInput, TResult>(this TInput? value, Func<TInput, TResult> evaluator)
            where TInput : struct
        {
            return value.HasValue ? evaluator(value.Value) : default(TResult);
        }

        /// <summary>
        /// Basic implementation of Return for lightweight "Maybe" processing.  It should be noted that this will return
        /// the failure result only if the value (LHS of Return) is null, not if the evaluator returns null!
        /// </summary>
        /// <typeparam name="TInput">Type of input value.</typeparam>
        /// <typeparam name="TResult">Type of result value.</typeparam>
        /// <param name="value">The value to evaluate.</param>
        /// <param name="evaluator">The method of evaluation.</param>
        /// <param name="failureValue">The value to return if the input value is null.</param>
        /// <returns>The evaluation result or failureValue if value is null.</returns>
        public static TResult MaybeValue<TInput, TResult>(this TInput? value, Func<TInput, TResult> evaluator, TResult failureValue)
            where TInput : struct
        {
            return value.HasValue ? evaluator(value.Value) : failureValue;
        }
        
        /// <summary>
        /// Basic implementation of With for lightweight "Maybe" processing.
        /// </summary>
        /// <typeparam name="TInput">Type of input value.</typeparam>
        /// <param name="value">The value to evaluate.</param>
        /// <returns>The evaluation result or null if value is null.</returns>
        public static TInput MaybeValue<TInput>(this TInput? value) where TInput : struct
        {
            return value ?? default(TInput);
        }

        /// <summary>
        /// Basic implementation of Return for lightweight "Maybe" processing.  It should be noted that this will return
        /// the failure result only if the value (LHS of Return) is null, not if the evaluator returns null!
        /// </summary>
        /// <typeparam name="TInput">Type of input value.</typeparam>
        /// <param name="value">The value to evaluate.</param>
        /// <param name="failureValue">The value to return if the input value is null.</param>
        /// <returns>The evaluation result or failureValue if value is null.</returns>
        public static TInput MaybeValue<TInput>(this TInput? value, TInput failureValue) where TInput : struct
        {
            return value ?? failureValue;
        }

        /// <summary>
		/// Checks to see if a value is contained within a set of values.
		/// </summary>
		/// <typeparam name="T">The type of item to check.</typeparam>
		/// <param name="value">The value to find in the set.</param>
		/// <param name="compareSet">The set to find the value in.</param>
		/// <returns>True if the item is in the set, false otherwise.</returns>
		public static bool In<T>(this T value, params T[] compareSet)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}

			if (compareSet == null)
			{
				throw new ArgumentNullException("compareSet");
			}

			if (compareSet.Length == 0)
			{
				throw new ArgumentException("Variable argument list must have at least one value.", "compareSet");
			}

			return compareSet.Contains(value);
		}
	}
}
