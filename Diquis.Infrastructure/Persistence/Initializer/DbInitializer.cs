using Diquis.Domain.Entities.Common;
using Diquis.Domain.Entities.Multitenancy;
using Diquis.Infrastructure.Persistence.Contexts;
using Microsoft.AspNetCore.Identity;

namespace Diquis.Infrastructure.Persistence.Initializer
{
    /// <summary>
    /// Provides methods for seeding the database with initial data such as tenants, users, and roles.
    /// </summary>
    public static class DbInitializer
    {
        /// <summary>
        /// Seeds the root tenant, admin user, and default roles if they do not exist.
        /// </summary>
        /// <param name="context">The database context to seed.</param>
        public static void SeedTenantAdminAndRoles(BaseDbContext context)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));
            Tenant rootTenant = context.Tenants.FirstOrDefault(x => x.Id == "root"); // if no root tenant is found
            if (rootTenant != null)
            {
                return;
            }


            Tenant tenant = new() // create root tenant
            {
                Id = "root",
                Name = "Root",
                IsActive = true,
                CreatedOn = DateTime.UtcNow,
            };

            _ = context.Tenants.Add(tenant);

            ApplicationUser user = new() // create root tenant admin user
            {
                Id = "55555555-5555-5555-5555-555555555555",
                Email = "admin@email.com",
                NormalizedEmail = "ADMIN@EMAIL.COM",
                UserName = "admin@email.com.root",
                FirstName = "Default",
                LastName = "Admin",
                NormalizedUserName = "ADMIN@EMAIL.COM.ROOT",
                PhoneNumber = null,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D"),
                TenantId = "root",
            };

            PasswordHasher<ApplicationUser> password = new();
            string hashed = password.HashPassword(user, "Password123!");
            user.PasswordHash = hashed;
            _ = context.Users.Add(user);


            List<IdentityRole> roles = new() // create default roles
            {
                new IdentityRole() { Id = "1", Name = "root", ConcurrencyStamp = Guid.NewGuid().ToString("D"), NormalizedName = "ROOT" }, // super_user
                new IdentityRole() { Id = "2", Name = "academy_owner", ConcurrencyStamp = Guid.NewGuid().ToString("D"), NormalizedName = "ACADEMY_OWNER" },
                new IdentityRole() { Id = "3", Name = "academy_admin", ConcurrencyStamp = Guid.NewGuid().ToString("D"), NormalizedName = "ACADEMY_ADMIN" },
                new IdentityRole() { Id = "4", Name = "director_of_football", ConcurrencyStamp = Guid.NewGuid().ToString("D"), NormalizedName = "DIRECTOR_OF_FOOTBALL" },
                new IdentityRole() { Id = "5", Name = "coach", ConcurrencyStamp = Guid.NewGuid().ToString("D"), NormalizedName = "COACH" },
                new IdentityRole() { Id = "6", Name = "parent", ConcurrencyStamp = Guid.NewGuid().ToString("D"), NormalizedName = "PARENT" },
                new IdentityRole() { Id = "7", Name = "player", ConcurrencyStamp = Guid.NewGuid().ToString("D"), NormalizedName = "PLAYER" }
            };
            context.Roles.AddRange(roles);

            IdentityUserRole<string> rootAdmin = new() { RoleId = "1", UserId = "55555555-5555-5555-5555-555555555555" }; // add root admin user to root role
            _ = context.UserRoles.Add(rootAdmin);

            _ = context.SaveChanges();
        }
    }
}
