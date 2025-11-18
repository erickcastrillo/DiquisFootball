using Diquis.Domain.Entities.Common;
using Diquis.Domain.Entities.Multitenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Diquis.Infrastructure.Persistence.Extensions
{
    /// <summary>
    /// Provides extension methods for handling tenant and audit fields during DbContext save operations.
    /// </summary>
    public static class OnSaveChangesExtensions
    {
        /// <summary>
        /// Sets tenant and audit fields for entities implementing <see cref="IMustHaveTenant"/>, <see cref="IAuditableEntity"/>, and <see cref="ISoftDelete"/> interfaces.
        /// This method should be called before saving changes to ensure proper population of tenant, audit, and soft delete fields.
        /// </summary>
        /// <typeparam name="TContext">The type of the DbContext.</typeparam>
        /// <param name="context">The DbContext instance.</param>
        /// <param name="CurrentUserId">The current user's ID as a string (should be a valid Guid or null).</param>
        /// <param name="CurrentTenantId">The current tenant's ID as a string.</param>
        /// <remarks>
        /// - For entities implementing <see cref="IMustHaveTenant"/>, sets the <c>TenantId</c> property on add or modify.
        /// - For entities implementing <see cref="IAuditableEntity"/>, sets audit fields (<c>CreatedOn</c>, <c>CreatedBy</c>, <c>LastModifiedOn</c>, <c>LastModifiedBy</c>).
        /// - For entities implementing <see cref="ISoftDelete"/>, intercepts delete operations and marks them as modified with soft delete fields.
        /// </remarks>
        public static void TenantAndAuditFields<TContext>(this TContext context, string CurrentUserId, string CurrentTenantId) where TContext : DbContext
        {
            ChangeTracker changeTracker = context.ChangeTracker;

            // Write tenant Id to tables with IMustHaveTenant
            foreach (var entry in changeTracker.Entries<IMustHaveTenant>().ToList())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                    case EntityState.Modified:
                        entry.Entity.TenantId = CurrentTenantId;
                        break;
                }
            }

            // Handle auditable fields and soft delete on tables with IAuditableEntity
            foreach (var entry in changeTracker.Entries<IAuditableEntity>().ToList())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedOn = DateTime.UtcNow;
                        entry.Entity.CreatedBy = CurrentUserId != null ? Guid.Parse(CurrentUserId) : Guid.Empty;
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModifiedOn = DateTime.UtcNow;
                        entry.Entity.LastModifiedBy = CurrentUserId != null ? Guid.Parse(CurrentUserId) : Guid.Empty;
                        break;

                    case EntityState.Deleted:
                        // Intercept delete requests, forward as modified on tables with ISoftDelete
                        if (entry.Entity is ISoftDelete softDelete)
                        {
                            softDelete.DeletedOn = DateTime.UtcNow;
                            softDelete.DeletedBy = CurrentUserId != null ? Guid.Parse(CurrentUserId) : Guid.Empty;
                            entry.State = EntityState.Modified;
                        }
                        break;
                }
            }
        }
    }
}

