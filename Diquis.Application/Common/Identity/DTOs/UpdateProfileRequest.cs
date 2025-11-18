using Diquis.Application.Common.Marker;
using FluentValidation;

namespace Diquis.Application.Common.Identity.DTOs
{
    public class UpdateProfileRequest : IDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

    }

    public class UpdateProfileValidator : AbstractValidator<UpdateProfileRequest>
    {
        public UpdateProfileValidator()
        {
            _ = RuleFor(x => x.FirstName).NotEmpty();
            _ = RuleFor(x => x.LastName).NotEmpty();
            _ = RuleFor(x => x.Email).NotEmpty().EmailAddress();
        }
    }
}
