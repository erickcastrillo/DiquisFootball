using Diquis.Application.Common.Wrapper;
using Diquis.Domain.Entities.Multitenancy;
using Diquis.Infrastructure.Multitenancy;
using Diquis.Infrastructure.Multitenancy.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Diquis.WebApi.Controllers
{
    [Authorize(Roles = "root")] // restrict to root user
    [Route("api/[controller]")]
    [ApiController]
    public class TenantsController : ControllerBase // tenant management API controller
    {
        private readonly ITenantManagementService _tenantService;

        public TenantsController(ITenantManagementService tenantService)
        {
            _tenantService = tenantService; // inject tenant service
        }

        // full list (names only)
        [AllowAnonymous] // to populate tenant dropdown on login view
        [HttpGet("tenant-options")]
        public async Task<IActionResult> GetTenantOptions()
        {
            Response<IEnumerable<TenantOptionDTO>> result = await _tenantService.GetTenantOptions();
            return Ok(result);
        }

        // full list
        [HttpGet]
        public async Task<IActionResult> GetTenants()
        {
            Response<IEnumerable<Tenant>> result = await _tenantService.GetTenants();
            return Ok(result);
        }

        // create
        [HttpPost]
        public async Task<IActionResult> SaveTenant(CreateTenantRequest request)
        {
            Response<Tenant> result = await _tenantService.SaveTenant(request);
            return Ok(result);
        }

        // update
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTenant(UpdateTenantRequest request, string id)
        {
            Response<Tenant> result = await _tenantService.UpdateTenant(request, id);
            return Ok(result);
        }
    }
}
