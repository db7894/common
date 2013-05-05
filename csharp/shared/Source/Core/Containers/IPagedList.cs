
namespace SharedAssemblies.Core.Containers
{
	/// <summary>
	/// Interface for a custom paged list collection, borrowed from ASP.NET MVC's PagedList. 
    /// Use this interface to force custom collections to adhere to the PagedList concepts.
	/// </summary>
	public interface IPagedList
	{
		/// <summary>
		/// Gets or sets the total item count.
		/// </summary>
		/// <value>The total item count.</value>
		int TotalCount { get; }

		/// <summary>
		/// Gets or sets the page index.
		/// </summary>
		/// <value>The page index.</value>
		int PageIndex { get; }

		/// <summary>
		/// Gets or sets the size of the page (number of items per page).
		/// </summary>
		/// <value>The size of the page.</value>
		int PageSize { get; }

        /// <summary>
        /// Gets or sets the page count.
        /// </summary>
        /// <value>The page count.</value>
        int PageCount { get; }

        /// <summary>
        /// Gets the page number.
        /// </summary>
        /// <value>The page number.</value>
        int PageNumber { get; }

		/// <summary>
		/// Gets a value indicating whether there is a previous page.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance has a previous page; otherwise, <c>false</c>.
		/// </value>
		bool HasPreviousPage { get; }

		/// <summary>
		/// Gets a value indicating whether there is a next page.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance has a next page; otherwise, <c>false</c>.
		/// </value>
		bool HasNextPage { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is the first page.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is the first page; otherwise, <c>false</c>.
        /// </value>
        bool IsFirstPage { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is the last page.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is the last page; otherwise, <c>false</c>.
        /// </value>
        bool IsLastPage { get; }
	}
}
