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
    public class ProductImageModel
    {
        [Key]
        public int Key { get; set; }
        public string Name { get; set; }
        public string ProductId { get; set; }

        [ForeignKey("ProductId")]
        [ValidateNever]
        public ProductModel Product { get; set; }
    }
}
