using Diquis.Application.Common.Filter;

namespace Diquis.Application.Services.CategoryService.Filters
{
    /// <summary>
    /// Represents a filter for paginated category table queries, including keyword search.
    /// Inherits pagination and sorting properties from <see cref="PaginationFilter"/>.
    /// </summary>
    public class CategoryTableFilter : PaginationFilter
    {
        /// <summary>
        /// Gets or sets the keyword used to filter categories by name or description.
        /// </summary>
        public string? Keyword { get; set; }
    }
}
