using Diquis.Domain.Entities.Common;

namespace Diquis.Domain.Entities.Catalog
{
    /// <summary>
    /// Represents a product in the catalog.
    /// </summary>
    public class Product : AuditableEntity
    {
        /// <summary>
        /// Gets or sets the name of the product.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the product.
        /// </summary>
        public string Description { get; set; }
    }
}
