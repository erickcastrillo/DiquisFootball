using AutoMapper;
using Diquis.Application.Services.CategoryService.DTOs;
using Diquis.Application.Common.Wrapper;
using Diquis.Application.Common;
using Diquis.Application.Utility;
using Diquis.Application.Services.CategoryService.Filters;
using Diquis.Application.Services.CategoryService.Specifications;
using Diquis.Domain.Entities.Football.Common;
using Diquis.Domain.Entities.Football.PlayerManagement;

namespace Diquis.Application.Services.CategoryService
{
    /// <summary>
    /// Provides service operations for managing <see cref="Category"/> entities, including retrieval, creation, update, and deletion.
    /// </summary>
    /// <remarks>
    /// This service uses an asynchronous repository pattern and AutoMapper for entity-to-DTO mapping.
    /// It supports full list retrieval, paginated queries for table views, and CRUD operations.
    /// </remarks>
    public class CategoryService : ICategoryService
    {
        private readonly IRepositoryAsync _repository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryService"/> class.
        /// </summary>
        /// <param name="repository">The asynchronous repository for data access.</param>
        /// <param name="mapper">The AutoMapper instance for entity-DTO mapping.</param>
        public CategoryService(IRepositoryAsync repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper; 
        }

        /// <summary>
        /// Retrieves a list of categories, optionally filtered by a keyword.
        /// </summary>
        /// <param name="keyword">The keyword to filter categories by name. Optional.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a <see cref="Response{T}"/>
        /// with an enumerable of <see cref="CategoryDTO"/> objects.
        /// </returns>
        public async Task<Response<IEnumerable<CategoryDTO>>> GetCategoriesAsync(string keyword = "")
        {
            CategorySearchList specification = new(keyword);
            IEnumerable<CategoryDTO> list = await _repository.GetListAsync<Category, CategoryDTO, Guid>(specification);
            return Response<IEnumerable<CategoryDTO>>.Success(list);
        }

        /// <summary>
        /// Retrieves a paginated list of categories based on the provided filter.
        /// </summary>
        /// <param name="filter">The filter criteria for pagination and searching.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a <see cref="PaginatedResponse{T}"/>
        /// with <see cref="CategoryDTO"/> items.
        /// </returns>
        public async Task<PaginatedResponse<CategoryDTO>> GetCategoriesPaginatedAsync(CategoryTableFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                filter.PageNumber = 1;
            }

            string dynamicOrder = (filter.Sorting != null) ? NanoHelpers.GenerateOrderByString(filter) : "";
            CategorySearchTable specification = new(filter?.Keyword, dynamicOrder);
            PaginatedResponse<CategoryDTO> pagedResponse = await _repository.GetPaginatedResultsAsync<Category, CategoryDTO, Guid>(filter.PageNumber, filter.PageSize, specification);
            return pagedResponse;
        }

        /// <summary>
        /// Retrieves a specific category by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the category.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a <see cref="Response{T}"/>
        /// with the <see cref="CategoryDTO"/>.
        /// </returns>
        public async Task<Response<CategoryDTO>> GetCategoryAsync(Guid id)
        {
            try
            {
                CategoryDTO dto = await _repository.GetByIdAsync<Category, CategoryDTO, Guid>(id);
                return Response<CategoryDTO>.Success(dto);
            }
            catch (Exception ex)
            {
                return Response<CategoryDTO>.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Creates a new category.
        /// </summary>
        /// <param name="request">The request object containing the category details.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a <see cref="Response{T}"/>
        /// with the unique identifier of the created category.
        /// </returns>
        public async Task<Response<Guid>> CreateCategoryAsync(CreateCategoryRequest request)
        {
            CategoryMatchName specification = new(request.Name);
            bool CategoryExists = await _repository.ExistsAsync<Category, Guid>(specification);
            if (CategoryExists)
            {
                return Response<Guid>.Fail("Category already exists");
            }

            // Set required properties explicitly
            Category newCategory = new Category
            {
                Name = request.Name,
                Locale = /* TODO: Set appropriate Locale value here */ default(Locale),
                Players = new List<Player>(),
                TenantId = "" // Set as appropriate
            };

            _ = _mapper.Map(request, newCategory);

            try
            {
                Category response = await _repository.CreateAsync<Category, Guid>(newCategory);
                _ = await _repository.SaveChangesAsync();
                return Response<Guid>.Success(response.Id);
            }
            catch (Exception ex)
            {
                return Response<Guid>.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Updates an existing category.
        /// </summary>
        /// <param name="request">The request object containing the updated category details.</param>
        /// <param name="id">The unique identifier of the category to update.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a <see cref="Response{T}"/>
        /// with the unique identifier of the updated category.
        /// </returns>
        public async Task<Response<Guid>> UpdateCategoryAsync(UpdateCategoryRequest request, Guid id)
        {
            Category CategoryInDb = await _repository.GetByIdAsync<Category, Guid>(id);
            if (CategoryInDb == null)
            {
                return Response<Guid>.Fail("Not Found");
            }

            Category updatedCategory = _mapper.Map(request, CategoryInDb);

            try
            {
                Category response = await _repository.UpdateAsync<Category, Guid>(updatedCategory);
                _ = await _repository.SaveChangesAsync();
                return Response<Guid>.Success(response.Id);
            }
            catch (Exception ex)
            {
                return Response<Guid>.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a category by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the category to delete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a <see cref="Response{T}"/>
        /// with the unique identifier of the deleted category.
        /// </returns>
        public async Task<Response<Guid>> DeleteCategoryAsync(Guid id)
        {
            try
            {
                Category? Category = await _repository.RemoveByIdAsync<Category, Guid>(id);
                _ = await _repository.SaveChangesAsync();

                return Response<Guid>.Success(Category.Id);
            }
            catch (Exception ex)
            {
                return Response<Guid>.Fail(ex.Message);
            }
        }
    }
}
