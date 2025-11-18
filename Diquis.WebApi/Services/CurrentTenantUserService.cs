using System.Security.Claims;
using Diquis.Application.Common;
using Diquis.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

// This is a scoped service that does not persist data, it fits better in the WebApi project vs the Infrastructure project
// --This class uses a seperate DB Context to avoid a circular logic error when it looks up the tenant
// --Query filters in ApplicationDbContext depend on having a 'TenantId' present in CurrentTenantUserService

namespace Diquis.WebApi.Services
{
    public class CurrentTenantUserService : ICurrentTenantUserService
    {
        private readonly TenantDbContext _tenantDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentTenantUserService(TenantDbContext tenantDbContext, IHttpContextAccessor httpContextAccessor)
        {
            _tenantDbContext = tenantDbContext;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<bool> SetTenantUser(string tenant)
        {
            var tenantInfo = await _tenantDbContext.Tenants.Where(x => x.Id == tenant && x.IsActive).FirstOrDefaultAsync(); // check if tenant exists

            if (tenantInfo != null)
            {
                TenantId = tenant;
                ConnectionString = tenantInfo.ConnectionString;
                UserId = _httpContextAccessor?.HttpContext?.User?.FindFirstValue("uid"); // this is the User ID (GUID) from the JWT token, it will be null on login
                return true;
            }
            else
            {
                throw new Exception("Tenant invalid");
            }
        }

        public string? ConnectionString { get; set; }
        public string? TenantId { get; set; }
        public string? UserId { get; set; }
    }
}
