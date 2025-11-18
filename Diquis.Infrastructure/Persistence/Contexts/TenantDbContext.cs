using Diquis.Domain.Entities.Multitenancy;
using Microsoft.EntityFrameworkCore;

//----------------------- CLI Commands -----------------------------

// this context is used purely for looking up the tenant information in middleware (CurrentTenantUserService)
// do not use this context for migrations

// use BaseDbContext for core data -- tenants, identity, configuration
// use ApplicationDbContext for application data -- products, and everything else
// refer to those contexts for migration CLI commands

//------------------------------------------------------------------

namespace Diquis.Infrastructure.Persistence.Contexts
{
    /// <summary>
    /// Represents a lightweight <see cref="DbContext"/> for querying tenant information.
    /// <para>
    /// This context is intended for use in middleware (e.g., <c>CurrentTenantUserService</c>)
    /// to look up tenant details. It should not be used for database migrations.
    /// </para>
    /// <para>
    /// For core data (tenants, identity, configuration), use <c>BaseDbContext</c>.
    /// For application data (products, etc.), use <c>ApplicationDbContext</c>.
    /// </para>
    /// </summary>
    public class TenantDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TenantDbContext"/> class
        /// using the specified options.
        /// </summary>
        /// <param name="options">The options to be used by the <see cref="DbContext"/>.</param>
        public TenantDbContext(DbContextOptions<TenantDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the <see cref="DbSet{TEntity}"/> for <see cref="Tenant"/> entities.
        /// </summary>
        public DbSet<Tenant> Tenants { get; set; }
    }
}
