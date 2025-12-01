using AutoMapper;
using Diquis.Application.Common.Identity.DTOs;
using Diquis.Application.Services.CategoryService.DTOs;
using Diquis.Application.Services.DivisionService.DTOs;
using Diquis.Application.Services.ProductService.DTOs;
using Diquis.Domain.Entities.Catalog;
using Diquis.Domain.Entities.Common;
using Diquis.Domain.Entities.Football.Common;
using Diquis.Domain.Entities.Multitenancy;
using Diquis.Infrastructure.Multitenancy.DTOs;

namespace Diquis.Infrastructure.Mapper
{
    /// <summary>
    /// Defines AutoMapper mapping configurations for the Diquis infrastructure layer.
    /// </summary>
    public class MappingProfiles : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MappingProfiles"/> class.
        /// Configures entity-to-DTO and DTO-to-entity mappings for tenants, users, and products.
        /// </summary>
        public MappingProfiles()
        {
            // tenant mappings
            _ = CreateMap<Tenant, TenantOptionDTO>();

            // user mappings
            _ = CreateMap<ApplicationUser, UserDto>();
            _ = CreateMap<UpdateProfileRequest, ApplicationUser>();
            _ = CreateMap<UpdateUserRequest, ApplicationUser>();
            _ = CreateMap<RegisterUserRequest, ApplicationUser>()
                .ForMember(x => x.UserName, o => o.MapFrom(s => s.Email));
            _ = CreateMap<UserDto, UpdateProfileRequest>();

            // product mappings
            _ = CreateMap<Product, ProductDTO>();
            _ = CreateMap<CreateProductRequest, Product>();
            _ = CreateMap<UpdateProductRequest, Product>();

            // add new entity mappings here...
            _ = CreateMap<Category, CategoryDTO>();
            _ = CreateMap<CreateCategoryRequest, Category>();
            _ = CreateMap<UpdateCategoryRequest, Category>();

            _ = CreateMap<Division, DivisionDTO>();
            _ = CreateMap<CreateDivisionRequest, Division>();
            _ = CreateMap<UpdateDivisionRequest, Division>();

        }
    }
}
