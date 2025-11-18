using Diquis.Application.Common.Marker;
using FluentValidation;

namespace Diquis.Application.Common.Identity.DTOs
{
    public class RegisterUserRequest : IDto
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string PhoneNumber { get; set; }

        public string RoleId { get; set; }
    }

    public class RegisterUserValidator : AbstractValidator<RegisterUserRequest>
    {
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
