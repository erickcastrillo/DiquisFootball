using Diquis.Application.Common.Marker;

namespace Diquis.Infrastructure.Multitenancy.DTOs
{
    /// <summary>
    /// Data Transfer Object representing a tenant option.
    /// </summary>
    public class TenantOptionDTO : IDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the tenant option.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the tenant option.
        /// </summary>
        public string Name { get; set; }
    }
}
