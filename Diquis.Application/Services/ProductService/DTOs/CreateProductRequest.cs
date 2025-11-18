using Diquis.Application.Common.Marker;
using FluentValidation;

namespace Diquis.Application.Services.ProductService.DTOs
{
    public class CreateProductRequest : IDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class CreateProductValidator : AbstractValidator<CreateProductRequest>
    {
        public CreateProductValidator()
        {
            _ = RuleFor(x => x.Name).NotEmpty();
            _ = RuleFor(x => x.Description).NotEmpty();
        }
    }
}
