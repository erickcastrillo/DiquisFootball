using System.Linq.Expressions;
using System.Reflection;
using Ardalis.Specification;

namespace Diquis.Application.Common.Specification
{
    /// <summary>
    /// Provides extension methods for Ardalis.Specification to support dynamic ordering.
    /// </summary>
    /// <remarks>
    /// This extension allows the OrderBy method to accept a string list of columns for sort ordering,
    /// e.g., ('Name,-Supplier,Property.Name,Price') where a prefix '-' denotes Descending order.
    /// This is particularly useful for components like JQuery Datatables and Tanstack Table which send dynamic column ordering.
    /// </remarks>
    public static class ArdalisSpecificationExtensions
    {
        /// <summary>
        /// Applies dynamic ordering to a specification based on a comma-separated string of fields.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="specificationBuilder">The <see cref="ISpecificationBuilder{T}"/> instance.</param>
        /// <param name="orderByFields">
        /// A comma-separated string of field names to order by.
        /// Prefix a field with '-' for descending order (e.g., "Name,-CreatedOn").
        /// </param>
        /// <returns>The <see cref="ISpecificationBuilder{T}"/> with the order applied.</returns>
        /// <exception cref="ArgumentException">Thrown if a specified property is not found on the entity type.</exception>
        public static ISpecificationBuilder<T> OrderBy<T>(
       this ISpecificationBuilder<T> specificationBuilder,
       string orderByFields)
        {
            if (string.IsNullOrWhiteSpace(orderByFields))
                return specificationBuilder;

            var fields = orderByFields.Split(',');
            IOrderedSpecificationBuilder<T> orderedBuilder = null;

            for (var index = 0; index < fields.Length; index++)
            {
                var field = fields[index].Trim();
                bool isDescending = field.StartsWith('-');
                if (isDescending)
                    field = field.Substring(1);

                Type targetType = typeof(T);
                PropertyInfo matchedProperty = FindNestedProperty(targetType, field.ToLower());

                if (matchedProperty == null)
                    throw new ArgumentException($"Property '{field}' not found on type '{typeof(T).Name}'", nameof(orderByFields));

                ParameterExpression paramExpr = Expression.Parameter(typeof(T));

                Expression propertyExpr = paramExpr;
                foreach (string member in field.Split('.'))
                {
                    propertyExpr = Expression.PropertyOrField(propertyExpr, member);
                }

                Expression<Func<T, object?>> keySelector = Expression.Lambda<Func<T, object?>>(
                    Expression.Convert(propertyExpr, typeof(object)),
                    paramExpr);

                // First field uses OrderBy/OrderByDescending, subsequent fields use ThenBy/ThenByDescending
                if (index == 0)
                {
                    orderedBuilder = isDescending
                        ? specificationBuilder.OrderByDescending(keySelector)
                        : specificationBuilder.OrderBy(keySelector);
                }
                else
                {
                    orderedBuilder = isDescending
                        ? orderedBuilder.ThenByDescending(keySelector)
                        : orderedBuilder.ThenBy(keySelector);
                }
            }

            return specificationBuilder;
        }

        /// <summary>
        /// Finds a nested property by its name, supporting dot notation (e.g., "Supplier.Name").
        /// </summary>
        /// <param name="type">The type to search for the property.</param>
        /// <param name="propertyName">The name of the property, potentially nested.</param>
        /// <returns>The <see cref="PropertyInfo"/> of the found property, or null if not found.</returns>
        // helper method for cases where the column property is nested, for example 'Supplier.Name'
        public static PropertyInfo FindNestedProperty(Type type, string propertyName)
        {
            string[] propertyNames = propertyName.Split('.'); // Split the property name by dot to handle nesting

            Type currentType = type;
            PropertyInfo property = null;

            foreach (string name in propertyNames)
            {
                PropertyInfo? nestedProperty = currentType.GetProperties()
                    .FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

                if (nestedProperty == null)
                {
                    return null; // Property not found at this level
                }

                currentType = nestedProperty.PropertyType;
                property = nestedProperty;
            }

            return property;
        }
    }
}
