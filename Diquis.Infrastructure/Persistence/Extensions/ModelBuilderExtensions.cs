using System.Linq.Expressions;
using Diquis.Domain.Entities.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Query;

namespace Diquis.Infrastructure.Persistence.Extensions
{
    /// <summary>
    /// Provides extension methods for configuring the Entity Framework Core <see cref="ModelBuilder"/>.
    /// </summary>
    public static class ModelBuilderExtensions
    {
        /// <summary>
        /// Renames ASP.NET Identity tables to custom names within the "Identity" schema.
        /// </summary>
        /// <param name="builder">The <see cref="ModelBuilder"/> to configure.</param>
        public static void ApplyIdentityConfiguration(this ModelBuilder builder)
        {
            _ = builder.Entity<ApplicationUser>(entity =>
            {
                _ = entity.ToTable("Users", "Identity");
            });
            _ = builder.Entity<IdentityRole>(entity =>
            {
                _ = entity.ToTable("Roles", "Identity");
            });
            _ = builder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                _ = entity.ToTable("RoleClaims", "Identity");
            });
            _ = builder.Entity<IdentityUserRole<string>>(entity =>
            {
                _ = entity.ToTable("UserRoles", "Identity");
            });
            _ = builder.Entity<IdentityUserClaim<string>>(entity =>
            {
                _ = entity.ToTable("UserClaims", "Identity");
            });
            _ = builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                _ = entity.ToTable("UserLogins", "Identity");
            });
            _ = builder.Entity<IdentityUserToken<string>>(entity =>
            {
                _ = entity.ToTable("UserTokens", "Identity");
            });
        }

        /// <summary>
        /// Appends a global query filter to all entities implementing the specified interface.
        /// </summary>
        /// <typeparam name="TInterface">The interface type to filter entities by.</typeparam>
        /// <param name="modelBuilder">The <see cref="ModelBuilder"/> to configure.</param>
        /// <param name="expression">The filter expression to apply.</param>
        /// <returns>The configured <see cref="ModelBuilder"/>.</returns>
        public static ModelBuilder AppendGlobalQueryFilter<TInterface>(this ModelBuilder modelBuilder, Expression<Func<TInterface, bool>> expression)
        {
            IEnumerable<Type> entities = modelBuilder.Model
                .GetEntityTypes()
                .Where(e => e.ClrType.GetInterface(typeof(TInterface).Name) != null)
                .Select(e => e.ClrType);
            foreach (Type entity in entities)
            {
                ParameterExpression parameterType = Expression.Parameter(modelBuilder.Entity(entity).Metadata.ClrType);
                Expression expressionFilter = ReplacingExpressionVisitor.Replace(expression.Parameters.Single(), parameterType, expression.Body);
                LambdaExpression currentQueryFilter = modelBuilder.Entity(entity).GetQueryFilter();
                if (currentQueryFilter != null)
                {
                    Expression currentExpressionFilter = ReplacingExpressionVisitor.Replace(currentQueryFilter.Parameters.Single(), parameterType, currentQueryFilter.Body);
                    expressionFilter = Expression.AndAlso(currentExpressionFilter, expressionFilter);
                }

                LambdaExpression lambdaExpression = Expression.Lambda(expressionFilter, parameterType);

                _ = modelBuilder.Entity(entity).HasQueryFilter(lambdaExpression);
            }
            return modelBuilder;
        }

        /// <summary>
        /// Retrieves the current query filter applied to the entity, if any.
        /// </summary>
        /// <param name="builder">The <see cref="EntityTypeBuilder"/> for the entity.</param>
        /// <returns>The <see cref="LambdaExpression"/> representing the query filter, or <c>null</c> if none exists.</returns>
        private static LambdaExpression GetQueryFilter(this EntityTypeBuilder builder)
        {
            return builder?.Metadata?.GetQueryFilter();
        }
    }
}
