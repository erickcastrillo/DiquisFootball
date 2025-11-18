using System.Threading.Tasks;
using Diquis.Application.Common.Marker;

namespace Diquis.Application.Common
{
    /// <summary>
    /// Service interface for managing the current tenant user context.
    /// </summary>
    public interface ICurrentTenantUserService : IScopedService
    {
        /// <summary>
        /// Gets or sets the connection string for the current tenant.
        /// </summary>
        string? ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the tenant identifier.
        /// </summary>
        string? TenantId { get; set; }

        /// <summary>
        /// Sets the tenant user context.
        /// </summary>
        /// <param name="tenant">The tenant identifier.</param>
        /// <returns>True if the tenant user was set successfully; otherwise, false.</returns>
        public Task<bool> SetTenantUser(string tenant);

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        string? UserId { get; set; }
    }
}

