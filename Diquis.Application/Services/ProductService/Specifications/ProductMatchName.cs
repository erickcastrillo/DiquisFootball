using Ardalis.Specification;
using Diquis.Domain.Entities.Catalog;

namespace Diquis.Application.Services.ProductService.Specifications
{
    /// <summary>
    /// Specification for filtering <see cref="Product"/> entities by name and ordering by name.
    /// </summary>
    public class ProductMatchName : Specification<Product>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductMatchName"/> class.
        /// Filters products by the specified name if provided, and orders the results by name.
        /// </summary>
        /// <param name="name">
        /// The name to match. If <c>null</c> or whitespace, no filtering is applied.
        /// </param>
        public ProductMatchName(string? name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                _ = Query.Where(h => h.Name == name);
            }
            _ = Query.OrderBy(h => h.Name);
        }
    }
}
