using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Diquis.Application.Services.DivisionService.DTOs;
using Diquis.Application.Services.DivisionService.Filters;
using Diquis.Application.Common.Wrapper;
using Diquis.Application.Services.DivisionService;

namespace Diquis.WebApi.Controllers.Football
{
    /// <summary>
    /// API controller for managing Division entities.
    /// </summary>
    /// <remarks>
    /// Provides endpoints for CRUD operations and listing of divisions.
    /// </remarks>
    [Route("api/Football/[controller]")]
    [ApiController]
    public class DivisionsController : ControllerBase
    {
        private readonly IDivisionService _DivisionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DivisionsController"/> class.
        /// </summary>
        /// <param name="DivisionService">The division service dependency.</param>
        public DivisionsController(IDivisionService DivisionService)
        {
            _DivisionService = DivisionService;
        }

        /// <summary>
        /// Gets a full list of divisions, optionally filtered by a keyword.
        /// </summary>
        /// <param name="keyword">The keyword to filter divisions by name (optional).</param>
        /// <returns>A response containing a list of <see cref="DivisionDTO"/>.</returns>
        /// <remarks>
        /// Returns all divisions. Requires one of the following roles: root, admin, editor, basic.
        /// </remarks>
        /// <response code="200">Returns the list of divisions.</response>
        /// <response code="400">If the request is invalid.</response>
        /// <response code="401">If the user is unauthorized.</response>
        /// <response code="403">If the user does not have permission.</response>
        /// <response code="404">If no divisions are found.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [Authorize(Roles = "root, admin, editor, basic")]
        [HttpGet]
        public async Task<IActionResult> GetDivisionsAsync(string keyword = "")
        {
            Response<IEnumerable<DivisionDTO>> result = await _DivisionService.GetDivisionsAsync(keyword);
            return Ok(result);
        }

        /// <summary>
        /// Gets a paginated and filtered list of divisions.
        /// </summary>
        /// <param name="filter">The filter and pagination options.</param>
        /// <returns>A paginated response containing <see cref="DivisionDTO"/> items.</returns>
        /// <remarks>
        /// Returns paginated divisions for table display. Requires one of the following roles: root, admin, editor, basic.
        /// </remarks>
        /// <response code="200">Returns the paginated list of divisions.</response>
        /// <response code="400">If the filter is invalid.</response>
        /// <response code="401">If the user is unauthorized.</response>
        /// <response code="403">If the user does not have permission.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [Authorize(Roles = "root, admin, editor, basic")]
        [HttpPost("Divisions-paginated")]
        public async Task<IActionResult> GetDivisionsPaginatedAsync(DivisionTableFilter filter)
        {
            PaginatedResponse<DivisionDTO> result = await _DivisionService.GetDivisionsPaginatedAsync(filter);
            return Ok(result);
        }

        /// <summary>
        /// Gets a single division by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the division.</param>
        /// <returns>A response containing the <see cref="DivisionDTO"/>.</returns>
        /// <remarks>
        /// Returns a single division. Requires one of the following roles: root, admin, editor, basic.
        /// </remarks>
        /// <response code="200">Returns the division data.</response>
        /// <response code="400">If the id is invalid.</response>
        /// <response code="401">If the user is unauthorized.</response>
        /// <response code="403">If the user does not have permission.</response>
        /// <response code="404">If the division is not found.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [Authorize(Roles = "root, admin, editor, basic")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDivisionAsync(Guid id)
        {
            Response<DivisionDTO> result = await _DivisionService.GetDivisionAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Creates a new division.
        /// </summary>
        /// <param name="request">The division creation request.</param>
        /// <returns>A response containing the unique identifier of the created division.</returns>
        /// <remarks>
        /// Creates a new division. Requires one of the following roles: root, admin, editor.
        /// </remarks>
        /// <response code="200">Returns the ID of the newly created division.</response>
        /// <response code="400">If the request data is invalid.</response>
        /// <response code="401">If the user is unauthorized.</response>
        /// <response code="403">If the user does not have permission.</response>
        /// <response code="409">If a division with the same name already exists.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [Authorize(Roles = "root, admin, editor")]
        [HttpPost]
        public async Task<IActionResult> CreateDivisionAsync(CreateDivisionRequest request)
        {
            try
            {
                Response<Guid> result = await _DivisionService.CreateDivisionAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Updates an existing division.
        /// </summary>
        /// <param name="request">The division update request.</param>
        /// <param name="id">The unique identifier of the division to update.</param>
        /// <returns>A response containing the unique identifier of the updated division.</returns>
        /// <remarks>
        /// Updates a division. Requires one of the following roles: root, admin, editor.
        /// </remarks>
        /// <response code="200">Returns the ID of the updated division.</response>
        /// <response code="400">If the request data or id is invalid.</response>
        /// <response code="401">If the user is unauthorized.</response>
        /// <response code="403">If the user does not have permission.</response>
        /// <response code="404">If the division is not found.</response>
        /// <response code="409">If a division with the same name already exists.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [Authorize(Roles = "root,admin, editor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDivisionAsync(UpdateDivisionRequest request, Guid id)
        {
            try
            {
                Response<Guid> result = await _DivisionService.UpdateDivisionAsync(request, id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a division by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the division to delete.</param>
        /// <returns>A response containing the unique identifier of the deleted division.</returns>
        /// <remarks>
        /// Deletes a division. Requires one of the following roles: root, admin, editor.
        /// </remarks>
        /// <response code="200">Returns the ID of the deleted division.</response>
        /// <response code="400">If the id is invalid.</response>
        /// <response code="401">If the user is unauthorized.</response>
        /// <response code="403">If the user does not have permission.</response>
        /// <response code="404">If the division is not found.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [Authorize(Roles = "root,admin, editor")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDivisionAsync(Guid id)
        {
            try
            {
                Response<Guid> response = await _DivisionService.DeleteDivisionAsync(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
