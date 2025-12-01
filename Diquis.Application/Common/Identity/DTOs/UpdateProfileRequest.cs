using Diquis.Application.Common.Marker;
using FluentValidation;

namespace Diquis.Application.Common.Identity.DTOs
{
    /// <summary>
    /// Represents a request to update a user's profile information.
    /// </summary>
    public class UpdateProfileRequest : IDto
    {
        /// <summary>
        /// Gets or sets the first name of the user.
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Gets or sets the last name of the user.
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// Gets or sets the phone number of the user.
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        public string Email { get; set; }

    }

    /// <summary>
    /// Validator for <see cref="UpdateProfileRequest"/>.
    /// </summary>
    public class UpdateProfileValidator : AbstractValidator<UpdateProfileRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateProfileValidator"/> class.
        /// </summary>
        public UpdateProfileValidator()
        {
            _ = RuleFor(x => x.FirstName).NotEmpty();
            _ = RuleFor(x => x.LastName).NotEmpty();
            _ = RuleFor(x => x.Email).NotEmpty().EmailAddress();
        }
    }
}
