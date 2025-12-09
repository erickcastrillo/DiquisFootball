using Diquis.Application.Common;
using Diquis.Domain.Entities.Catalog;
using Diquis.Domain.Entities.Common;
using Diquis.Domain.Entities.Multitenancy;
using Diquis.Domain.Enums;
using Diquis.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Diquis.Infrastructure.Persistence.Contexts
{
    public class ApplicationDbContext : DbContext
    {
        public string CurrentTenantId { get; set; }
        public string CurrentUserId { get; set; }
        public string CurrentTenantConnectionString { get; set; }

        private readonly ICurrentTenantUserService _currentTenantService;

        public ApplicationDbContext(ICurrentTenantUserService currentTenantUserService, DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            _currentTenantService = currentTenantUserService;
            CurrentTenantId = _currentTenantService.TenantId;
            CurrentUserId = _currentTenantService.UserId;
            CurrentTenantConnectionString = _currentTenantService.ConnectionString;
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            _ = builder.AppendGlobalQueryFilter<IMustHaveTenant>(b => b.TenantId == CurrentTenantId);
            _ = builder.AppendGlobalQueryFilter<ISoftDelete>(s => s.DeletedOn == null);

            builder.SeedStaticData();
            
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                if (typeof(ILocalizable).IsAssignableFrom(entityType.ClrType))
                {
                    builder.Entity(entityType.ClrType)
                        .Property(nameof(ILocalizable.Locale))
                        .HasConversion<string>();
                }
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string tenantConnectionString = CurrentTenantConnectionString;
            if (!string.IsNullOrEmpty(tenantConnectionString))
            {
                _ = optionsBuilder.UseNpgsql(tenantConnectionString);
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return await this.SaveChangesWithTransactionAsync(CurrentUserId, CurrentTenantId, cancellationToken);
        }
    }
}


