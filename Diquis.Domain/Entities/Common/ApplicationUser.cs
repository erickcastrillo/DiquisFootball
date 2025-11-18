using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;


namespace Diquis.Domain.Entities.Common
{
    /// <summary>
    /// Represents an application user with extended properties for auditing, soft deletion, and multi-tenancy.
    /// Inherits from <see cref="IdentityUser"/> and implements <see cref="IAuditableEntity"/> and <see cref="ISoftDelete"/>.
    /// </summary>
    public class ApplicationUser : IdentityUser, IAuditableEntity, ISoftDelete
    {
        /// <summary>
        /// Gets or sets the user's first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the user's last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the tenant identifier for multi-tenancy support.
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets or sets the URL of the user's profile image.
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the refresh token for the user.
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// Gets or sets the expiry time of the refresh token.
        /// </summary>
        public DateTime RefreshTokenExpiryTime { get; set; }

        /// <summary>
        /// Gets or sets the role identifier for the user.
        /// Not mapped to the database.
        /// </summary>
        [NotMapped]
        public string RoleId { get; set; }

        //-- Auditable / Soft Delete Fields --//

        /// <summary>
        /// Gets or sets the identifier of the user who created this entity.
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the date and time when this entity was created.
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who last modified this entity.
        /// </summary>
        public Guid? LastModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the date and time when this entity was last modified.
        /// </summary>
        public DateTime? LastModifiedOn { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who deleted this entity.
        /// </summary>
        public Guid? DeletedBy { get; set; }

        /// <summary>
        /// Gets or sets the date and time when this entity was deleted.
        /// </summary>
        public DateTime? DeletedOn { get; set; }
    }
}
