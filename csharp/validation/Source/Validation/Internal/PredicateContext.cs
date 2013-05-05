using System;
using System.Linq.Expressions;

namespace Bashwork.General.Validation.Internal
{
	/// <summary>
	/// A wrapper around a single predicate and various attributes
	/// attached to it.
	/// </summary>
	public sealed class PredicateContext<TObject>
	{
		/// <summary>
		/// The actual predicate that is to be invoked
		/// </summary>
		public Expression<Predicate<TObject>> Predicate;

		/// <summary>
		/// A possible overloaded error message that may be used
		/// </summary>
		public string ErrorMessage;

		/// <summary>
		/// A possible overload of the property name that may be used
		/// </summary>
		public string PropertyName;
	}
}
