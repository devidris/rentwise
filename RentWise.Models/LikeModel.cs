using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentWise.Models
{
    public class LikeModel:DefaultModel
    {
        [Key]
        public int Id { get; set; } 
        public string UserId { get; set; }
        public string ProductId { get; set; }
    }
}
