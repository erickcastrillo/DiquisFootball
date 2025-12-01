using Diquis.Application.Common.Filter;

namespace Diquis.Application.Services.ProductService.Filters
{
    /// <summary>
    /// Represents a filter for paginated product table queries, including keyword search.
    /// Inherits pagination and sorting properties from <see cref="PaginationFilter"/>.
    /// </summary>
    public class ProductTableFilter : PaginationFilter
    {
        /// <summary>
        /// Gets or sets the keyword used to filter products by name or description.
        /// </summary>
        public string? Keyword { get; set; }
    }
}
