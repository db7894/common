using System;
using System.ComponentModel;

namespace Bashwork.General.Validation.Internal
{
	/// <summary>
	/// From Kzu's blog: http://www.clariusconsulting.net/blogs/kzu/archive/2008/03/10/58301.aspx 
	/// </summary>
	/// <remarks>This will only work on projects outside of this solution</remarks>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface ICleanFluentInterface
	{
		/// <summary>
		/// Intentionally blocked from the fluent interface 
		/// </summary>
		/// <returns>This return should not be used</returns>
		[EditorBrowsable(EditorBrowsableState.Never)]
		Type GetType();

		/// <summary>
		/// Intentionally blocked from the fluent interface 
		/// </summary>
		/// <returns>This return should not be used</returns>
		[EditorBrowsable(EditorBrowsableState.Never)]
		int GetHashCode();

		/// <summary>
		/// Intentionally blocked from the fluent interface 
		/// </summary>
		/// <returns>This return should not be used</returns>
		[EditorBrowsable(EditorBrowsableState.Never)]
		string ToString();

		/// <summary>
		/// Intentionally blocked from the fluent interface 
		/// </summary>
		/// <param name="obj">This input is unused</param>
		/// <returns>This return should not be used</returns>
		[EditorBrowsable(EditorBrowsableState.Never)]
		bool Equals(object obj);
	}
}