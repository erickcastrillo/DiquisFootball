using Diquis.Domain.Entities.Common;
using Diquis.Domain.Entities.Multitenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;

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
                        entry.Entity.CreatedBy = !string.IsNullOrWhiteSpace(CurrentUserId) ? Guid.Parse(CurrentUserId) : Guid.Empty;
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModifiedOn = DateTime.UtcNow;
                        entry.Entity.LastModifiedBy = !string.IsNullOrWhiteSpace(CurrentUserId) ? Guid.Parse(CurrentUserId) : Guid.Empty;
                        break;

                    case EntityState.Deleted:
                        // Intercept delete requests, forward as modified on tables with ISoftDelete
                        if (entry.Entity is ISoftDelete softDelete)
                        {
                            softDelete.DeletedOn = DateTime.UtcNow;
                            softDelete.DeletedBy = !string.IsNullOrWhiteSpace(CurrentUserId) ? Guid.Parse(CurrentUserId) : Guid.Empty;
                            entry.State = EntityState.Modified;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Sets tenant and audit fields and saves changes within an explicit transaction.
        /// If an error occurs, the transaction is automatically rolled back.
        /// </summary>
        /// <typeparam name="TContext">The type of the DbContext.</typeparam>
        /// <param name="context">The DbContext instance.</param>
        /// <param name="CurrentUserId">The current user's ID as a string (should be a valid Guid or null).</param>
        /// <param name="CurrentTenantId">The current tenant's ID as a string.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>The number of state entries written to the database.</returns>
        /// <exception cref="Exception">Throws any exception that occurs during the save operation after rolling back the transaction.</exception>
        /// <remarks>
        /// This method wraps the save operation in an explicit transaction. If a transaction is already active,
        /// it will be reused to avoid nested transactions. The transaction is automatically committed on success
        /// or rolled back on failure.
        /// </remarks>
        public static async Task<int> SaveChangesWithTransactionAsync<TContext>(
            this TContext context,
            string CurrentUserId,
            string CurrentTenantId,
            CancellationToken cancellationToken = default) where TContext : DbContext
        {
            // Check if there's already an active transaction
            IDbContextTransaction currentTransaction = context.Database.CurrentTransaction;
            
            if (currentTransaction != null)
            {
                // Transaction already exists, just set fields and save
                context.TenantAndAuditFields(CurrentUserId, CurrentTenantId);
                // Call base SaveChangesAsync to avoid infinite recursion
                return await SaveChangesBaseCoreAsync(context, cancellationToken);
            }

            // Create new transaction
            await using IDbContextTransaction transaction = await context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                // Set tenant and audit fields
                context.TenantAndAuditFields(CurrentUserId, CurrentTenantId);

                // Save changes - call base to avoid infinite recursion
                int result = await SaveChangesBaseCoreAsync(context, cancellationToken);

                // Commit transaction
                await transaction.CommitAsync(cancellationToken);

                return result;
            }
            catch
            {
                // Rollback transaction on error
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        /// <summary>
        /// Calls the base DbContext.SaveChangesAsync method to avoid calling any overridden implementation.
        /// This prevents infinite recursion when SaveChangesAsync is overridden in derived contexts.
        /// </summary>
        /// <typeparam name="TContext">The type of the DbContext.</typeparam>
        /// <param name="context">The DbContext instance.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>The number of state entries written to the database.</returns>
        private static Task<int> SaveChangesBaseCoreAsync<TContext>(TContext context, CancellationToken cancellationToken) where TContext : DbContext
        {
            // Call SaveChangesAsync with acceptAllChangesOnSuccess parameter to invoke the base implementation
            // This bypasses any overridden SaveChangesAsync() method in derived classes
            return context.SaveChangesAsync(acceptAllChangesOnSuccess: true, cancellationToken);
        }
    }
}

