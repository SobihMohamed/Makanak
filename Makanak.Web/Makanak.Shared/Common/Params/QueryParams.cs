using Makanak.Domain.EnumsHelper.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Shared.Common.Params
{
    public class QueryParams
    {
        private const int maxPageSize = 50;
        private int _pageSize = 10;
        public int PageIndex { get; set; } = 1;
        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = (value > maxPageSize) ? maxPageSize : value; }
        }

        private string? _search = string.Empty;
        public string? Search
        {
            get { return _search; }
            set { _search = value?.ToLower() ; }
        }

        public SortingOptionsEnum? Sort { get; set; }
    }
}
