using AutoMapper;
using Diquis.Application.Common.Wrapper;
using Diquis.Application.Utility;
using Diquis.Domain.Entities.Common;
using Diquis.Domain.Entities.Multitenancy;
using Diquis.Infrastructure.Multitenancy.DTOs;
using Diquis.Infrastructure.Persistence.Contexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Diquis.Infrastructure.Multitenancy
{
    /// <summary>
    /// Provides services for managing tenants, including creation, retrieval, and updates.
    /// </summary>
    public class TenantManagementService : ITenantManagementService
    {
        private readonly BaseDbContext _baseDbContext;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantManagementService"/> class.
        /// </summary>
        /// <param name="baseDbContext">The base database context.</param>
        /// <param name="configuration">The application configuration.</param>
        /// <param name="serviceProvider">The service provider for dependency resolution.</param>
        /// <param name="userManager">The user manager for identity operations.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="environment">The web hosting environment.</param>
        public TenantManagementService(
            BaseDbContext baseDbContext,
            IConfiguration configuration,
            IServiceProvider serviceProvider,
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            IWebHostEnvironment environment)
        {
            _baseDbContext = baseDbContext;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            _userManager = userManager;
            _mapper = mapper;
            _environment = environment;
        }

        /// <summary>
        /// Retrieves all tenant details.
        /// </summary>
        /// <returns>
        /// A <see cref="Response{T}"/> containing an enumerable of <see cref="Tenant"/> objects.
        /// </returns>
        public async Task<Response<IEnumerable<Tenant>>> GetTenants()
        {
            List<Tenant> list = await _baseDbContext.Tenants.OrderByDescending(x => x.CreatedOn).ToListAsync();
            return Response<IEnumerable<Tenant>>.Success(list);
        }

        /// <summary>
        /// Retrieves tenant options (ID and Name) for use in dropdowns or selection lists.
        /// </summary>
        /// <returns>
        /// A <see cref="Response{T}"/> containing an enumerable of <see cref="TenantOptionDTO"/> objects.
        /// </returns>
        public async Task<Response<IEnumerable<TenantOptionDTO>>> GetTenantOptions()
        {
            List<Tenant> list = await _baseDbContext.Tenants
                .Where(x => x.IsActive)
                .OrderByDescending(x => x.Id == "root")
                .ThenBy(x => x.Name)
                .ToListAsync();
            IEnumerable<TenantOptionDTO> tenantOptions = _mapper.Map<IEnumerable<TenantOptionDTO>>(list);

            return Response<IEnumerable<TenantOptionDTO>>.Success(tenantOptions);
        }

        /// <summary>
        /// Creates and saves a new tenant, including the default admin user and optional isolated database.
        /// </summary>
        /// <param name="request">The tenant creation request.</param>
        /// <returns>
        /// A <see cref="Response{T}"/> containing the created <see cref="Tenant"/> or error messages.
        /// </returns>
        public async Task<Response<Tenant>> SaveTenant(CreateTenantRequest request)
        {
            string organizationSlug = NanoHelpers.ToUrlSlug(request.Id);
            bool tenantExists = await _baseDbContext.Tenants.AnyAsync(x => x.Id == organizationSlug);

            if (tenantExists)
            {
                return Response<Tenant>.Fail("Tenant already exists");
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            SqlConnectionStringBuilder builder = new(connectionString);
            string mainDatabaseName = builder.InitialCatalog;
            string tenantDbName = mainDatabaseName + "-" + organizationSlug;
            builder.InitialCatalog = tenantDbName;
            string modifiedConnectionString = builder.ConnectionString;

            Tenant tenant = new()
            {
                Id = request.Id,
                Name = request.Name,
                ConnectionString = request.HasIsolatedDatabase ? modifiedConnectionString : null,
                CreatedOn = DateTime.UtcNow,
                IsActive = true
            };

            try
            {
                _ = await _baseDbContext.Tenants.AddAsync(tenant);

                ApplicationUser user = new()
                {
                    UserName = request.AdminEmail + "." + tenant.Id,
                    FirstName = "Default",
                    LastName = "Admin",
                    Email = request.AdminEmail,
                    EmailConfirmed = true,
                    TenantId = tenant.Id
                };

                IdentityResult result = await _userManager.CreateAsync(user, request.Password);
                if (result.Succeeded)
                {
                    if (request.HasIsolatedDatabase)
                    {
                        try
                        {
                            using IServiceScope scopeTenant = _serviceProvider.CreateScope();
                            ApplicationDbContext dbContext = scopeTenant.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                            dbContext.Database.SetConnectionString(modifiedConnectionString);
                            if (_environment.IsDevelopment())
                            {
                                if (dbContext.Database.GetPendingMigrations().Any())
                                {
                                    Console.ForegroundColor = ConsoleColor.Blue;
                                    Console.WriteLine($"Applying ApplicationDB Migrations for New '{tenant.Id}' tenant.");
                                    Console.ResetColor();
                                    dbContext.Database.Migrate();
                                }
                            }
                            else
                            {
                                // -- add host specific code here for production environment
                                // -- Azure, AWS, etc. https://diquis.com/deploy-multi-tenant-azure-elastic-database-pool
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }
                    }

                    _ = await _userManager.AddToRoleAsync(user, "admin");
                    _ = await _baseDbContext.SaveChangesAsync();
                    return Response<Tenant>.Success(tenant);
                }
                else
                {
                    List<string> messages = new();
                    foreach (IdentityError error in result.Errors)
                    {
                        messages.Add(error.Description);
                    }
                    return Response<Tenant>.Fail(messages);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Updates an existing tenant's information.
        /// </summary>
        /// <param name="request">The update request containing new tenant data.</param>
        /// <param name="id">The identifier of the tenant to update.</param>
        /// <returns>
        /// A <see cref="Response{T}"/> containing the updated <see cref="Tenant"/> or error messages.
        /// </returns>
        public async Task<Response<Tenant>> UpdateTenant(UpdateTenantRequest request, string id)
        {
            Tenant tenant = await _baseDbContext.Tenants.SingleOrDefaultAsync(x => x.Id == id);

            if (tenant == null)
            {
                return Response<Tenant>.Fail("Not Found");
            }

            if (tenant.Id == "root")
            {
                return Response<Tenant>.Fail("Cannot edit root tenant");
            }

            tenant.IsActive = request.IsActive;
            tenant.Name = request.Name;

            try
            {
                _ = await _baseDbContext.SaveChangesAsync();
                return Response<Tenant>.Success(tenant);
            }
            catch (Exception ex)
            {
                return Response<Tenant>.Fail(ex.Message);
            }
        }
    }
}
