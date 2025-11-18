using System.Collections.Generic;

namespace Diquis.Application.Common.Filter
{
    public class PaginationFilter
    {
        // server-side pagination filter, specific to Tanstack Table v8 conventions (React, Vue) https://tanstack.com/table/v8
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public List<TanstackColumnOrder> Sorting { get; set; }
        public PaginationFilter()
        {
            PageNumber = 1;
            PageSize = 10;
            Sorting = new List<TanstackColumnOrder>();
        }
    }
    public class TanstackColumnOrder
    {
        public string Id { get; set; }
        public bool Desc { get; set; }
    }
}
