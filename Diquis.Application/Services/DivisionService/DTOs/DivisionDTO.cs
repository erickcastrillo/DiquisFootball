using Diquis.Application.Common.Marker;

namespace Diquis.Application.Services.DivisionService.DTOs
{
    /// <summary>
    /// Data Transfer Object for Division entity.
    /// </summary>
    public class DivisionDTO : IDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the division.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the division.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the creation date and time of the division.
        /// </summary>
        public DateTime CreatedOn { get; set; }
    }
}

