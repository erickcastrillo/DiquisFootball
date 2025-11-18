using Microsoft.EntityFrameworkCore;

namespace Diquis.Infrastructure.Persistence.Extensions
{
    /// <summary>
    /// Provides extension methods for seeding static data into the model.
    /// </summary>
    public static class StaticDataSeederExtensions
    {
        /// <summary>
        /// Seeds static data into the model. This data is managed by EF migrations.
        /// </summary>
        /// <param name="builder">The model builder.</param>
        public static void SeedStaticData(this ModelBuilder builder) // create methods here for model seed data (static data) -- this data will be managed by EF migrations
        {
            //// for example

            //builder.Entity<ProductType>().HasData(
            //    new ProductType() { Id = 1, Name = "typeA" },
            //    new ProductType() { Id = 2, Name = "typeB" },
            //    new ProductType() { Id = 3, Name = "typeC" }
            //    );
        }
    }
}
