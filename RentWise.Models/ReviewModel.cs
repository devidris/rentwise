using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RentWise.Models.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentWise.Models
{
    public class ReviewModel : DefaultModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Range(0, 10, ErrorMessage = "Rating range is within 0 - 10")]
        public int RatingValue { get; set; }
        public string RatingDescription { get; set; }
        [ValidateNever]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        [ValidateNever]
        public UsersDetailsModel UserDetails { get; set; }
        [ValidateNever]
        public string ProductId { get; set; }
        [ForeignKey("ProductId")]
        [ValidateNever]
        public ProductModel Product { get; set; }
        [ValidateNever]
        public string AgentId { get; set; }

    }
}
