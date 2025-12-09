using Diquis.Application.Common.Marker;
using Diquis.Domain.Enums;
using FluentValidation;

namespace Diquis.Application.Services.ProductService.DTOs
{
    /// <summary>
    /// Represents a request to create a new product.
    /// </summary>
    public class CreateProductRequest : IDto
    {
        /// <summary>
        /// Gets or sets the name of the product.
        /// </summary>
        public string Name { get; set; } = null!;
        /// <summary>
        /// Gets or sets the description of the product.
        /// </summary>
        public string Description { get; set; } = null!;
        /// <summary>
        /// Gets or sets the locale of the product.
        /// </summary>
        public Locale Locale { get; set; }
    }

    /// <summary>
    /// Validator for <see cref="CreateProductRequest"/>.
    /// </summary>
    public class CreateProductValidator : AbstractValidator<CreateProductRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateProductValidator"/> class.
        /// </summary>
        public CreateProductValidator()
        {
            _ = RuleFor(x => x.Name).NotEmpty();
            _ = RuleFor(x => x.Description).NotEmpty();
            _ = RuleFor(x => x.Locale).IsInEnum();
        }
    }
}
