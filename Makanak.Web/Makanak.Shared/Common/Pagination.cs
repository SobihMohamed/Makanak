using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Shared.Common
{
    public class Pagination<TEntity>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public IEnumerable<TEntity> Data { get; set; }
        public Pagination(int pageIndex , int pageSize , int totalCount , IEnumerable<TEntity> entities) 
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = totalCount;
            Data = entities;
        }
    }
}
