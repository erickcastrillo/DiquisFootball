using Ardalis.Specification;

using Diquis.Domain.Entities.Football.Common;

namespace Diquis.Application.Services.DivisionService.Specifications
{
    /// <summary>
    /// Specification for filtering <see cref="Division"/> entities by name and ordering by name.
    /// </summary>
    public class DivisionMatchName : Specification<Division>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DivisionMatchName"/> class.
        /// Filters divisions by the specified name if provided, and orders the result by name.
        /// </summary>
        /// <param name="name">The name to match. If null or whitespace, no filtering is applied.</param>
        public DivisionMatchName(string? name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                _ = Query.Where(h => h.Name == name);
            }
            _ = Query.OrderBy(h => h.Name);
        }
    }
}
