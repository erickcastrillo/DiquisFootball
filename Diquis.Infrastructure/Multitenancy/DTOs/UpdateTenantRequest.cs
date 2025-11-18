using Diquis.Application.Common.Marker;
using FluentValidation;

namespace Diquis.Infrastructure.Multitenancy.DTOs
{
    /// <summary>
    /// Represents a request to update tenant information.
    /// </summary>
    public class UpdateTenantRequest : IDto
    {
        /// <summary>
        /// Gets or sets a value indicating whether the tenant is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the name of the tenant.
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// Validator for <see cref="UpdateTenantRequest"/>.
    /// </summary>
    public class UpdateTenantValidator : AbstractValidator<UpdateTenantRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateTenantValidator"/> class.
        /// </summary>
        public UpdateTenantValidator()
        {
            _ = RuleFor(x => x.Name).NotEmpty();
        }
    }
}
