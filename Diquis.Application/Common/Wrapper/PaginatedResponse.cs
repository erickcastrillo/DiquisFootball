using System.Collections.Generic;

namespace Diquis.Application.Common.Wrapper
{
    /// <summary>
    /// Represents a paginated response for server-side table data.
    /// </summary>
    /// <typeparam name="T">The type of the data items.</typeparam>
    public class PaginatedResponse<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaginatedResponse{T}"/> class.
        /// </summary>
        /// <param name="data">The data items for the current page.</param>
        /// <param name="count">The total number of items.</param>
        /// <param name="page">The current page number.</param>
        /// <param name="pageSize">The size of each page.</param>
        public PaginatedResponse(List<T> data, int count, int page, int pageSize)
        {
            Data = data;
            CurrentPage = page;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            TotalCount = count;
        }

        /// <summary>
        /// The data items for the current page.
        /// </summary>
        public List<T> Data { get; set; }

        /// <summary>
        /// The current page number.
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// The total number of pages.
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// The total number of items.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// The size of each page.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Indicates if there is a previous page.
        /// </summary>
        public bool HasPreviousPage => CurrentPage > 1;

        /// <summary>
        /// Indicates if there is a next page.
        /// </summary>
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
