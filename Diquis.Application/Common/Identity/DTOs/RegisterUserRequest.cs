using Diquis.Application.Common.Marker;
using FluentValidation;

namespace Diquis.Application.Common.Identity.DTOs
{
    /// <summary>
    /// Represents a request to register a new user.
    /// </summary>
    public class RegisterUserRequest : IDto
    {
        /// <summary>
        /// Gets or sets the first name of the user.
        /// </summary>
        public string FirstName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the last name of the user.
        /// </summary>
        public string LastName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        public string Email { get; set; } = null!;

        /// <summary>
        /// Gets or sets the password for the user.
        /// </summary>
        public string Password { get; set; } = null!;

        /// <summary>
        /// Gets or sets the phone number of the user.
        /// </summary>
        public string PhoneNumber { get; set; } = null!;

        /// <summary>
        /// Gets or sets the role ID for the user.
        /// </summary>
        public string RoleId { get; set; } = null!;
    }

    /// <summary>
    /// Validator for <see cref="RegisterUserRequest"/>.
    /// </summary>
    public class RegisterUserValidator : AbstractValidator<RegisterUserRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterUserValidator"/> class.
        /// </summary>
        public RegisterUserValidator()
        {
            _ = RuleFor(x => x.Email).NotEmpty().EmailAddress();
            _ = RuleFor(x => x.FirstName).NotEmpty();
            _ = RuleFor(x => x.LastName).NotEmpty();
            _ = RuleFor(x => x.Password).NotEmpty().WithMessage("Please enter the password");


            List<string> conditions = new() { "admin", "editor", "basic" };
            _ = RuleFor(x => x.RoleId).Must(conditions.Contains)
                    .WithMessage("Please only use: " + string.Join(", ", conditions));

        }
    }
}
