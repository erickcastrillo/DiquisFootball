using Ardalis.Specification;
using Diquis.Application.Common.Specification;
using Diquis.Domain.Entities.Catalog;
using Diquis.Domain.Entities.Football.Common;

namespace Diquis.Application.Services.DivisionService.Specifications
{
    /// <summary>
    /// Specification for searching and sorting <see cref="Division"/> entities for table views.
    /// Supports filtering by division name and dynamic ordering.
    /// </summary>
    public class DivisionSearchTable : Specification<Division>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DivisionSearchTable"/> class.
        /// Applies optional filtering by name and dynamic ordering.
        /// </summary>
        /// <param name="name">Optional. The name or partial name to filter divisions by. If null or whitespace, no name filter is applied.</param>
        /// <param name="dynamicOrder">Optional. The dynamic order string (e.g., "Name desc"). If null or empty, defaults to descending by <see cref="Division.CreatedOn"/>.</param>
        public DivisionSearchTable(string? name = "", string? dynamicOrder = "")
        {
            // filters
            if (!string.IsNullOrWhiteSpace(name))
            {
                _ = Query.Where(x => x.Name.Contains(name));
            }

            // sort order
            if (string.IsNullOrEmpty(dynamicOrder))
            {
                _ = Query.OrderByDescending(x => x.CreatedOn); // default sort order
            }
            else
            {
                _ = Query.OrderBy(dynamicOrder); // dynamic (JQDT) sort order
            }
        }
    }
}
