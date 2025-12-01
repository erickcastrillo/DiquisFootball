using Diquis.Application.Common.Marker;
using Diquis.Application.Common.Wrapper;
using Diquis.Application.Services.CategoryService.DTOs;
using Diquis.Application.Services.CategoryService.Filters;

namespace Diquis.Application.Services.CategoryService
{
    /// <summary>
    /// Defines the contract for category-related service operations.
    /// </summary>
    public interface ICategoryService : IScopedService
    {
        /// <summary>
        /// Retrieves a list of categories, optionally filtered by a keyword.
        /// </summary>
        /// <param name="keyword">The keyword to filter categories by name. Optional.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a <see cref="Response{T}"/>
        /// with an enumerable of <see cref="CategoryDTO"/> objects.
        /// </returns>
        Task<Response<IEnumerable<CategoryDTO>>> GetCategoriesAsync(string keyword = "");

        /// <summary>
        /// Retrieves a paginated list of categories based on the provided filter.
        /// </summary>
        /// <param name="filter">The filter criteria for pagination and searching.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a <see cref="PaginatedResponse{T}"/>
        /// with <see cref="CategoryDTO"/> items.
        /// </returns>
        Task<PaginatedResponse<CategoryDTO>> GetCategoriesPaginatedAsync(CategoryTableFilter filter);

        /// <summary>
        /// Retrieves a specific category by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the category.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a <see cref="Response{T}"/>
        /// with the <see cref="CategoryDTO"/>.
        /// </returns>
        Task<Response<CategoryDTO>> GetCategoryAsync(Guid id);

        /// <summary>
        /// Creates a new category.
        /// </summary>
        /// <param name="request">The request object containing the category details.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a <see cref="Response{T}"/>
        /// with the unique identifier of the created category.
        /// </returns>
        Task<Response<Guid>> CreateCategoryAsync(CreateCategoryRequest request);

        /// <summary>
        /// Updates an existing category.
        /// </summary>
        /// <param name="request">The request object containing the updated category details.</param>
        /// <param name="id">The unique identifier of the category to update.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a <see cref="Response{T}"/>
        /// with the unique identifier of the updated category.
        /// </returns>
        Task<Response<Guid>> UpdateCategoryAsync(UpdateCategoryRequest request, Guid id);

        /// <summary>
        /// Deletes a category by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the category to delete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a <see cref="Response{T}"/>
        /// with the unique identifier of the deleted category.
        /// </returns>
        Task<Response<Guid>> DeleteCategoryAsync(Guid id);
    }
}
