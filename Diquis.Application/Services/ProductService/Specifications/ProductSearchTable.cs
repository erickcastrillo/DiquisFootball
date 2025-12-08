using Ardalis.Specification;
using Diquis.Application.Common.Specification;
using Diquis.Domain.Entities.Catalog;
using Diquis.Domain.Enums;

namespace Diquis.Application.Services.ProductService.Specifications
{
    /// <summary>
    /// Specification for searching and sorting products for table display.
    /// </summary>
    public class ProductSearchTable : Specification<Product>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductSearchTable"/> class with optional name filter and dynamic order.
        /// </summary>
        /// <param name="name">Optional product name filter.</param>
        /// <param name="dynamicOrder">Optional dynamic order string.</param>
        /// <param name="locale">The locale to filter products by.</param>
        public ProductSearchTable(string? name = "", string? dynamicOrder = "", Locale locale = Locale.Es)
        {
            // filters
            if (!string.IsNullOrWhiteSpace(name))
            {
                _ = Query.Where(x => x.Name.Contains(name));
            }

            _ = Query.Where(x => x.Locale == locale); // Filter by locale

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
