using Diquis.Application.Common.Filter;

namespace Diquis.Application.Services.ProductService.Filters
{
    public class ProductTableFilter : PaginationFilter
    {
        public string? Keyword { get; set; }
    }
}
