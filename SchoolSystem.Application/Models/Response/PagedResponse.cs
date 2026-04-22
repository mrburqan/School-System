using System;
using System.Collections.Generic;

namespace SchoolSystem.Application.Models.Response
{
    public class PagedResponse<T>
    {
        public IEnumerable<T> Data { get; set; } = Array.Empty<T>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public int TotalPages => PageSize > 0
            ? (int)Math.Ceiling((double)TotalCount / PageSize)
            : 0;

        public static PagedResponse<T> Create(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
        {
            return new PagedResponse<T>
            {
                Data = items ?? Array.Empty<T>(),
                TotalCount = totalCount < 0 ? 0 : totalCount,
                PageNumber = pageNumber > 0 ? pageNumber : 1,
                PageSize = pageSize > 0 ? pageSize : 1
            };
        }

    }
}
