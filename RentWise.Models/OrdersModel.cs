using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentWise.Models
{
    public class OrdersModel:DefaultModel
    {
        [Key]
        public int OrderId { get; set; }
        public string ProductId { get; set; }
        [ForeignKey("ProductId")]
        [ValidateNever]
        public ProductModel Product { get; set; }
        public string UserId { get; set; }

        public string AgentId { get; set; }
        public int LkpStatus { get; set; } = 1;
        public bool Paid { get; set; } = false;

        public DateTime? PaidDate { get; set; }
        public int LkpPaymentMethod { get; set; } 

        public int ProductQuantity { get; set; }
        public int TotalAmount { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
