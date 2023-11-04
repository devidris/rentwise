using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentWise.Models.Identity
{
    public class ProductModel
    {
        public string LkpCategory { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProductId { get; set; } = Guid.NewGuid().ToString();
        public int PriceDay { get; set; }
        public int PriceWeekend { get; set; }
        public int PriceWeek { get; set; }
    }
}
