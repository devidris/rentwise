using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentWise.Models
{
    public class ReviewModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int RatingValue { get; set; }
        public string RatingDescription { get; set; }
        [ValidateNever]
        public string UserId { get; set; }
        [ValidateNever]
        public string ProductId { get; set; }
        [ValidateNever]
        public string AgentId { get; set; }
        [ValidateNever]
        public string UserName { get; set; }

    }
}
