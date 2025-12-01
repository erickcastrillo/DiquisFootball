using Diquis.Application.Common.Marker;

namespace Diquis.Application.Services.ProductService.DTOs
{
    /// <summary>
    /// Data Transfer Object representing a product.
    /// </summary>
    public class ProductDTO : IDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the product.
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Gets or sets the name of the product.
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// Gets or sets the description of the product.
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// Gets or sets the date and time when the product was created.
        /// </summary>
        public DateTime CreatedOn { get; set; }

    }
}

