using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Diquis.Application.Services.CategoryService;
using Diquis.Application.Services.CategoryService.DTOs;
using Diquis.Application.Services.CategoryService.Filters;
using Diquis.Application.Common.Wrapper;

namespace Diquis.WebApi.Controllers.Football
{
    /// <summary>
    /// API controller for managing categories.
    /// </summary>
    [Route("api/Football/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _CategoryService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoriesController"/> class.
        /// </summary>
        /// <param name="CategoryService">The category service.</param>
        public CategoriesController(ICategoryService CategoryService)
        {
            _CategoryService = CategoryService;
        }

        /// <summary>
        /// Gets a list of all categories, optionally filtered by a keyword.
        /// </summary>
        /// <param name="keyword">The keyword to filter categories by name.</param>
        /// <returns>A list of categories wrapped in a response object.</returns>
        /// <remarks>
        /// Retrieves all categories. Optionally, filter by keyword in the category name.<br/>
        /// <b>Sample request:</b> GET /api/Categories?keyword=example<br/>
        /// <b>Authorization:</b> Requires roles: root, admin, editor, basic.
        /// </remarks>
        /// <response code="200">Returns the list of categories.</response>
        /// <response code="400">If the request is invalid.</response>
        /// <response code="401">If the user is unauthorized.</response>
        /// <response code="403">If the user does not have permission.</response>
        /// <response code="404">If no categories are found.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [Authorize(Roles = "root, admin, editor, basic")]
        [HttpGet]
        public async Task<IActionResult> GetCategoriesAsync(string keyword = "")
        {
            Response<IEnumerable<CategoryDTO>> result = await _CategoryService.GetCategoriesAsync(keyword);
            return Ok(result);
        }

        /// <summary>
        /// Gets a paginated and filtered list of categories.
        /// </summary>
        /// <param name="filter">The filter and pagination options.</param>
        /// <returns>A paginated response containing categories.</returns>
        /// <remarks>
        /// Retrieves a paginated list of categories based on the provided filter.<br/>
        /// <b>Sample request:</b> <br/>
        /// POST /api/Categories-paginated<br/>
        /// <pre>{
        ///   "keyword": "example",
        ///   "pageNumber": 1,
        ///   "pageSize": 10,
        ///   "sorting": []
        /// }</pre>
        /// <b>Authorization:</b> Requires roles: root, admin, editor, basic.
        /// </remarks>
        /// <response code="200">Returns the paginated list of categories.</response>
        /// <response code="400">If the filter is invalid.</response>
        /// <response code="401">If the user is unauthorized.</response>
        /// <response code="403">If the user does not have permission.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [Authorize(Roles = "root, admin, editor, basic")]
        [HttpPost("Categories-paginated")]
        public async Task<IActionResult> GetCategoriesPaginatedAsync(CategoryTableFilter filter)
        {
            PaginatedResponse<CategoryDTO> result = await _CategoryService.GetCategoriesPaginatedAsync(filter);
            return Ok(result);
        }

        /// <summary>
        /// Gets a single category by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the category.</param>
        /// <returns>The category data wrapped in a response object.</returns>
        /// <remarks>
        /// Retrieves a category by its unique identifier.<br/>
        /// <b>Sample request:</b> GET /api/Categories/{id}<br/>
        /// <b>Authorization:</b> Requires roles: root, admin, editor, basic.
        /// </remarks>
        /// <response code="200">Returns the category data.</response>
        /// <response code="400">If the id is invalid.</response>
        /// <response code="401">If the user is unauthorized.</response>
        /// <response code="403">If the user does not have permission.</response>
        /// <response code="404">If the category is not found.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [Authorize(Roles = "root, admin, editor, basic")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryAsync(Guid id)
        {
            Response<CategoryDTO> result = await _CategoryService.GetCategoryAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Creates a new category.
        /// </summary>
        /// <param name="request">The request containing the category data.</param>
        /// <returns>The result of the create operation, including the new category's ID.</returns>
        /// <remarks>
        /// Creates a new category with the provided data.<br/>
        /// <b>Sample request:</b> <br/>
        /// POST /api/Categories<br/>
        /// <pre>{
        ///   "name": "Sample Category",
        /// }</pre>
        /// <b>Authorization:</b> Requires roles: root, admin, editor.
        /// </remarks>
        /// <response code="200">Returns the ID of the newly created category.</response>
        /// <response code="400">If the request data is invalid.</response>
        /// <response code="401">If the user is unauthorized.</response>
        /// <response code="403">If the user does not have permission.</response>
        /// <response code="409">If a category with the same name already exists.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [Authorize(Roles = "root, admin, editor")]
        [HttpPost]
        public async Task<IActionResult> CreateCategoryAsync(CreateCategoryRequest request)
        {
            try
            {
                Response<Guid> result = await _CategoryService.CreateCategoryAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Updates an existing category.
        /// </summary>
        /// <param name="request">The request containing the updated category data.</param>
        /// <param name="id">The unique identifier of the category to update.</param>
        /// <returns>The result of the update operation.</returns>
        /// <remarks>
        /// Updates the category with the specified ID using the provided data.<br/>
        /// <b>Sample request:</b> <br/>
        /// PUT /api/Categories/{id}<br/>
        /// <pre>{
        ///   "name": "Updated Category Name",
        /// }</pre>
        /// <b>Authorization:</b> Requires roles: root, admin, editor.
        /// </remarks>
        /// <response code="200">Returns the ID of the updated category.</response>
        /// <response code="400">If the request data or id is invalid.</response>
        /// <response code="401">If the user is unauthorized.</response>
        /// <response code="403">If the user does not have permission.</response>
        /// <response code="404">If the category is not found.</response>
        /// <response code="409">If a category with the same name already exists.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [Authorize(Roles = "root,admin, editor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategoryAsync(UpdateCategoryRequest request, Guid id)
        {
            try
            {
                Response<Guid> result = await _CategoryService.UpdateCategoryAsync(request, id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a category by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the category to delete.</param>
        /// <returns>The result of the delete operation.</returns>
        /// <remarks>
        /// Deletes the category with the specified ID.<br/>
        /// <b>Sample request:</b> DELETE /api/Categories/{id}<br/>
        /// <b>Authorization:</b> Requires roles: root, admin, editor.
        /// </remarks>
        /// <response code="200">Returns the ID of the deleted category.</response>
        /// <response code="400">If the id is invalid.</response>
        /// <response code="401">If the user is unauthorized.</response>
        /// <response code="403">If the user does not have permission.</response>
        /// <response code="404">If the category is not found.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [Authorize(Roles = "root,admin, editor")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoryAsync(Guid id)
        {
            try
            {
                Response<Guid> response = await _CategoryService.DeleteCategoryAsync(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
