using Diquis.Application.Common.Marker;
using FluentValidation;

namespace Diquis.Application.Services.ProductService.DTOs
{
    public class UpdateProductRequest : IDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class UpdateProductValidator : AbstractValidator<UpdateProductRequest>
    {
        public UpdateProductValidator()
        {
            _ = RuleFor(x => x.Name).NotEmpty();
            _ = RuleFor(x => x.Description).NotEmpty();
        }
    }
}

