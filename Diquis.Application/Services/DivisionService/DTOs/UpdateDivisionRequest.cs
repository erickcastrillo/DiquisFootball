using FluentValidation;
using Diquis.Application.Common.Marker;

namespace Diquis.Application.Services.DivisionService.DTOs
{
    /// <summary>
    /// Represents a request to update a division.
    /// </summary>
    public class UpdateDivisionRequest : IDto
    {
        /// <summary>
        /// Gets or sets the name of the division.
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// Validator for <see cref="UpdateDivisionRequest"/>.
    /// </summary>
    public class UpdateDivisionValidator : AbstractValidator<UpdateDivisionRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateDivisionValidator"/> class.
        /// </summary>
        public UpdateDivisionValidator()
        {
            _ = RuleFor(x => x.Name).NotEmpty();
        }
    }
}

