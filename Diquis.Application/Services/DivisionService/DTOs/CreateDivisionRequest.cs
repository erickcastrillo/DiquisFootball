using FluentValidation;
using Diquis.Application.Common.Marker;

namespace Diquis.Application.Services.DivisionService.DTOs
{
    /// <summary>
    /// Request DTO for creating a new division.
    /// </summary>
    public class CreateDivisionRequest : IDto
    {
        /// <summary>
        /// Gets or sets the name of the division.
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// Validator for <see cref="CreateDivisionRequest"/>.
    /// </summary>
    public class CreateDivisionValidator : AbstractValidator<CreateDivisionRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateDivisionValidator"/> class.
        /// </summary>
        public CreateDivisionValidator()
        {
            _ = RuleFor(x => x.Name).NotEmpty();
        }
    }
}
