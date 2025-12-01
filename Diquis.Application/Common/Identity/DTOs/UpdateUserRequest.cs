using Diquis.Application.Common.Marker;
using FluentValidation;

namespace Diquis.Application.Common.Identity.DTOs
{
    /// <summary>
    /// Represents a request to update an existing user's details.
    /// </summary>
    public class UpdateUserRequest : IDto
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
        /// <summary>
        /// Gets or sets a value indicating whether the user is active.
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// Gets or sets the role ID assigned to the user.
        /// </summary>
        public string RoleId { get; set; }
    }

    /// <summary>
    /// Validator for <see cref="UpdateUserRequest"/>.
    /// </summary>
    public class UpdateUserValidator : AbstractValidator<UpdateUserRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateUserValidator"/> class.
        /// </summary>
        public UpdateUserValidator()
        {
            _ = RuleFor(x => x.FirstName).NotEmpty();
            _ = RuleFor(x => x.LastName).NotEmpty();
            _ = RuleFor(x => x.Email).NotEmpty().EmailAddress();
            _ = RuleFor(x => x.IsActive).NotNull(); //Null will accept true or false

            List<string> conditions = new() { "admin", "editor", "basic" };
            _ = RuleFor(x => x.RoleId).Must(conditions.Contains)
                    .WithMessage("Please only use: " + string.Join(", ", conditions));

        }
    }
}
