using FluentValidation;
using Diquis.Application.Common.Marker;

namespace Diquis.Application.Services.CategoryService.DTOs
{
    /// <summary>
    /// Represents a request to update a category.
    /// </summary>
    public class UpdateCategoryRequest : IDto
    {
        /// <summary>
        /// Gets or sets the name of the category.
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// Validator for <see cref="UpdateCategoryRequest"/>.
    /// </summary>
    public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCategoryValidator"/> class.
        /// </summary>
        public UpdateCategoryValidator()
        {
            _ = RuleFor(x => x.Name).NotEmpty();
        }
    }
}

