using FluentValidation;
using Diquis.Application.Common.Marker;

namespace Diquis.Application.Services.CategoryService.DTOs
{
    /// <summary>
    /// Represents a request to create a new category.
    /// </summary>
    public class CreateCategoryRequest : IDto
    {
        /// <summary>
        /// Gets or sets the name of the category to be created.
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// Validator for <see cref="CreateCategoryRequest"/>.
    /// </summary>
    public class CreateCategoryValidator : AbstractValidator<CreateCategoryRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCategoryValidator"/> class.
        /// </summary>
        public CreateCategoryValidator()
        {
            _ = RuleFor(x => x.Name).NotEmpty();
        }
    }
}
