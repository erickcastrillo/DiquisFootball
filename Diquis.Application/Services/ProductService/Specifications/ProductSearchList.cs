using Ardalis.Specification;
using Diquis.Domain.Entities.Catalog;

namespace Diquis.Application.Services.ProductService.Specifications
{
    public class ProductSearchList : Specification<Product>
    {
        public ProductSearchList(string? keyword = "")
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
