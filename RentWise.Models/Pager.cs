using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentWise.Models
{
    public class Pager
    {
        public int TotalItems { get; private set; }
        public int CurrentPage { get; private set; }
        public int PageSize { get; private set; }
        public int TotalPages { get; private set; }

        public Pager(int totalItems, int page, int pageSize = 10)
        {
            TotalItems = totalItems;
            CurrentPage = page;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling((decimal)TotalItems / PageSize);
        }
    }
}
