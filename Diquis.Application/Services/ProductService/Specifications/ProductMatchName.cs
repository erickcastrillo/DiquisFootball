using Ardalis.Specification;
using Diquis.Domain.Entities.Catalog;

namespace Diquis.Application.Services.ProductService.Specifications
{
    public class ProductMatchName : Specification<Product>
    {
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
