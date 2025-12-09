using Diquis.Application.Common;
using Diquis.Domain.Entities.Common;
using Diquis.Domain.Entities.Multitenancy;
using Diquis.Infrastructure.Persistence.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Diquis.Infrastructure.Persistence.Contexts
{
    // this context is responsible for identity, tenants, and any global configuration data
    // in a multi-database scenerio, this context manages tables that belong only in the main db, while applicationDbContext manages tables that are generated in every database

    //---------------------------------- CLI COMMANDS --------------------------------------------------

    // set default project to Diquis.Infrastructure in Package Manager Console
    // when scaffolding database migrations, you must specify which -context (BaseDbContext), -o is the output directory, use the following command:

    // add-migration -Context BaseDbContext -o Persistence/Migrations/BaseDb Base-MigrationName
    // update-database -Context BaseDbContext (or just run the app, migrations are run automatically on app startup)

    //--------------------------------------------------------------------------------------------------


    /// <summary>
    /// Represents an application role for identity management.
    /// Inherits from <see cref="IdentityRole"/>.
    /// </summary>
    public class ApplicationRoles : IdentityRole
    {
    }

    /// <summary>
    /// The base database context for the application.
    /// Responsible for managing identity, tenants, and global configuration data.
    /// In a multi-database scenario, this context manages tables that belong only in the main database,
    /// while <c>ApplicationDbContext</c> manages tables generated in every database.
    /// </summary>
    public class BaseDbContext : IdentityDbContext<ApplicationUser>
    {
        /// <summary>
        /// Gets or sets the current tenant identifier.
        /// </summary>
        public string CurrentTenantId { get; set; }

        /// <summary>
        /// Gets or sets the current user identifier.
        /// </summary>
        public string CurrentUserId { get; set; }

        private readonly ICurrentTenantUserService _currentTenantService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseDbContext"/> class.
        /// Default constructor for <see cref="IDesignTimeDbContextFactory{TContext}"/>.
        /// </summary>
        /// <param name="options">The options to be used by a <see cref="DbContext"/>.</param>
        public BaseDbContext(DbContextOptions<BaseDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseDbContext"/> class with tenant and user context.
        /// </summary>
        /// <param name="currentTenantUserService">The current tenant user service.</param>
        /// <param name="options">The options to be used by a <see cref="DbContext"/>.</param>
        public BaseDbContext(ICurrentTenantUserService currentTenantUserService, DbContextOptions<BaseDbContext> options) : base(options)
        {
            _currentTenantService = currentTenantUserService;
            CurrentTenantId = _currentTenantService.TenantId;
            CurrentUserId = _currentTenantService.UserId;
        }

        /// <summary>
        /// Gets or sets the tenants in the system.
        /// </summary>
        public DbSet<Tenant> Tenants { get; set; }

        /// <summary>
        /// Configures the model for the context.
        /// Applies global query filters, renames tables, and runs seeders.
        /// </summary>
        /// <param name="builder">The model builder being used to configure the context.</param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Rename identity tables
            builder.ApplyIdentityConfiguration();

            // Multitenancy query filter for ApplicationUser
            _ = builder.Entity<ApplicationUser>().HasQueryFilter(a => a.TenantId == CurrentTenantId);

            // Filter out deleted entities (soft delete)
            _ = builder.AppendGlobalQueryFilter<ISoftDelete>(s => s.DeletedOn == null);
        }

        /// <summary>
        /// Saves all changes made in this context to the database asynchronously.
        /// Handles audit fields and soft delete logic before saving within a transaction.
        /// The transaction is automatically rolled back if any error occurs.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>The number of state entries written to the database.</returns>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return await this.SaveChangesWithTransactionAsync(CurrentUserId, CurrentTenantId, cancellationToken);
        }
    }
}
