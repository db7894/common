using System;


namespace SharedAssemblies.Core.Extensions
{
	/// <summary>
	/// Extension methods on the Action delegates
	/// </summary>
	public static class ActionExtensions
	{
		/// <summary>
		/// Extension method that converts an Action to a predicate Func delegate.
		/// </summary>
		/// <param name="source">The source action.</param>
		/// <param name="result">The result to return from the new predicate.</param>
		/// <returns>A Func predicate wrapper for the action.</returns>
		public static Func<bool> AsPredicate(this Action source, bool result = false)
		{
			return AsFunc(source, result);
		}

		/// <summary>
		/// Extension method that converts an Action to a predicate Func delegate.
		/// </summary>
		/// <param name="source">The source action.</param>
		/// <param name="result">The result to return from the new predicate.</param>
		/// <returns>A Func predicate wrapper for the action.</returns>
		/// <typeparam name="T">The argument type for the delegate.</typeparam>
		public static Func<T, bool> AsPredicate<T>(this Action<T> source, bool result = false)
		{
			return AsFunc(source, result);
		}

		/// <summary>
		/// Extension method that converts an Action to a predicate Func delegate.
		/// </summary>
		/// <param name="source">The source action.</param>
		/// <param name="result">The result to return from the new predicate.</param>
		/// <returns>A Func predicate wrapper for the action.</returns>
		/// <typeparam name="T1">The first argument type for the delegate.</typeparam>
		/// <typeparam name="T2">The second argument type for the delegate.</typeparam>
		public static Func<T1, T2, bool> AsPredicate<T1, T2>(this Action<T1, T2> source, bool result = false)
		{
			return AsFunc(source, result);
		}

		/// <summary>
		/// Extension method that converts an Action to a predicate Func delegate.
		/// </summary>
		/// <param name="source">The source action.</param>
		/// <param name="result">The result to return from the new predicate.</param>
		/// <returns>A Func predicate wrapper for the action.</returns>
		/// <typeparam name="T1">The first argument type for the delegate.</typeparam>
		/// <typeparam name="T2">The second argument type for the delegate.</typeparam>
		/// <typeparam name="T3">The third argument type for the delegate.</typeparam>
		public static Func<T1, T2, T3, bool> AsPredicate<T1, T2, T3>(this Action<T1, T2, T3> source, bool result = false)
		{
			return AsFunc(source, result);
		}

		/// <summary>
		/// Extension method that converts an Action to a predicate Func delegate.
		/// </summary>
		/// <param name="source">The source action.</param>
		/// <param name="result">The result to return from the new predicate.</param>
		/// <returns>A Func predicate wrapper for the action.</returns>
		/// <typeparam name="T1">The first argument type for the delegate.</typeparam>
		/// <typeparam name="T2">The second argument type for the delegate.</typeparam>
		/// <typeparam name="T3">The third argument type for the delegate.</typeparam>
		/// <typeparam name="T4">The fourth argument type for the delegate.</typeparam>
		public static Func<T1, T2, T3, T4, bool> AsPredicate<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> source, bool result = false)
		{
			return AsFunc(source, result);
		}

		/// <summary>
		/// Extension method that converts an Action to a Func delegate.
		/// </summary>
		/// <param name="source">The source action.</param>
		/// <param name="result">The result to return from the new Func delegate.</param>
		/// <returns>A Func wrapper for the action.</returns>
		/// <typeparam name="TResult">The return type of the new Func delegate.</typeparam>
		public static Func<TResult> AsFunc<TResult>(this Action source, TResult result = default(TResult))
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			return () =>
			{
				source();
				return result;
			};
		}

		/// <summary>
		/// Extension method that converts an Action to a Func delegate.
		/// </summary>
		/// <param name="source">The source action.</param>
		/// <param name="result">The result to return from the new Func delegate.</param>
		/// <returns>A Func wrapper for the action.</returns>
		/// <typeparam name="T">The first argument type.</typeparam>
		/// <typeparam name="TResult">The return type of the new Func delegate.</typeparam>
		public static Func<T, TResult> AsFunc<T, TResult>(this Action<T> source, TResult result = default(TResult))
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			return t =>
				{
					source(t);
					return result;
				};
		}

		/// <summary>
		/// Extension method that converts an Action to a Func delegate.
		/// </summary>
		/// <param name="source">The source action.</param>
		/// <param name="result">The result to return from the new Func delegate.</param>
		/// <returns>A Func wrapper for the action.</returns>
		/// <typeparam name="T1">The first argument type.</typeparam>
		/// <typeparam name="T2">The second argument type.</typeparam>
		/// <typeparam name="TResult">The return type of the new Func delegate.</typeparam>
		public static Func<T1, T2, TResult> AsFunc<T1, T2, TResult>(this Action<T1, T2> source, TResult result = default(TResult))
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			return (t1, t2) =>
				{
					source(t1, t2);
					return result;
				};
		}

		/// <summary>
		/// Extension method that converts an Action to a Func delegate.
		/// </summary>
		/// <param name="source">The source action.</param>
		/// <param name="result">The result to return from the new Func delegate.</param>
		/// <returns>A Func wrapper for the action.</returns>
		/// <typeparam name="T1">The first argument type.</typeparam>
		/// <typeparam name="T2">The second argument type.</typeparam>
		/// <typeparam name="T3">The third argument type.</typeparam>
		/// <typeparam name="TResult">The return type of the new Func delegate.</typeparam>
		public static Func<T1, T2, T3, TResult> AsFunc<T1, T2, T3, TResult>(this Action<T1, T2, T3> source, TResult result = default(TResult))
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			return (t1, t2, t3) =>
			{
				source(t1, t2, t3);
				return result;
			};
		}

		/// <summary>
		/// Extension method that converts an Action to a Func delegate.
		/// </summary>
		/// <param name="source">The source action.</param>
		/// <param name="result">The result to return from the new Func delegate.</param>
		/// <returns>A Func wrapper for the action.</returns>
		/// <typeparam name="T1">The first argument type.</typeparam>
		/// <typeparam name="T2">The second argument type.</typeparam>
		/// <typeparam name="T3">The third argument type.</typeparam>
		/// <typeparam name="T4">The fourth argument type.</typeparam>
		/// <typeparam name="TResult">The return type of the new Func delegate.</typeparam>
		public static Func<T1, T2, T3, T4, TResult> AsFunc<T1, T2, T3, T4, TResult>(this Action<T1, T2, T3, T4> source, 
			TResult result = default(TResult))
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			return (t1, t2, t3, t4) =>
			{
				source(t1, t2, t3, t4);
				return result;
			};
		}

		/// <summary>
		/// Extension method that converts an Func to a Func delegate.
		/// </summary>
		/// <param name="source">The, TResult> source Func.</param>
		/// <returns>A Func wrapper for the Func.</returns>
		/// <typeparam name="TResult">The return type of the new Func delegate.</typeparam>
		public static Action AsAction<TResult>(this Func<TResult> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			return () => source();
		}

		/// <summary>
		/// Extension method that converts an Func to a Func delegate.
		/// </summary>
		/// <param name="source">The, TResult> source Func.</param>
		/// <returns>A Func wrapper for the Func.</returns>
		/// <typeparam name="T">The first argument type.</typeparam>
		/// <typeparam name="TResult">The return type of the new Func delegate.</typeparam>
		public static Action<T> AsAction<T, TResult>(this Func<T, TResult> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			return (t) => source(t);
		}

		/// <summary>
		/// Extension method that converts an Func to a Func delegate.
		/// </summary>
		/// <param name="source">The, TResult> source Func.</param>
		/// <returns>A Func wrapper for the Func.</returns>
		/// <typeparam name="T1">The first argument type.</typeparam>
		/// <typeparam name="T2">The second argument type.</typeparam>
		/// <typeparam name="TResult">The return type of the new Func delegate.</typeparam>
		public static Action<T1, T2> AsAction<T1, T2, TResult>(this Func<T1, T2, TResult> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			return (t1, t2) => source(t1, t2);
		}

		/// <summary>
		/// Extension method that converts an Func to a Func delegate.
		/// </summary>
		/// <param name="source">The, TResult> source Func.</param>
		/// <returns>A Func wrapper for the Func.</returns>
		/// <typeparam name="T1">The first argument type.</typeparam>
		/// <typeparam name="T2">The second argument type.</typeparam>
		/// <typeparam name="T3">The third argument type.</typeparam>
		/// <typeparam name="TResult">The return type of the new Func delegate.</typeparam>
		public static Action<T1, T2, T3> AsAction<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			return (t1, t2, t3) => source(t1, t2, t3);
		}

		/// <summary>
		/// Extension method that converts an Func to a Func delegate.
		/// </summary>
		/// <param name="source">The, TResult> source Func.</param>
		/// <returns>A Func wrapper for the Func.</returns>
		/// <typeparam name="T1">The first argument type.</typeparam>
		/// <typeparam name="T2">The second argument type.</typeparam>
		/// <typeparam name="T3">The third argument type.</typeparam>
		/// <typeparam name="T4">The fourth argument type.</typeparam>
		/// <typeparam name="TResult">The return type of the new Func delegate.</typeparam>
		public static Action<T1, T2, T3, T4> AsAction<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			return (t1, t2, t3, t4) => source(t1, t2, t3, t4); 
		}
	}
}
