using Diquis.Application.Common.Marker;
using Diquis.Application.Common.Wrapper;
using Diquis.Domain.Entities.Multitenancy;
using Diquis.Infrastructure.Multitenancy.DTOs;

namespace Diquis.Infrastructure.Multitenancy
{
    /// <summary>
    /// Defines the contract for tenant management operations in a multitenant application.
    /// Provides methods for retrieving, creating, and updating tenants.
    /// </summary>
    public interface ITenantManagementService : IScopedService
    {
        /// <summary>
        /// Retrieves all tenants in the system.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation.
        /// The task result contains a <see cref="Response{T}"/> with an <see cref="IEnumerable{Tenant}"/> of tenants.
        /// </returns>
        Task<Response<IEnumerable<Tenant>>> GetTenants();

        /// <summary>
        /// Retrieves tenant options for selection purposes.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation.
        /// The task result contains a <see cref="Response{T}"/> with an <see cref="IEnumerable{TenantOptionDTO}"/> of tenant options.
        /// </returns>
        Task<Response<IEnumerable<TenantOptionDTO>>> GetTenantOptions();

        /// <summary>
        /// Creates and saves a new tenant based on the provided request.
        /// </summary>
        /// <param name="request">The request containing tenant creation details.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation.
        /// The task result contains a <see cref="Response{T}"/> with the created <see cref="Tenant"/>.
        /// </returns>
        Task<Response<Tenant>> SaveTenant(CreateTenantRequest request);

        /// <summary>
        /// Updates an existing tenant with the specified ID using the provided request data.
        /// </summary>
        /// <param name="request">The request containing updated tenant details.</param>
        /// <param name="id">The unique identifier of the tenant to update.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation.
        /// The task result contains a <see cref="Response{T}"/> with the updated <see cref="Tenant"/>.
        /// </returns>
        Task<Response<Tenant>> UpdateTenant(UpdateTenantRequest request, string id);
    }
}
