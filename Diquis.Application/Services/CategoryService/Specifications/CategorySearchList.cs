using Ardalis.Specification;
using Diquis.Domain.Entities.Football.Common;

namespace Diquis.Application.Services.CategoryService.Specifications
{
    /// <summary>
    /// Specification for searching and listing <see cref="Category"/> entities.
    /// Applies optional keyword filtering on the category name and sorts results by creation date descending.
    /// </summary>
    public class CategorySearchList : Specification<Category>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CategorySearchList"/> class.
        /// </summary>
        /// <param name="keyword">
        /// Optional keyword to filter categories by name. If null or whitespace, no filtering is applied.
        /// </param>
        public CategorySearchList(string? keyword = "")
        {
            // filters
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                _ = Query.Where(x => x.Name.Contains(keyword));
            }

            _ = Query.OrderByDescending(x => x.CreatedOn); // default sort order
        }
    }
}
