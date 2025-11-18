using Diquis.Application.Common.Marker;
using FluentValidation;

namespace Diquis.Infrastructure.Multitenancy.DTOs
{
    /// <summary>
    /// Represents a request to create a new tenant.
    /// </summary>
    public class CreateTenantRequest : IDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the tenant. Must be a slug (lowercase letters, numbers, and hyphens only).
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the tenant.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the administrator's email address for the tenant.
        /// </summary>
        public string AdminEmail { get; set; }

        /// <summary>
        /// Gets or sets the password for the tenant's administrator.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the tenant should have an isolated database.
        /// </summary>
        public bool HasIsolatedDatabase { get; set; } // create a field in the UI to capture this property
    }

    /// <summary>
    /// Validator for <see cref="CreateTenantRequest"/>.
    /// </summary>
    public class CreateTenantValidator : AbstractValidator<CreateTenantRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateTenantValidator"/> class.
        /// </summary>
        public CreateTenantValidator()
        {
            _ = RuleFor(x => x.Id).Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$")
                .WithMessage("Tenant Id must be a slug (lowercase letters, numbers, and hyphens only).");
            _ = RuleFor(x => x.Name).NotEmpty();
            _ = RuleFor(x => x.AdminEmail).NotEmpty().EmailAddress();
        }
    }
}
