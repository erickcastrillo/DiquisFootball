using Ardalis.Specification;
using Diquis.Application.Common.Specification;
using Diquis.Domain.Entities.Football.Common;

namespace Diquis.Application.Services.CategoryService.Specifications
{
    /// <summary>
    /// Specification for searching and sorting <see cref="Category"/> entities for table views.
    /// Supports filtering by name and dynamic or default ordering.
    /// </summary>
    public class CategorySearchTable : Specification<Category>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CategorySearchTable"/> class.
        /// </summary>
        /// <param name="name">
        /// Optional. The name or partial name to filter categories by. If null or whitespace, no name filter is applied.
        /// </param>
        /// <param name="dynamicOrder">
        /// Optional. The dynamic order string (e.g., "Name", "CreatedOn desc") for sorting.
        /// If null or empty, defaults to descending order by <see cref="Category.CreatedOn"/>.
        /// </param>
        public CategorySearchTable(string? name = "", string? dynamicOrder = "")
        {
            // filters
            if (!string.IsNullOrWhiteSpace(name))
            {
                _ = Query.Where(x => x.Name.Contains(name));
            }

            // sort order
            if (string.IsNullOrEmpty(dynamicOrder))
            {
                _ = Query.OrderByDescending(x => x.CreatedOn); // default sort order
            }
            else
            {
                _ = Query.OrderBy(dynamicOrder); // dynamic (JQDT) sort order
            }
        }
    }
}
