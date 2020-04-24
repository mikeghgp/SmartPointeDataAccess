using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartPointe.DTOs
{
    public class PagedResult<T>
{
    public class PagingInfo
    {
        public int PageNo { get; set; }

        public int PageSize { get; set; }

        public int PageCount { get; set; }

        public long TotalRecordCount { get; set; }

    }
    public List<ResidentialProperty> Data { get; private set; }

    public PagingInfo Paging { get; private set; }

    public PagedResult(List<ResidentialProperty> items, int pageNo, int pageSize, long totalRecordCount)
    {
        Paging = new PagingInfo
        {
            PageNo = pageNo,
            PageSize = pageSize,
            TotalRecordCount = totalRecordCount,
            PageCount = totalRecordCount > 0
                ? (int) Math.Ceiling(totalRecordCount/(double) pageSize)
                : 0
        };

        Data = new List<ResidentialProperty>(items);
        
    }
}
}
