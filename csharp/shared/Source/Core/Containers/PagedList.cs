using System;
using System.Collections.Generic;
using System.Linq;

namespace SharedAssemblies.Core.Containers
{
	/// <summary>
	/// PagedList class subclassess from List&lt;T&gt; and contains properties to maintain a paged
	/// collection of type T.
	/// </summary>
	/// <typeparam name="T">The type of object in the List.</typeparam>
	public class PagedList<T> : List<T>, IPagedList
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PagedList&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="index">The index.</param>
		/// <param name="pageSize">Size of the page.</param>
		public PagedList(IEnumerable<T> source, int index, int pageSize) 
			: this (source == null ? new List<T>().AsQueryable() : source.AsQueryable(), index, pageSize)
		{
				
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PagedList&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="index">The index.</param>
		/// <param name="pageSize">Size of the page.</param>
		public PagedList(IQueryable<T> source, int index, int pageSize)
		{
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", index, "PageIndex cannot be less than 0.");
			}
			if (pageSize < 1)
			{
				throw new ArgumentOutOfRangeException("pageSize", pageSize, "PageSize cannot be less than 1.");
			}

			TotalCount = source.Count();
			PageSize = pageSize;
			PageCount = TotalCount > 0 ? (int)System.Math.Ceiling(TotalCount / (double)PageSize) : 0;
			
			// Ensure we don't request an index out of range of our pages.
			if ((index + 1) > PageCount)
			{
				index = 0;
			}

			PageIndex = index;            

			AddRange(source.Skip(index * pageSize).Take(pageSize).ToList());
		}

		/// <summary>
		/// Gets or sets the total item count.
		/// </summary>
		/// <value>The total item count.</value>
		public int TotalCount { get; protected set; }

		/// <summary>
		/// Gets or sets the page index.
		/// </summary>
		/// <value>The page index.</value>
		public int PageIndex { get; protected set; }

		/// <summary>
		/// Gets or sets the size of the page (number of items per page).
		/// </summary>
		/// <value>The size of the page.</value>
		public int PageSize { get; set; }

		/// <summary>
		/// Gets or sets the page count.
		/// </summary>
		/// <value>The page count.</value>
		public int PageCount { get; protected set; }

		/// <summary>
		/// Gets the page number.
		/// </summary>
		/// <value>The page number.</value>
		public int PageNumber
		{
			get { return PageIndex + 1; }
		}

		/// <summary>
		/// Gets a value indicating whether there is a previous page.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance has a previous page; otherwise, <c>false</c>.
		/// </value>
		public bool HasPreviousPage
		{
			get { return (PageIndex > 0); }
		}

		/// <summary>
		/// Gets a value indicating whether there is a next page.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance has a next page; otherwise, <c>false</c>.
		/// </value>
		public bool HasNextPage
		{
			get { return ((PageIndex + 1) * PageSize) < TotalCount; }
		}

		/// <summary>
		/// Gets a value indicating whether this instance is first page.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is first page; otherwise, <c>false</c>.
		/// </value>
		public bool IsFirstPage
		{
			get { return PageIndex <= 0; }
		}

		/// <summary>
		/// Gets a value indicating whether this instance is last page.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is last page; otherwise, <c>false</c>.
		/// </value>
		public bool IsLastPage
		{
			get { return PageNumber >= PageCount; }
		}        	
	}
}
