using System;

namespace SharedAssemblies.Core.Patterns
{
	/// <summary>
	/// A collection of functional primitives.
	/// </summary>
	public static class Functional
	{
		#region K-Combinator

		/// <summary>
		/// Given an argument, create a function that returns that argument
		/// regardless of the input argument.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument</typeparam>
		/// <typeparam name="TResult">The type of the result of the operation</typeparam>
		/// <returns>The modified function</returns>
		public static Func<TResult, Func<T1, TResult>> K<T1, TResult>() 
		{
			return arg1 => arg2 => arg1;
		} 

		/// <summary>
		/// Given an argument, create a function that returns that argument
		/// regardless of the input argument.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument</typeparam>
		/// <typeparam name="TResult">The type of the result of the operation</typeparam>
		/// <param name="arg1">The argument to create a function of</param>
		/// <returns>The modified function</returns>
		public static Func<T1, TResult> K<T1, TResult>(TResult arg1) 
		{
			return arg2 => arg1;
		} 

		/// <summary>
		/// Given an argument, create a function that returns that argument
		/// </summary>
		/// <typeparam name="TResult">The type of the result of the operation</typeparam>
		/// <param name="arg1">The argument to create a function of</param>
		/// <returns>The modified function</returns>
		public static Func<TResult> K<TResult>(TResult arg1) 
		{
			return () => arg1;
		} 

		#endregion

		#region I-Combinator

		/// <summary>
		/// Create a function that returns the argument it is given
		/// </summary>
		/// <typeparam name="TResult">The type of the result of the operation</typeparam>
		/// <returns>The modified function</returns>
		public static Func<TResult, TResult> I<TResult>() 
		{
			return arg1 => arg1;
		}

		/// <summary>
		/// Create a function that returns the argument it is given
		/// </summary>
		/// <typeparam name="TResult">The type of the result of the operation</typeparam>
		/// <param name="arg1">The argument to return an identity of</param>
		/// <returns>The modified function</returns>
		public static TResult I<TResult>(TResult arg1)
		{
			return arg1;
		} 

		#endregion

		#region Y-Combinator

		/// <summary>
		/// Implements anonymous recursion using a function that takes the function
		/// to recurse on and returns the main function implementation. (Y-Combinator)
		/// </summary>
		/// <typeparam name="T1">The type of the first argument</typeparam>
		/// <typeparam name="TResult">The type of the result of the operation</typeparam>
		/// <param name="func">The function to operate on</param>
		/// <returns>The modified function</returns>
		public static Func<T1, TResult> Y<T1, TResult>(
			Func<Func<T1, TResult>, Func<T1, TResult>> func) 
		{
			return arg1 => func(Y(func))(arg1); 
		} 
 
		/// <summary>
		/// Implements anonymous recursion using a function that takes the function
		/// to recurse on and returns the main function implementation. (Y-Combinator)
		/// </summary>
		/// <typeparam name="T1">The type of the first argument</typeparam>
		/// <typeparam name="T2">The type of the second argument</typeparam>
		/// <typeparam name="TResult">The type of the result of the operation</typeparam>
		/// <param name="func">The function to operate on</param>
		/// <returns>The modified function</returns>
		public static Func<T1, T2, TResult> Y<T1, T2, TResult>(
			Func<Func<T1, T2, TResult>, Func<T1, T2, TResult>> func) 
		{
			return (arg1, arg2) => func(Y(func))(arg1, arg2); 
		} 
 		
		/// <summary>
		/// Implements anonymous recursion using a function that takes the function
		/// to recurse on and returns the main function implementation. (Y-Combinator)
		/// </summary>
		/// <typeparam name="T1">The type of the first argument</typeparam>
		/// <typeparam name="T2">The type of the second argument</typeparam>
		/// <typeparam name="T3">The type of the third argument</typeparam>
		/// <typeparam name="TResult">The type of the result of the operation</typeparam>
		/// <param name="func">The function to operate on</param>
		/// <returns>The modified function</returns>
		public static Func<T1, T2, T3, TResult> Y<T1, T2, T3, TResult>(
			Func<Func<T1, T2, T3, TResult>, Func<T1, T2, T3, TResult>> func) 
		{
			return (arg1, arg2, arg3) => func(Y(func))(arg1, arg2, arg3); 
		} 
 
		/// <summary>
		/// Implements anonymous recursion using a function that takes the function
		/// to recurse on and returns the main function implementation. (Y-Combinator)
		/// </summary>
		/// <typeparam name="T1">The type of the first argument</typeparam>
		/// <typeparam name="T2">The type of the second argument</typeparam>
		/// <typeparam name="T3">The type of the third argument</typeparam>
		/// <typeparam name="T4">The type of the fourth argument</typeparam>
		/// <typeparam name="TResult">The type of the result of the operation</typeparam>
		/// <param name="func">The function to operate on</param>
		/// <returns>The modified function</returns>
		public static Func<T1, T2, T3, T4, TResult> Y<T1, T2, T3, T4, TResult>(
			Func<Func<T1, T2, T3, T4, TResult>, Func<T1, T2, T3, T4, TResult>> func) 
		{
			return (arg1, arg2, arg3, arg4) => func(Y(func))(arg1, arg2, arg3, arg4); 
		} 

		#endregion

		#region Thunking Methods

		/// <summary>
		/// Given an argument, return a constant function that returns that argument.
		/// </summary>
		/// <typeparam name="TResult">The type of the result of the operation</typeparam>
		/// <param name="arg1">The first argument to thunk</param>
		/// <returns>The modified function</returns>
		public static Func<TResult> Thunk<TResult>(TResult arg1)
		{
			return () => arg1;
		} 

		/// <summary>
		/// Given a function that takes N arguments and the required arguments, return
		/// the populated function whose execution can be deferred.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument</typeparam>
		/// <typeparam name="TResult">The type of the result of the operation</typeparam>
		/// <param name="func">The function to operate on</param>
		/// <param name="arg1">The first argument to thunk</param>
		/// <returns>The modified function</returns>
		public static Func<TResult> Thunk<T1, TResult>(Func<T1, TResult> func, T1 arg1) 
		{ 
			return () => func(arg1); 
		} 

		/// <summary>
		/// Given a function that takes N arguments and the required arguments, return
		/// the populated function whose execution can be deferred.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument</typeparam>
		/// <typeparam name="T2">The type of the second argument</typeparam>
		/// <typeparam name="TResult">The type of the result of the operation</typeparam>
		/// <param name="func">The function to operate on</param>
		/// <param name="arg1">The first argument to thunk</param>
		/// <param name="arg2">The second argument to thunk</param>
		/// <returns>The modified function</returns>
		public static Func<TResult> Thunk<T1, T2, TResult>(
			Func<T1, T2, TResult> func, T1 arg1, T2 arg2) 
		{ 
			return () => func(arg1, arg2); 
		} 

		/// <summary>
		/// Given a function that takes N arguments and the required arguments, return
		/// the populated function whose execution can be deferred.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument</typeparam>
		/// <typeparam name="T2">The type of the second argument</typeparam>
		/// <typeparam name="T3">The type of the third argument</typeparam>
		/// <typeparam name="TResult">The type of the result of the operation</typeparam>
		/// <param name="func">The function to operate on</param>
		/// <param name="arg1">The first argument to thunk</param>
		/// <param name="arg2">The second argument to thunk</param>
		/// <param name="arg3">The third argument to thunk</param>
		/// <returns>The modified function</returns>
		public static Func<TResult> Thunk<T1, T2, T3, TResult>(
			Func<T1, T2, T3, TResult> func, T1 arg1, T2 arg2, T3 arg3)
		{ 
			return () => func(arg1, arg2, arg3); 
		}

		/// <summary>
		/// Given a function that takes N arguments and the required arguments, return
		/// the populated function whose execution can be deferred.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument</typeparam>
		/// <typeparam name="T2">The type of the second argument</typeparam>
		/// <typeparam name="T3">The type of the third argument</typeparam>
		/// <typeparam name="T4">The type of the fourth argument</typeparam>
		/// <typeparam name="TResult">The type of the result of the operation</typeparam>
		/// <param name="func">The function to operate on</param>
		/// <param name="arg1">The first argument to thunk</param>
		/// <param name="arg2">The second argument to thunk</param>
		/// <param name="arg3">The third argument to thunk</param>
		/// <param name="arg4">The fourth argument to thunk</param>
		/// <returns>The modified function</returns>
		public static Func<TResult> Thunk<T1, T2, T3, T4, TResult>(
			Func<T1, T2, T3, T4, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			return () => func(arg1, arg2, arg3, arg4);
		} 

		#endregion

		#region Currying Methods
 
		/// <summary>
		/// Given a function that takes N arguments, return a function that can be
		/// partially applied with lesser number of arguments.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument</typeparam>
		/// <typeparam name="T2">The type of the second argument</typeparam>
		/// <typeparam name="TResult">The type of the result of the operation</typeparam>
		/// <param name="func">The function to operate on</param>
		/// <returns>The modified function</returns>
		public static Func<T1, Func<T2, TResult>> Curry<T1, T2, TResult>(Func<T1, T2, TResult> func) 
		{ 
			return arg1 => arg2 => func(arg1, arg2); 
		} 

		/// <summary>
		/// Given a function that takes N arguments, return a function that can be
		/// partially applied with lesser number of arguments.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument</typeparam>
		/// <typeparam name="T2">The type of the second argument</typeparam>
		/// <typeparam name="T3">The type of the third argument</typeparam>
		/// <typeparam name="TResult">The type of the result of the operation</typeparam>
		/// <param name="func">The function to operate on</param>
		/// <returns>The modified function</returns>
		public static Func<T1, Func<T2, Func<T3, TResult>>> Curry<T1, T2, T3, TResult>(
			Func<T1, T2, T3, TResult> func) 
		{ 
			return arg1 => arg2 => arg3 => func(arg1, arg2, arg3); 
		} 

		/// <summary>
		/// Given a function that takes N arguments, return a function that can be
		/// partially applied with lesser number of arguments.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument</typeparam>
		/// <typeparam name="T2">The type of the second argument</typeparam>
		/// <typeparam name="T3">The type of the third argument</typeparam>
		/// <typeparam name="T4">The type of the fourth argument</typeparam>
		/// <typeparam name="TResult">The type of the result of the operation</typeparam>
		/// <param name="func">The function to operate on</param>
		/// <returns>The modified function</returns>
		public static Func<T1, Func<T2, Func<T3, Func<T4, TResult>>>> Curry<T1, T2, T3, T4, TResult>(
			Func<T1, T2, T3, T4, TResult> func) 
		{ 
			return arg1 => arg2 => arg3 => arg4 => func(arg1, arg2, arg3, arg4); 
		} 

		#endregion

		#region Reverse Currying Methods
 
		/// <summary>
		/// Given a function that takes N arguments, return a function that can be
		/// partially applied with lesser number of arguments in reverse.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument</typeparam>
		/// <typeparam name="T2">The type of the second argument</typeparam>
		/// <typeparam name="TResult">The type of the result of the operation</typeparam>
		/// <param name="func">The function to operate on</param>
		/// <returns>The modified function</returns>
		public static Func<T2, Func<T1, TResult>> ReverseCurry<T1, T2, TResult>(
			Func<T1, T2, TResult> func) 
		{ 
			return arg2 => arg1 => func(arg1, arg2); 
		} 

		/// <summary>
		/// Given a function that takes N arguments, return a function that can be
		/// partially applied with lesser number of arguments in reverse.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument</typeparam>
		/// <typeparam name="T2">The type of the second argument</typeparam>
		/// <typeparam name="T3">The type of the third argument</typeparam>
		/// <typeparam name="TResult">The type of the result of the operation</typeparam>
		/// <param name="func">The function to operate on</param>
		/// <returns>The modified function</returns>
		public static Func<T3, Func<T2, Func<T1, TResult>>> ReverseCurry<T1, T2, T3, TResult>(
			Func<T1, T2, T3, TResult> func) 
		{ 
			return arg3 => arg2 => arg1 => func(arg1, arg2, arg3); 
		} 

		/// <summary>
		/// Given a function that takes N arguments, return a function that can be
		/// partially applied with lesser number of arguments in reverse.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument</typeparam>
		/// <typeparam name="T2">The type of the second argument</typeparam>
		/// <typeparam name="T3">The type of the third argument</typeparam>
		/// <typeparam name="T4">The type of the fourth argument</typeparam>
		/// <typeparam name="TResult">The type of the result of the operation</typeparam>
		/// <param name="func">The function to operate on</param>
		/// <returns>The modified function</returns>
		public static Func<T4, Func<T3, Func<T2, Func<T1, TResult>>>> ReverseCurry<T1, T2, T3, T4, TResult>(
			Func<T1, T2, T3, T4, TResult> func) 
		{ 
			return arg4 => arg3 => arg2 => arg1 => func(arg1, arg2, arg3, arg4); 
		} 

		#endregion

		#region Un-Currying Methods

		/// <summary>
		/// Given a function that has been curried to a lesser number of arguments,
		/// return a function that takes all parameters at once.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument</typeparam>
		/// <typeparam name="T2">The type of the second argument</typeparam>
		/// <typeparam name="TResult">The type of the result of the operation</typeparam>
		/// <param name="func">The function to operate on</param>
		/// <returns>The modified function</returns>
		public static Func<T1, T2, TResult> Uncurry<T1, T2, TResult>(
			Func<T1, Func<T2, TResult>> func) 
		{ 
			return (arg1, arg2) => func(arg1)(arg2); 
		} 

		/// <summary>
		/// Given a function that has been curried to a lesser number of arguments,
		/// return a function that takes all parameters at once.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument</typeparam>
		/// <typeparam name="T2">The type of the second argument</typeparam>
		/// <typeparam name="T3">The type of the third argument</typeparam>
		/// <typeparam name="TResult">The type of the result of the operation</typeparam>
		/// <param name="func">The function to operate on</param>
		/// <returns>The modified function</returns>
		public static Func<T1, T2, T3, TResult> Uncurry<T1, T2, T3, TResult>(
			Func<T1, Func<T2, Func<T3, TResult>>> func) 
		{ 
			return (arg1, arg2, arg3) => func(arg1)(arg2)(arg3); 
		} 

		/// <summary>
		/// Given a function that has been curried to a lesser number of arguments,
		/// return a function that takes all parameters at once.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument</typeparam>
		/// <typeparam name="T2">The type of the second argument</typeparam>
		/// <typeparam name="T3">The type of the third argument</typeparam>
		/// <typeparam name="T4">The type of the fourth argument</typeparam>
		/// <typeparam name="TResult">The type of the result of the operation</typeparam>
		/// <param name="func">The function to operate on</param>
		/// <returns>The modified function</returns>
		public static Func<T1, T2, T3, T4, TResult> Uncurry<T1, T2, T3, T4, TResult>(
			Func<T1, Func<T2, Func<T3, Func<T4, TResult>>>> func) 
		{ 
			return (arg1, arg2, arg3, arg4) => func(arg1)(arg2)(arg3)(arg4); 
		} 

		#endregion
 
		#region Reverse Un-Currying Methods
 
		/// <summary>
		/// Given a function that has been curried in reverse to a lesser number of
		/// arguments, return a function that takes all parameters at once.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument</typeparam>
		/// <typeparam name="T2">The type of the second argument</typeparam>
		/// <typeparam name="TResult">The type of the result of the operation</typeparam>
		/// <param name="func">The function to operate on</param>
		/// <returns>The modified function</returns>
		public static Func<T1, T2, TResult> ReverseUncurry<T1, T2, TResult>(
			Func<T2, Func<T1, TResult>> func) 
		{ 
			return (arg1, arg2) => func(arg2)(arg1); 
		} 

		/// <summary>
		/// Given a function that has been curried in reverse to a lesser number of
		/// arguments, return a function that takes all parameters at once.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument</typeparam>
		/// <typeparam name="T2">The type of the second argument</typeparam>
		/// <typeparam name="T3">The type of the third argument</typeparam>
		/// <typeparam name="TResult">The type of the result of the operation</typeparam>
		/// <param name="func">The function to operate on</param>
		/// <returns>The modified function</returns>
		public static Func<T1, T2, T3, TResult> ReverseUncurry<T1, T2, T3, TResult>(
			Func<T3, Func<T2, Func<T1, TResult>>> func) 
		{ 
			return (arg1, arg2, arg3) => func(arg3)(arg2)(arg1); 
		} 

		/// <summary>
		/// Given a function that has been curried in reverse to a lesser number of
		/// arguments, return a function that takes all parameters at once.
		/// </summary>
		/// <typeparam name="T1">The type of the first argument</typeparam>
		/// <typeparam name="T2">The type of the second argument</typeparam>
		/// <typeparam name="T3">The type of the third argument</typeparam>
		/// <typeparam name="T4">The type of the fourth argument</typeparam>
		/// <typeparam name="TResult">The type of the result of the operation</typeparam>
		/// <param name="func">The function to operate on</param>
		/// <returns>The modified function</returns>
		public static Func<T1, T2, T3, T4, TResult> ReverseUncurry<T1, T2, T3, T4, TResult>(
			Func<T4, Func<T3, Func<T2, Func<T1, TResult>>>> func) 
		{ 
			return (arg1, arg2, arg3, arg4) => func(arg4)(arg3)(arg2)(arg1); 
		} 

		#endregion
	} 
}

