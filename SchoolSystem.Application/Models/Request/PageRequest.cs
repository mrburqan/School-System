using System;
using System.Collections.Generic;

namespace SchoolSystem.Application.Models.Request
{
    public class PageRequest
    {
        private int _pageNumber = 1;
        public int pageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value > 0 ? value : 1;
        }

        private int _pageSize = int.MaxValue;
        public int pageSize
        {
            get => _pageSize;
            set => _pageSize = value < 1 ? int.MaxValue : value;
        }

        public int Skip => (pageNumber - 1) * pageSize;
        public int Take => pageSize;
        private string _searchTerm;
        public string SearchTerm
        {
            get => _searchTerm;
            set => _searchTerm = value?.Trim();
        }

        private string _orderByColumn;
        public string OrderByColumn
        {
            get => _orderByColumn;
            set => _orderByColumn = value?.Trim();
        }
        public bool IsAscending { get; set; } = true;
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public Dictionary<string, object> Filters { get; set; } = new Dictionary<string, object>();
        public List<string> IncludeProperties { get; set; } = new List<string>();
    }
}
