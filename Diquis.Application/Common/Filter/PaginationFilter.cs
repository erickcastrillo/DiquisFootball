using System.Collections.Generic;

namespace Diquis.Application.Common.Filter
{
    /// <summary>
    /// Represents a server-side pagination filter, specific to Tanstack Table v8 conventions (React, Vue).
    /// <see href="https://tanstack.com/table/v8">Tanstack Table v8 documentation</see>
    /// </summary>
    public class PaginationFilter
    {
        /// <summary>
        /// Gets or sets the current page number.
        /// </summary>
        public int PageNumber { get; set; }
        /// <summary>
        /// Gets or sets the number of items per page.
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// Gets or sets the list of column sorting orders.
        /// </summary>
        public List<TanstackColumnOrder> Sorting { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="PaginationFilter"/> class.
        /// </summary>
        public PaginationFilter()
        {
            PageNumber = 1;
            PageSize = 10;
            Sorting = new List<TanstackColumnOrder>();
        }
    }
    /// <summary>
    /// Represents the sorting order for a single column in Tanstack Table.
    /// </summary>
    public class TanstackColumnOrder
    {
        /// <summary>
        /// Gets or sets the identifier of the column to sort by.
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the column should be sorted in descending order.
        /// </summary>
        public bool Desc { get; set; }
    }
}
