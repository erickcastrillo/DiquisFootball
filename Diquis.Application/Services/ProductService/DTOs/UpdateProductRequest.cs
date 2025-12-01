using Diquis.Application.Common.Marker;
using FluentValidation;

namespace Diquis.Application.Services.ProductService.DTOs
{
    /// <summary>
    /// Represents a request to update an existing product.
    /// </summary>
    public class UpdateProductRequest : IDto
    {
        /// <summary>
        /// Gets or sets the updated name of the product.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the updated description of the product.
        /// </summary>
        public string Description { get; set; }
    }

    /// <summary>
    /// Validator for <see cref="UpdateProductRequest"/>.
    /// </summary>
    public class UpdateProductValidator : AbstractValidator<UpdateProductRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateProductValidator"/> class.
        /// </summary>
        public UpdateProductValidator()
        {
            _ = RuleFor(x => x.Name).NotEmpty();
            _ = RuleFor(x => x.Description).NotEmpty();
        }
    }
}

