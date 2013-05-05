using System;
using System.Collections.Generic;
using SharedAssemblies.Core.Extensions;
using SharedAssemblies.Core.Containers;


namespace SharedAssemblies.General.Memoizers
{
	/// <summary>
	/// Represents a call to a method given the method name and the arguments to the method call.
	/// </summary>
	internal sealed class MethodCall : IEquatable<MethodCall>
	{
		// static comparer to compare all arrays
		private static readonly EqualityComparer<string> _methodNameComparer = EqualityComparer<string>.Default;
		private static readonly ArrayComparer<object> _argumentComparer = new ArrayComparer<object>();

		/// <summary>
		/// Gets or sets the name of the method being called.
		/// </summary>
		public string MethodName { get; set; }

		/// <summary>
		/// Gets or sets the arguments to the method call.
		/// </summary>
		public object[] Arguments { get; set; }

		/// <summary>
		/// Compares two MethodCalls to see if they refer to the same call. 
		/// </summary>
		/// <param name="call">The second call to compare to this call.</param>
		/// <returns>True if both method calls are the same.</returns>
		public bool Equals(MethodCall call)
		{
			// two method calls are the same if their method names are the same
			// and the arguments supplied are the same.
			return _methodNameComparer.Equals(MethodName, call.MethodName) &&
					_argumentComparer.Equals(Arguments, call.Arguments);
		}

		/// <summary>
		/// Compares a MethodCall to another object to see if they refer to the 
		/// same method call.
		/// </summary>
		/// <param name="obj">The object to compare the MethodCall to.</param>
		/// <returns>True if they both refer to the same MethodCall.</returns>
		public override bool Equals(object obj)
		{
			// call IEquatable<MethodCall>.Equals() instead
			return obj is MethodCall ? Equals((MethodCall)obj) : false;
		}

		/// <summary>
		/// Gets the hash code for the method call by combining the hash code of the 
		/// MethodName and the Arguments.
		/// </summary>
		/// <remarks>
		/// See Jon Skeet (C# MVP) response in the StackOverflow thread 
		/// http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
		/// </remarks>
		/// <returns>The hash code for the MethodCall.</returns>
		public override int GetHashCode()
		{
			unchecked
			{
				int hash = 23;

				hash = 17 * hash + _methodNameComparer.GetHashCode(MethodName);
				hash = 17 * hash + _argumentComparer.GetHashCode(Arguments);

				return hash;
			}
		}

		/// <summary>
		/// Serializes the MethodCall by creating a string with the method name and arguments.
		/// </summary>
		/// <returns>String representation of the MethodCall.</returns>
		public override string ToString()
		{
			return string.Format("[MethodCall - Name: [{0}], Arguments: [{1}]]",
				MethodName, Arguments.Summarize(10));
		}
	}
}
