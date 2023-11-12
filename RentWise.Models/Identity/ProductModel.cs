﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentWise.Models.Identity
{
    public class ProductModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name="Category")]
        public int LkpCategory { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        public string Description { get; set; }
        public string ProductId { get; set; } = Guid.NewGuid().ToString();
        [Range(1, int.MaxValue, ErrorMessage = "The daily rental price must be higher than zero")]
        [Display(Name = "Price per day")]
        public int PriceDay { get; set; } = 0;
        [Range(1, int.MaxValue, ErrorMessage = "The weekend rental price must be higher than zero")]
        [Display(Name="Price per weekend")]
        public int PriceWeekend { get; set; } = 0;
        [Range(1, int.MaxValue, ErrorMessage = "The weekly rental price must be higher than zero")]
        [Display(Name ="Price per week")]
        public int PriceWeek { get; set; } = 0;
        [NotMapped]
        [Display(Name = "Main Image")]
        public string? MainImage { get; set; }
        public int NoOfImages { get; set; } 
        public string? Includes { get; set; }
        public string? Rules { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        [Display(Name="Cancellation Policy")]
        [NotMapped]
        public string? CancellationPolicy { get; set; }
        [ValidateNever]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        [ValidateNever]
        public IdentityUser User { get; set; }
    }
}
