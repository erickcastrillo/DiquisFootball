using Diquis.Application.Common.Marker;
using Diquis.Application.Common.Wrapper;
using Diquis.Application.Services.ProductService.DTOs;
using Diquis.Application.Services.ProductService.Filters;

namespace Diquis.Application.Services.ProductService
{
    /// <summary>
    /// Service interface for product-related operations.
    /// </summary>
    public interface IProductService : IScopedService
    {
        /// <summary>
        /// Gets a list of products matching the specified keyword.
        /// </summary>
        /// <param name="keyword">The search keyword.</param>
        /// <returns>A response containing a list of product DTOs.</returns>
        Task<Response<IEnumerable<ProductDTO>>> GetProductsAsync(string keyword = "");

        /// <summary>
        /// Gets a paginated list of products based on the provided filter.
        /// </summary>
        /// <param name="filter">The product table filter.</param>
        /// <returns>A paginated response containing product DTOs.</returns>
        Task<PaginatedResponse<ProductDTO>> GetProductsPaginatedAsync(ProductTableFilter filter);

        /// <summary>
        /// Gets a product by its identifier.
        /// </summary>
        /// <param name="id">The product identifier.</param>
        /// <returns>A response containing the product DTO.</returns>
        Task<Response<ProductDTO>> GetProductAsync(Guid id);

        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <param name="request">The create product request.</param>
        /// <returns>A response containing the identifier of the created product.</returns>
        Task<Response<Guid>> CreateProductAsync(CreateProductRequest request);

        /// <summary>
        /// Updates an existing product.
        /// </summary>
        /// <param name="request">The update product request.</param>
        /// <param name="id">The product identifier.</param>
        /// <returns>A response containing the identifier of the updated product.</returns>
        Task<Response<Guid>> UpdateProductAsync(UpdateProductRequest request, Guid id);

        /// <summary>
        /// Deletes a product by its identifier.
        /// </summary>
        /// <param name="id">The product identifier.</param>
        /// <returns>A response containing the identifier of the deleted product.</returns>
        Task<Response<Guid>> DeleteProductAsync(Guid id);

        /// <summary>
        /// Gets an export of all products as a byte array.
        /// </summary>
        /// <returns>A response containing the exported data as a byte array.</returns>
        Task<Response<byte[]>> GetProductsExportAsync();

        /// <summary>
        /// Gets a PDF export of a product by its identifier.
        /// </summary>
        /// <param name="id">The product identifier.</param>
        /// <returns>A response containing the PDF data as a byte array.</returns>
        Task<Response<byte[]>> GetProductPdfAsync(Guid id);
    }
}
