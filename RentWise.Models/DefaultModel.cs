using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentWise.Models
{
    public class DefaultModel
    {
        public bool Enabled { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;    
        public DateTime UpdatedAt { get; set;} = DateTime.Now;
    }
}
