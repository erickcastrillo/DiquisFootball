using Ardalis.Specification;

using Diquis.Domain.Entities.Football.Common;

namespace Diquis.Application.Services.DivisionService.Specifications
{
    /// <summary>
    /// Specification for searching and listing <see cref="Division"/> entities.
    /// Applies optional keyword filtering on the division name and sorts results by creation date descending.
    /// </summary>
    public class DivisionSearchList : Specification<Division>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DivisionSearchList"/> class.
        /// </summary>
        /// <param name="keyword">
        /// Optional search keyword to filter divisions by name. If null or whitespace, no filtering is applied.
        /// </param>
        public DivisionSearchList(string? keyword = "")
        {
            // filters
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                _ = Query.Where(x => x.Name.Contains(keyword));
            }

            _ = Query.OrderByDescending(x => x.CreatedOn); // default sort order
        }
    }
}
