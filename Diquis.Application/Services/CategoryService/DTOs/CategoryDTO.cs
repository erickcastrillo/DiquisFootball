using Diquis.Application.Common.Marker;

namespace Diquis.Application.Services.CategoryService.DTOs
{
    /// <summary>
    /// Data Transfer Object representing a category.
    /// </summary>
    public class CategoryDTO : IDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the category.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the category.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the category was created.
        /// </summary>
        public DateTime CreatedOn { get; set; }
    }
}

