using Diquis.Application.Common;
using Diquis.Domain.Entities.Catalog;
using Diquis.Domain.Entities.Common;
using Diquis.Domain.Entities.Football.Common;
using Diquis.Domain.Entities.Football.PlayerManagement;
using Diquis.Domain.Entities.Multitenancy;
using Diquis.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

// this context is responsible for application data
// in a multi-database scenerio, this context manages tables that are generated in every database

//---------------------------------- CLI COMMANDS --------------------------------------------------

// set default project to Diquis.Infrastructure in Package Manager Console
// when scaffolding database migrations, you must specify which context (ApplicationDbContext), -o is the output directory, use the following command:

// add-migration -Context ApplicationDbContext -o Persistence/Migrations/AppDb AppDbInitial
// update-database -Context ApplicationDbContext

// NOTE: if you use the update-database command, you'll likely see 'No migrations were applied. The database is already up to date' because the migrations are applied programatically during the build.

//--------------------------------------------------------------------------------------------------

/// <summary>
/// Represents the main Entity Framework Core database context for application data.
/// In a multi-database scenario, this context manages tables that are generated in every database.
/// </summary>
namespace Diquis.Infrastructure.Persistence.Contexts
{
    /// <summary>
    /// The main context class for application data.
    /// Migrations are run using this context.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Gets or sets the current tenant identifier.
        /// </summary>
        public string CurrentTenantId { get; set; }

        /// <summary>
        /// Gets or sets the current user identifier.
        /// </summary>
        public string CurrentUserId { get; set; }

        /// <summary>
        /// Gets or sets the current tenant's connection string.
        /// </summary>
        public string CurrentTenantConnectionString { get; set; }

        private readonly ICurrentTenantUserService _currentTenantService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
        /// </summary>
        /// <param name="currentTenantUserService">The current tenant user service.</param>
        /// <param name="options">The options to be used by a <see cref="DbContext"/>.</param>
        public ApplicationDbContext(ICurrentTenantUserService currentTenantUserService, DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            _currentTenantService = currentTenantUserService;
            CurrentTenantId = _currentTenantService.TenantId;
            CurrentUserId = _currentTenantService.UserId;
            CurrentTenantConnectionString = _currentTenantService.ConnectionString;
        }

        /// <summary>
        /// Gets or sets the <see cref="DbSet{TEntity}"/> for <see cref="Product"/> entities.
        /// </summary>
        public DbSet<Product> Products { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DbSet{TEntity}"/> for <see cref="Category"/> entities.
        /// </summary>
        public DbSet<Category> Categories { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DbSet{TEntity}"/> for <see cref="Division"/> entities.
        /// </summary>
        public DbSet<Division> Divisions { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DbSet{TEntity}"/> for <see cref="Position"/> entities.
        /// </summary>
        public DbSet<Position> Positions { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DbSet{TEntity}"/> for <see cref="Revision"/> entities.
        /// </summary>
        public DbSet<Revision> Revisions { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DbSet{TEntity}"/> for <see cref="Skill"/> entities.
        /// </summary>
        public DbSet<Skill> Skills { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DbSet{TEntity}"/> for <see cref="Player"/> entities.
        /// </summary>
        public DbSet<Player> Players { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DbSet{TEntity}"/> for <see cref="PlayerSkill"/> entities.
        /// </summary>
        public DbSet<PlayerSkill> PlayerSkills { get; set; }

        /// <summary>
        /// Configures the model by applying global query filters, renaming tables, and running seeders.
        /// </summary>
        /// <param name="builder">The builder being used to construct the model for this context.</param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Multitenancy query filter for all entities implementing IMustHaveTenant.
            _ = builder.AppendGlobalQueryFilter<IMustHaveTenant>(b => b.TenantId == CurrentTenantId);

            // Filter out deleted entities (soft delete).
            _ = builder.AppendGlobalQueryFilter<ISoftDelete>(s => s.DeletedOn == null);

            // Seed static data.
            builder.SeedStaticData();
        }

        /// <summary>
        /// Configures the database (and other options) to be used for this context.
        /// This method is called for each request.
        /// </summary>
        /// <param name="optionsBuilder">A builder used to create or modify options for this context.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string tenantConnectionString = CurrentTenantConnectionString;
            if (!string.IsNullOrEmpty(tenantConnectionString))
            {
                _ = optionsBuilder.UseSqlServer(tenantConnectionString);
            }
        }

        /// <summary>
        /// Asynchronously saves all changes made in this context to the database.
        /// Handles audit fields (createdOn, createdBy, modifiedBy, modifiedOn) and soft delete on save changes.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous save operation. The task result contains the number of state entries written to the database.
        /// </returns>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            this.TenantAndAuditFields(CurrentUserId, CurrentTenantId);
            int result = await base.SaveChangesAsync(cancellationToken);
            return result;
        }
    }
}


