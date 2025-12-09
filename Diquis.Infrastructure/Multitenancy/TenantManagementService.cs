using AutoMapper;
using Diquis.Application.Common;
using Diquis.Application.Common.Wrapper;
using Diquis.Application.Utility;
using Diquis.Domain.Entities.Common;
using Diquis.Domain.Entities.Multitenancy;
using Diquis.Domain.Enums;
using Diquis.Infrastructure.BackgroundJobs;
using Diquis.Infrastructure.Multitenancy.DTOs;
using Diquis.Infrastructure.Persistence.Contexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;

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
        private readonly IBackgroundJobService _backgroundJobService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantManagementService"/> class.
        /// </summary>
        /// <param name="baseDbContext">The base database context.</param>
        /// <param name="configuration">The application configuration.</param>
        /// <param name="serviceProvider">The service provider for dependency resolution.</param>
        /// <param name="userManager">The user manager for identity operations.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="environment">The web hosting environment.</param>
        /// <param name="backgroundJobService">The background job service for enqueueing jobs.</param>
        public TenantManagementService(
            BaseDbContext baseDbContext,
            IConfiguration configuration,
            IServiceProvider serviceProvider,
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            IWebHostEnvironment environment,
            IBackgroundJobService backgroundJobService)
        {
            _baseDbContext = baseDbContext;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            _userManager = userManager;
            _mapper = mapper;
            _environment = environment;
            _backgroundJobService = backgroundJobService;
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
        /// Creates a new tenant and enqueues a background job for provisioning.
        /// </summary>
        /// <param name="request">The tenant creation request.</param>
        /// <param name="initiatingUserId">The ID of the user initiating the tenant creation.</param>
        /// <returns>
        /// A <see cref="Response{T}"/> containing the tenant ID and job ID.
        /// </returns>
        public async Task<Response<string>> SaveTenantAsync(CreateTenantRequest request, string initiatingUserId)
        {
            string organizationSlug = NanoHelpers.ToUrlSlug(request.Id);
            bool tenantExists = await _baseDbContext.Tenants.AnyAsync(x => x.Id == organizationSlug);

            if (tenantExists)
            {
                return Response<string>.Fail("Tenant already exists");
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            NpgsqlConnectionStringBuilder builder = new(connectionString);
            string mainDatabaseName = builder.Database;
            string tenantDbName = mainDatabaseName + "-" + organizationSlug;
            builder.Database = tenantDbName;
            string modifiedConnectionString = builder.ConnectionString;

            Tenant tenant = new()
            {
                Id = request.Id,
                Name = request.Name,
                ConnectionString = request.HasIsolatedDatabase ? modifiedConnectionString : null,
                CreatedOn = DateTime.UtcNow,
                IsActive = true,
                Status = ProvisioningStatus.Pending
            };

            try
            {
                await _baseDbContext.Tenants.AddAsync(tenant);
                await _baseDbContext.SaveChangesAsync();

                // Enqueue background job for tenant provisioning
                var jobId = _backgroundJobService.Enqueue(() =>
                    new ProvisionTenantJob(default!, default!, default!, default!, default!, default!, default!)
                        .ExecuteAsync(tenant.Id, request, initiatingUserId));

                var response = Response<string>.Success(tenant.Id);
                response.Messages.Add("Tenant creation initiated. The tenant will be provisioned in the background.");
                return response;
            }
            catch (Exception ex)
            {
                return Response<string>.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Updates an existing tenant by enqueueing a background job.
        /// </summary>
        /// <param name="request">The update request containing new tenant data.</param>
        /// <param name="id">The identifier of the tenant to update.</param>
        /// <param name="initiatingUserId">The ID of the user initiating the tenant update.</param>
        /// <returns>
        /// A <see cref="Response{T}"/> containing the tenant ID and job ID.
        /// </returns>
        public async Task<Response<string>> UpdateTenantAsync(UpdateTenantRequest request, string id, string initiatingUserId)
        {
            Tenant tenant = await _baseDbContext.Tenants.SingleOrDefaultAsync(x => x.Id == id);

            if (tenant == null)
            {
                return Response<string>.Fail("Not Found");
            }

            if (tenant.Id == "root")
            {
                return Response<string>.Fail("Cannot edit root tenant");
            }

            try
            {
                // Enqueue background job for tenant update
                var jobId = _backgroundJobService.Enqueue(() =>
                    new UpdateTenantJob(default!, default!, default!)
                        .ExecuteAsync(id, request, initiatingUserId));

                var response = Response<string>.Success(id);
                response.Messages.Add("Tenant update initiated. The tenant will be updated in the background.");
                return response;
            }
            catch (Exception ex)
            {
                return Response<string>.Fail(ex.Message);
            }
        }
    }
}
