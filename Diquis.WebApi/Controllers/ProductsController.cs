using Diquis.Application.Common.Wrapper;
using Diquis.Application.Services.ProductService;
using Diquis.Application.Services.ProductService.DTOs;
using Diquis.Application.Services.ProductService.Filters;
using Diquis.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Diquis.WebApi.Controllers
{
    /// <summary>
    /// API controller for managing products. Provides CRUD operations and export features.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductsController"/> class.
        /// </summary>
        /// <param name="productService">The product service.</param>
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Gets the full list of products, optionally filtered by a keyword.
        /// </summary>
        /// <param name="keyword">The search keyword (optional).</param>
        /// <param name="acceptLanguage">The Accept-Language header to determine the locale (optional).</param>
        /// <returns>A list of products.</returns>
        /// <remarks>
        /// Retrieves all products. Optionally, filter by keyword in the product name.<br/>
        /// <b>Sample request:</b> GET /api/Products?keyword=example<br/>
        /// <b>Authorization:</b> Requires roles: root, admin, editor, basic.
        /// </remarks>
        /// <response code="200">Returns the list of products.</response>
        /// <response code="400">If the request is invalid.</response>
        /// <response code="401">If the user is unauthorized.</response>
        /// <response code="403">If the user does not have permission.</response>
        /// <response code="404">If no products are found.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [Authorize(Roles = "root, admin, editor, basic")]
        [HttpGet]
        public async Task<IActionResult> GetProductsAsync(string keyword = "", [FromHeader(Name = "Accept-Language")] string? acceptLanguage = "es-ES")
        {
            Locale locale = ParseLocale(acceptLanguage);
            Response<IEnumerable<ProductDTO>> products = await _productService.GetProductsAsync(keyword, locale);
            return Ok(products);
        }

        /// <summary>
        /// Gets a paginated and filtered list of products for Tanstack Table v8.
        /// </summary>
        /// <param name="filter">The product table filter.</param>
        /// <param name="acceptLanguage">The Accept-Language header to determine the locale (optional).</param>
        /// <returns>A paginated response of products.</returns>
        /// <remarks>
        /// Retrieves a paginated list of products based on the provided filter.<br/>
        /// <b>Sample request:</b> <br/>
        /// POST /api/Products/products-paginated<br/>
        /// <pre>{
        ///   "keyword": "example",
        ///   "pageNumber": 1,
        ///   "pageSize": 10,
        ///   "sorting": []
        /// }</pre>
        /// <b>Authorization:</b> Requires roles: root, admin, editor, basic.
        /// </remarks>
        /// <response code="200">Returns the paginated list of products.</response>
        /// <response code="400">If the filter is invalid.</response>
        /// <response code="401">If the user is unauthorized.</response>
        /// <response code="403">If the user does not have permission.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [Authorize(Roles = "root, admin, editor, basic")]
        [HttpPost("products-paginated")]
        public async Task<IActionResult> GetProductsPaginatedAsync(ProductTableFilter filter, [FromHeader(Name = "Accept-Language")] string? acceptLanguage = "es-ES")
        {
            Locale locale = ParseLocale(acceptLanguage);
            PaginatedResponse<ProductDTO> products = await _productService.GetProductsPaginatedAsync(filter, locale);
            return Ok(products);
        }

        /// <summary>
        /// Gets a single product by its identifier.
        /// </summary>
        /// <param name="id">The product identifier.</param>
        /// <returns>The product details.</returns>
        /// <remarks>
        /// Retrieves a product by its unique identifier.<br/>
        /// <b>Sample request:</b> GET /api/Products/{id}<br/>
        /// <b>Authorization:</b> Requires roles: root, admin, editor, basic.
        /// </remarks>
        /// <response code="200">Returns the product details.</response>
        /// <response code="400">If the id is invalid.</response>
        /// <response code="401">If the user is unauthorized.</response>
        /// <response code="403">If the user does not have permission.</response>
        /// <response code="404">If the product is not found.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [Authorize(Roles = "root, admin, editor, basic")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductAsync(Guid id)
        {
            Response<ProductDTO> product = await _productService.GetProductAsync(id);
            return Ok(product);
        }

        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <param name="request">The create product request.</param>
        /// <returns>The identifier of the created product.</returns>
        /// <remarks>
        /// Creates a new product with the provided data.<br/>
        /// <b>Sample request:</b> <br/>
        /// POST /api/Products<br/>
        /// <pre>{
        ///   "name": "Sample Product",
        ///   "description": "A sample product description."
        /// }</pre>
        /// <b>Authorization:</b> Requires roles: root, admin, editor.
        /// </remarks>
        /// <response code="200">Returns the ID of the newly created product.</response>
        /// <response code="400">If the request data is invalid.</response>
        /// <response code="401">If the user is unauthorized.</response>
        /// <response code="403">If the user does not have permission.</response>
        /// <response code="409">If a product with the same name already exists.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [Authorize(Roles = "root, admin, editor")]
        [HttpPost]
        public async Task<IActionResult> CreateProductAsync(CreateProductRequest request)
        {
            try
            {
                Response<Guid> result = await _productService.CreateProductAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Updates an existing product.
        /// </summary>
        /// <param name="request">The update product request.</param>
        /// <param name="id">The product identifier.</param>
        /// <returns>The identifier of the updated product.</returns>
        /// <remarks>
        /// Updates the product with the specified ID using the provided data.<br/>
        /// <b>Sample request:</b> <br/>
        /// PUT /api/Products/{id}<br/>
        /// <pre>{
        ///   "name": "Updated Product Name",
        ///   "description": "Updated product description."
        /// }</pre>
        /// <b>Authorization:</b> Requires roles: root, admin, editor.
        /// </remarks>
        /// <response code="200">Returns the ID of the updated product.</response>
        /// <response code="400">If the request data or id is invalid.</response>
        /// <response code="401">If the user is unauthorized.</response>
        /// <response code="403">If the user does not have permission.</response>
        /// <response code="404">If the product is not found.</response>
        /// <response code="409">If a product with the same name already exists.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [Authorize(Roles = "root,admin, editor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductAsync(UpdateProductRequest request, Guid id)
        {
            try
            {
                Response<Guid> result = await _productService.UpdateProductAsync(request, id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a product by its identifier.
        /// </summary>
        /// <param name="id">The product identifier.</param>
        /// <returns>The identifier of the deleted product.</returns>
        /// <remarks>
        /// Deletes the product with the specified ID.<br/>
        /// <b>Sample request:</b> DELETE /api/Products/{id}<br/>
        /// <b>Authorization:</b> Requires roles: root, admin, editor.
        /// </remarks>
        /// <response code="200">Returns the ID of the deleted product.</response>
        /// <response code="400">If the id is invalid.</response>
        /// <response code="401">If the user is unauthorized.</response>
        /// <response code="403">If the user does not have permission.</response>
        /// <response code="404">If the product is not found.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [Authorize(Roles = "root,admin, editor")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductAsync(Guid id)
        {
            try
            {
                Response<Guid> productId = await _productService.DeleteProductAsync(id);
                return Ok(productId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Exports all products to an Excel file.
        /// </summary>
        /// <returns>The exported Excel file as a byte array.</returns>
        /// <remarks>
        /// Exports all products to an Excel file.<br/>
        /// <b>Sample request:</b> POST /api/Products/export<br/>
        /// <pre>{}</pre>
        /// <b>Authorization:</b> Requires roles: root, admin, editor.
        /// </remarks>
        /// <response code="200">Returns the exported Excel file.</response>
        /// <response code="400">If the export fails.</response>
        /// <response code="401">If the user is unauthorized.</response>
        /// <response code="403">If the user does not have permission.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [Authorize(Roles = "root, admin, editor")]
        [HttpPost("export")]
        public async Task<IActionResult> ExportProducts()
        {
            Response<byte[]> result = await _productService.GetProductsExportAsync();

            if (!result.Succeeded || result.Data == null)
                return BadRequest(result.Messages);

            return File(result.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Products.xlsx");
        }

        /// <summary>
        /// Gets a PDF export of a product by its identifier.
        /// </summary>
        /// <param name="id">The product identifier.</param>
        /// <returns>The product invoice as a PDF file.</returns>
        /// <remarks>
        /// Exports a product invoice as a PDF file.<br/>
        /// <b>Sample request:</b> POST /api/Products/pdf/{id}<br/>
        /// <pre>{}</pre>
        /// <b>Authorization:</b> Requires roles: root, admin, editor.
        /// </remarks>
        /// <response code="200">Returns the product invoice as a PDF file.</response>
        /// <response code="400">If the export fails or id is invalid.</response>
        /// <response code="401">If the user is unauthorized.</response>
        /// <response code="403">If the user does not have permission.</response>
        /// <response code="404">If the product is not found.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [Authorize(Roles = "root, admin, editor")]
        [HttpPost("pdf/{id}")]
        public async Task<IActionResult> GetProductPdfAsync(Guid id)
        {
            Response<byte[]> result = await _productService.GetProductPdfAsync(id);

            return !result.Succeeded || result.Data == null
                ? BadRequest(result.Messages)
                : File(result.Data, "application/pdf", "ProductInvoice.pdf");
        }

        private Locale ParseLocale(string? acceptLanguage)
        {
            if (string.IsNullOrEmpty(acceptLanguage))
            {
                return Locale.Es; // Default to Spanish if header is missing
            }

            if (acceptLanguage.StartsWith("es", StringComparison.OrdinalIgnoreCase))
            {
                return Locale.Es;
            }
            // Add more cases as needed for other languages
            // For now, default to English if not Spanish
            return Locale.En;
        }
    }
}
