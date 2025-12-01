using Diquis.Application.Common.Filter;

namespace Diquis.Application.Services.DivisionService.Filters
{
    /// <summary>
    /// Represents a filter for paginating and searching division table data.
    /// Inherits pagination and sorting properties from <see cref="PaginationFilter"/>.
    /// </summary>
    public class DivisionTableFilter : PaginationFilter
    {
        /// <summary>
        /// Gets or sets the keyword used to filter division records.
        /// </summary>
        public string? Keyword { get; set; }
    }
}
