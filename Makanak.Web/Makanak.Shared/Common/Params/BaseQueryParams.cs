using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Shared.Common.Params
{
    public class BaseQueryParams 
    {
        private const int maxPageSize = 50;
        private int _pageSize = 8;

        private int _pageIndex = 1; 

        public int PageIndex
        {
            get => _pageIndex;
            set => _pageIndex = (value <= 0) ? 1 : value;
        }
        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = (value > maxPageSize) ? maxPageSize : value; }
        }

        private string? _search = string.Empty;
        public string? Search
        {
            get { return _search; }
            set { _search = value?.ToLower(); }
        }
    }
}
