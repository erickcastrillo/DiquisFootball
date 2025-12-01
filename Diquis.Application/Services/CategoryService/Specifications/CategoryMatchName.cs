using Ardalis.Specification;
using Diquis.Domain.Entities.Football.Common;

namespace Diquis.Application.Services.CategoryService.Specifications
{
    /// <summary>
    /// Specification for filtering <see cref="Category"/> entities by name and ordering by name.
    /// </summary>
    public class CategoryMatchName : Specification<Category>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryMatchName"/> class.
        /// Filters categories by the specified name if provided, and orders the results by name.
        /// </summary>
        /// <param name="name">
        /// The name to match. If <c>null</c> or whitespace, no filtering is applied.
        /// </param>
        public CategoryMatchName(string? name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                _ = Query.Where(h => h.Name == name);
            }
            _ = Query.OrderBy(h => h.Name);
        }
    }
}
