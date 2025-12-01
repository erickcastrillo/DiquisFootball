using Diquis.Application.Common.Marker;

namespace Diquis.Application.Common.Identity.DTOs
{
    /// <summary>
    /// Data Transfer Object for user information.
    /// </summary>
    public class UserDto : IDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the user.
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Gets or sets the URL of the user's profile image.
        /// </summary>
        public string ImageUrl { get; set; }
        /// <summary>
        /// Gets or sets the first name of the user.
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Gets or sets the last name of the user.
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the user account is active.
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// Gets or sets the phone number of the user.
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// Gets or sets the role identifier assigned to the user.
        /// </summary>
        public string RoleId { get; set; }
        /// <summary>
        /// Gets or sets the date and time when the user account was created.
        /// </summary>
        public DateTime CreatedOn { get; set; }
    }
}
