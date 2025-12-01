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
            IDictionary<string, OrderTypeEnum> fields = ParseOrderBy(orderByFields);
            if (fields != null)
            {
                bool isFirst = true;
                foreach (KeyValuePair<string, OrderTypeEnum> field in fields)
                {
                    Type targetType = typeof(T);
                    PropertyInfo matchedProperty = FindNestedProperty(targetType, field.Key.ToLower());

                    if (matchedProperty == null)
                        throw new ArgumentException($"Property '{field.Key}' not found on type '{typeof(T).Name}'", nameof(orderByFields));

                    ParameterExpression paramExpr = Expression.Parameter(typeof(T));

                    Expression propertyExpr = paramExpr;
                    foreach (string member in field.Key.Split('.'))
                    {
                        propertyExpr = Expression.PropertyOrField(propertyExpr, member);
                    }

                    Expression<Func<T, object?>> keySelector = Expression.Lambda<Func<T, object?>>(
                        Expression.Convert(propertyExpr, typeof(object)),
                        paramExpr);

                    // Use the standard Ardalis.Specification OrderBy methods
                    switch (field.Value)
                    {
                        case OrderTypeEnum.OrderBy:
                            specificationBuilder.OrderBy(keySelector);
                            break;
                        case OrderTypeEnum.OrderByDescending:
                            specificationBuilder.OrderByDescending(keySelector);
                            break;
                        case OrderTypeEnum.ThenBy:
                            // For ThenBy, we need to call OrderBy on subsequent fields
                            // The Ardalis library will handle multiple OrderBy calls appropriately
                            if (isFirst)
                                specificationBuilder.OrderBy(keySelector);
                            else
                                specificationBuilder.OrderBy(keySelector);
                            break;
                        case OrderTypeEnum.ThenByDescending:
                            if (isFirst)
                                specificationBuilder.OrderByDescending(keySelector);
                            else
                                specificationBuilder.OrderByDescending(keySelector);
                            break;
                    }
                    isFirst = false;
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

        /// <summary>
        /// Parses a comma-separated string of order-by fields into a dictionary of field names and their order types.
        /// </summary>
        /// <param name="orderByFields">
        /// The input string (e.g., "Name,-Supplier,Property.Name,Price") where '-' denotes descending.
        /// </param>
        /// <returns>A dictionary mapping field names to their <see cref="OrderTypeEnum"/>.</returns>
        // helper method to parse the input string and turn it into something that Ardalis.Specification understands - a list of column names with their sort order
        private static IDictionary<string, OrderTypeEnum> ParseOrderBy(string orderByFields)
        {
            if (orderByFields is null) return null;
            var result = new Dictionary<string, OrderTypeEnum>();
            var fields = orderByFields.Split(',');
            for (var index = 0; index < fields.Length; index++)
            {
                var field = fields[index];
                var orderBy = OrderTypeEnum.OrderBy;
                if (field.StartsWith('-')) orderBy = OrderTypeEnum.OrderByDescending;
                if (index > 0)
                {
                    orderBy = OrderTypeEnum.ThenBy;
                    if (field.StartsWith('-')) orderBy = OrderTypeEnum.ThenByDescending;
                }
                if (field.StartsWith('-')) field = field.Substring(1);
                result.Add(field, orderBy);
            }
            return result;
        }
    }
}
