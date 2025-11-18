using Diquis.Application.Common.Wrapper;
using Diquis.Application.Services.ProductService;
using Diquis.Application.Services.ProductService.DTOs;
using Diquis.Application.Services.ProductService.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Diquis.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase // sample API controller with CRUD operations
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // full list
        [Authorize(Roles = "root, admin, editor, basic")]
        [HttpGet]
        public async Task<IActionResult> GetProductsAsync(string keyword = "")
        {
            Response<IEnumerable<ProductDTO>> products = await _productService.GetProductsAsync(keyword);
            return Ok(products);
        }

        // paginated & filtered list (Tanstack Table v8)
        [Authorize(Roles = "root, admin, editor, basic")]
        [HttpPost("products-paginated")]
        public async Task<IActionResult> GetProductsPaginatedAsync(ProductTableFilter filter)
        {
            PaginatedResponse<ProductDTO> products = await _productService.GetProductsPaginatedAsync(filter);
            return Ok(products);
        }

        // single by Id
        [Authorize(Roles = "root, admin, editor, basic")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductAsync(Guid id)
        {
            Response<ProductDTO> product = await _productService.GetProductAsync(id);
            return Ok(product);
        }

        // create
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

        // update
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

        // delete
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
        [Authorize(Roles = "root, admin, editor")]
        [HttpPost("export")]
        public async Task<IActionResult> ExportProducts()
        {
            Response<byte[]> result = await _productService.GetProductsExportAsync();

            if (!result.Succeeded || result.Data == null)
                return BadRequest(result.Messages);

            return File(result.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Products.xlsx");
        }

        [Authorize(Roles = "root, admin, editor")]
        [HttpPost("pdf/{id}")]
        public async Task<IActionResult> GetProductPdfAsync(Guid id)
        {
            Response<byte[]> result = await _productService.GetProductPdfAsync(id);

            return !result.Succeeded || result.Data == null
                ? BadRequest(result.Messages)
                : File(result.Data, "application/pdf", "ProductInvoice.pdf");
        }
    }
}
