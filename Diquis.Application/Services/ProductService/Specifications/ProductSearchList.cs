using Ardalis.Specification;
using Diquis.Domain.Entities.Catalog;
using Diquis.Domain.Enums;

namespace Diquis.Application.Services.ProductService.Specifications
{
    /// <summary>
    /// Specification for searching and listing <see cref="Product"/> entities.
    /// Applies optional keyword filtering on the product name and sorts results by creation date descending.
    /// </summary>
    public class ProductSearchList : Specification<Product>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductSearchList"/> class.
        /// </summary>
        /// <param name="keyword">
        /// Optional keyword to filter products by name. If null or whitespace, no filtering is applied.
        /// </param>
        /// <param name="locale">
        /// The locale to filter products by.
        /// </param>
        public ProductSearchList(string? keyword = "", Locale locale = Locale.Es)
        {
            // filters
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                _ = Query.Where(x => x.Name.Contains(keyword));
            }

            _ = Query.Where(x => x.Locale == locale); // Filter by locale

            _ = Query.OrderByDescending(x => x.CreatedOn); // default sort order
        }
    }
}
