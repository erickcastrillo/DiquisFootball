using Diquis.Application.Common.Wrapper;
using Diquis.Domain.Entities.Multitenancy;
using Diquis.Infrastructure.Multitenancy;
using Diquis.Infrastructure.Multitenancy.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Diquis.WebApi.Controllers
{
    /// <summary>
    /// API controller for managing tenants in a multitenant application.
    /// Provides endpoints for retrieving, creating, and updating tenants.
    /// </summary>
    [Authorize(Roles = "root")]
    [Route("api/[controller]")]
    [ApiController]
    public class TenantsController : ControllerBase
    {
        private readonly ITenantManagementService _tenantService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantsController"/> class.
        /// </summary>
        /// <param name="tenantService">The tenant management service.</param>
        public TenantsController(ITenantManagementService tenantService)
        {
            _tenantService = tenantService;
        }

        /// <summary>
        /// Gets the current authenticated user's ID from claims.
        /// </summary>
        /// <returns>The user ID.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when user ID cannot be found.</exception>
        private string GetCurrentUserId() =>
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("User ID not found in claims");

        /// <summary>
        /// Retrieves a list of tenant options (names and IDs only).
        /// This endpoint is accessible anonymously to populate tenant dropdowns on the login view.
        /// </summary>
        /// <returns>A list of tenant options wrapped in a response object.</returns>
        /// <remarks>
        /// Retrieves a list of tenant options (names and IDs only) for populating tenant dropdowns.<br/>
        /// <b>Sample request:</b> GET /api/Tenants/tenant-options<br/>
        /// <b>Authorization:</b> Anonymous.
        /// </remarks>
        /// <response code="200">Returns the list of tenant options.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [AllowAnonymous]
        [HttpGet("tenant-options")]
        public async Task<IActionResult> GetTenantOptions()
        {
            Response<IEnumerable<TenantOptionDTO>> result = await _tenantService.GetTenantOptions();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves the full list of tenants.
        /// </summary>
        /// <returns>A list of tenants wrapped in a response object.</returns>
        /// <remarks>
        /// Retrieves the full list of tenants.<br/>
        /// <b>Sample request:</b> GET /api/Tenants<br/>
        /// <b>Authorization:</b> Requires role: root.
        /// </remarks>
        /// <response code="200">Returns the list of tenants.</response>
        /// <response code="401">If the user is unauthorized.</response>
        /// <response code="403">If the user does not have permission.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpGet]
        public async Task<IActionResult> GetTenants()
        {
            Response<IEnumerable<Tenant>> result = await _tenantService.GetTenants();
            return Ok(result);
        }

        /// <summary>
        /// Creates a new tenant asynchronously via background job.
        /// </summary>
        /// <param name="request">The request containing tenant creation details.</param>
        /// <returns>Accepted response with tenant ID and background job information.</returns>
        /// <remarks>
        /// Creates a new tenant with the provided details. The tenant provisioning (database creation, admin user setup)
        /// is handled asynchronously in a background job. You will receive real-time notifications via SignalR when the process completes.<br/>
        /// <b>Sample request:</b> <br/>
        /// POST /api/Tenants<br/>
        /// <pre>{@
        ///   "id": "academy-demo",
        ///   "name": "Demo Academy",
        ///   "adminEmail": "admin@academy-demo.com",
        ///   "password": "SecurePassword123!",
        ///   "hasIsolatedDatabase": true
        /// }</pre>
        /// <b>Authorization:</b> Requires role: root.
        /// </remarks>
        /// <response code="202">Returns accepted with tenant ID. Provisioning will continue in background.</response>
        /// <response code="400">If the request data is invalid or tenant already exists.</response>
        /// <response code="401">If the user is unauthorized.</response>
        /// <response code="403">If the user does not have permission.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpPost]
        public async Task<IActionResult> SaveTenant(CreateTenantRequest request)
        {
            var userId = GetCurrentUserId();
            Response<string> result = await _tenantService.SaveTenantAsync(request, userId);

            return result.Succeeded
                ? Accepted(result) // 202 Accepted for async operation
                : BadRequest(result);
        }

        /// <summary>
        /// Updates an existing tenant asynchronously via background job.
        /// </summary>
        /// <param name="request">The request containing updated tenant details.</param>
        /// <param name="id">The unique identifier of the tenant to update.</param>
        /// <returns>Accepted response with tenant ID and background job information.</returns>
        /// <remarks>
        /// Updates an existing tenant with the specified ID. The update is handled asynchronously in a background job.
        /// You will receive real-time notifications via SignalR when the process completes.<br/>
        /// <b>Sample request:</b> <br/>
        /// PUT /api/Tenants/{id}<br/>
        /// <pre>{@
        ///   "name": "Updated Academy Name",
        ///   "isActive": true
        /// }</pre>
        /// <b>Authorization:</b> Requires role: root.
        /// </remarks>
        /// <response code="202">Returns accepted with tenant ID. Update will continue in background.</response>
        /// <response code="400">If the request data or ID is invalid, or if attempting to edit root tenant.</response>
        /// <response code="401">If the user is unauthorized.</response>
        /// <response code="403">If the user does not have permission.</response>
        /// <response code="404">If the tenant is not found.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTenant(UpdateTenantRequest request, string id)
        {
            var userId = GetCurrentUserId();
            Response<string> result = await _tenantService.UpdateTenantAsync(request, id, userId);

            return result.Succeeded
                ? Accepted(result)
                : BadRequest(result);
        }
    }
}
