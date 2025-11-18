using System.Security.Claims;
using Diquis.Application.Common;

namespace Diquis.WebApi.Middleware
{
    public class TenantResolver
    {
        private readonly RequestDelegate _next;
        public TenantResolver(RequestDelegate next)
        {
            _next = next;
        }

        // get Tenant Id (and User Id) from incoming requests 
        public async Task InvokeAsync(HttpContext context, ICurrentTenantUserService newCurrentTenantService, IWebHostEnvironment env)
        {
            // order is important -- first, check for tenant id in the JWT token
            string? tenantFromAuth = context.User?.FindFirstValue("tenant");
            if (!string.IsNullOrEmpty(tenantFromAuth))
            {
                _ = await newCurrentTenantService.SetTenantUser(tenantFromAuth); // this will set the tenant Id and the user Id (from the token)
            }
            else
            {
                // if no token, check anonymous requests with tenant Id in subdomain or request header header         
                // -- subdomain (optional)
                _ = context.Request.Headers.TryGetValue("Host", out var fullUrl);
                int segmentLength = (env.IsDevelopment() ? 2 : 3); // check lengths to extract subdomain from 'localhost:7250' vs 'diquis.net' or whatever your situation is
                string tenantFromSubdomain = GetSubDomain(fullUrl, segmentLength);

                if (string.IsNullOrEmpty(tenantFromSubdomain) == false)
                {
                    _ = await newCurrentTenantService.SetTenantUser(tenantFromSubdomain);
                }
                else // Header
                {

                    _ = context.Request.Headers.TryGetValue("tenant", out var tenantFromHeader); // Tenant Id from incoming request header
                    if (string.IsNullOrEmpty(tenantFromHeader) == false)
                    {
                        _ = await newCurrentTenantService.SetTenantUser(tenantFromHeader);
                    }
                    else
                    {
                        _ = await newCurrentTenantService.SetTenantUser("root"); // Fallback (Needed to load the react client initially)
                    }
                }
            }

            await _next(context);
        }

        // once the tenant Id is set, queries will only see users belonging to this tenant - this is made possible with global query filters in ApplicationDbContext 
        // next the username / password from the request body will be checked and the user will be issued a JWT token if valid     


        private static string GetSubDomain(string url, int segmentLength) // this is for resolving the tenant by subdomain
        {
            if (url.Contains("azurewebsites"))
            {
                return string.Empty;
            }

            string[] nodes = url.Split('.');
            return nodes.Length == segmentLength ? nodes[0].ToString() : string.Empty;
        }
    }
}
