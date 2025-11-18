using AutoMapper;
using Diquis.Application.Common;
using Diquis.Application.Common.ExcelExport;
using Diquis.Application.Common.PdfExport;
using Diquis.Application.Common.Wrapper;
using Diquis.Application.Services.ProductService.DTOs;
using Diquis.Application.Services.ProductService.Filters;
using Diquis.Application.Services.ProductService.Specifications;
using Diquis.Application.Utility;
using Diquis.Domain.Entities.Catalog;

namespace Diquis.Application.Services.ProductService
{
    // sample application service with CRUD operations -- use as a guide for creating your own services
    public class ProductService : IProductService
    {
        private readonly IRepositoryAsync _repository;
        private readonly IMapper _mapper;
        private readonly IExcelExportService _excelExportService;
        private readonly IPdfExportService _pdfExportService;

        public ProductService(IRepositoryAsync repository, IMapper mapper, IExcelExportService excelExportService, IPdfExportService pdfExportService)
        {
            _repository = repository; // inject repository 
            _mapper = mapper; // inject mapper
            _excelExportService = excelExportService;
            _pdfExportService = pdfExportService;
        }
        // get full List
        public async Task<Response<IEnumerable<ProductDTO>>> GetProductsAsync(string keyword = "")
        {
            ProductSearchList specification = new(keyword); // ardalis specification
            IEnumerable<ProductDTO> list = await _repository.GetListAsync<Product, ProductDTO, Guid>(specification); // full list, entity mapped to dto
            return Response<IEnumerable<ProductDTO>>.Success(list);
        }
        // get Tanstack Table paginated list (as seen in the React and Vue project tables)
        public async Task<PaginatedResponse<ProductDTO>> GetProductsPaginatedAsync(ProductTableFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.Keyword)) // set to first page if any search filters are applied
            {
                filter.PageNumber = 1;
            }

            string dynamicOrder = (filter.Sorting != null) ? NanoHelpers.GenerateOrderByString(filter) : ""; // possible dynamic ordering from datatable
            ProductSearchTable specification = new(filter?.Keyword, dynamicOrder); // ardalis specification
            PaginatedResponse<ProductDTO> pagedResponse = await _repository.GetPaginatedResultsAsync<Product, ProductDTO, Guid>(filter.PageNumber, filter.PageSize, specification); // paginated response, entity mapped to dto
            return pagedResponse;
        }
        // get single product by Id 
        public async Task<Response<ProductDTO>> GetProductAsync(Guid id)
        {
            try
            {
                ProductDTO dto = await _repository.GetByIdAsync<Product, ProductDTO, Guid>(id);
                return Response<ProductDTO>.Success(dto);
            }
            catch (Exception ex)
            {
                return Response<ProductDTO>.Fail(ex.Message);
            }
        }

        // create new product
        public async Task<Response<Guid>> CreateProductAsync(CreateProductRequest request)
        {
            ProductMatchName specification = new(request.Name); // ardalis specification 
            bool productExists = await _repository.ExistsAsync<Product, Guid>(specification);
            if (productExists)
            {
                return Response<Guid>.Fail("Product already exists");
            }

            Product newProduct = _mapper.Map(request, new Product()); // map dto to domain entity

            try
            {
                Product response = await _repository.CreateAsync<Product, Guid>(newProduct); // create new entity 
                _ = await _repository.SaveChangesAsync(); // save changes to db
                return Response<Guid>.Success(response.Id); // return id
            }
            catch (Exception ex)
            {
                return Response<Guid>.Fail(ex.Message);
            }
        }

        // update product
        public async Task<Response<Guid>> UpdateProductAsync(UpdateProductRequest request, Guid id)
        {
            Product productInDb = await _repository.GetByIdAsync<Product, Guid>(id); // get existing entity
            if (productInDb == null)
            {
                return Response<Guid>.Fail("Not Found");
            }

            Product updatedProduct = _mapper.Map(request, productInDb); // map dto to domain entity

            try
            {
                Product response = await _repository.UpdateAsync<Product, Guid>(updatedProduct);  // update entity 
                _ = await _repository.SaveChangesAsync(); // save changes to db
                return Response<Guid>.Success(response.Id); // return id
            }
            catch (Exception ex)
            {
                return Response<Guid>.Fail(ex.Message);
            }
        }

        // delete product
        public async Task<Response<Guid>> DeleteProductAsync(Guid id)
        {
            try
            {
                Product? product = await _repository.RemoveByIdAsync<Product, Guid>(id);
                _ = await _repository.SaveChangesAsync();

                return Response<Guid>.Success(product.Id);
            }
            catch (Exception ex)
            {
                return Response<Guid>.Fail(ex.Message);
            }
        }
        // export products
        public async Task<Response<byte[]>> GetProductsExportAsync()
        {
            try
            {
                ProductSearchList specification = new(); // ardalis specification
                IEnumerable<ProductDTO> productList = await _repository.GetListAsync<Product, ProductDTO, Guid>(specification); // full list, entity mapped to dto


                Dictionary<string, string> columnMapping = new()
                {
                    { "Name", "Name" },
                    { "Description", "Description" },
                    { "Id", "Id" },
                    { "CreatedOn", "Created On" },
                };

                byte[] excelBytes = _excelExportService.ExportToExcel(productList, columnMapping);
                return Response<byte[]>.Success(excelBytes);
            }
            catch (Exception ex)
            {
                return Response<byte[]>.Fail(ex.Message);
            }
        }

        public async Task<Response<byte[]>> GetProductPdfAsync(Guid id)
        {
            try
            {
                ProductDTO product = await _repository.GetByIdAsync<Product, ProductDTO, Guid>(id);
                if (product == null)
                {
                    return Response<byte[]>.Fail("Product not found");
                }

                byte[] pdfBytes = await _pdfExportService.Export(product);
                return Response<byte[]>.Success(pdfBytes);
            }
            catch (Exception ex)
            {
                return Response<byte[]>.Fail(ex.Message);
            }
        }
    }
}

