using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarConfigurator.Business.DTOs.Response
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int Page { get; set; }
        public int Count { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => Count > 0 ? (int)Math.Ceiling((double)TotalCount / Count) : 0;
    }
}
