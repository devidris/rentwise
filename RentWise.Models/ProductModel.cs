
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RentWise.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace RentWise.Models
{
    public class ProductModel : DefaultModel
    {
        [Required]
        [Display(Name = "Category")]
        public int LkpCategory { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        public string Description { get; set; }
        [Key]
        public string ? ProductId { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "The daily rental price must be higher than zero")]
        [Display(Name = "Price per day")]
        public int PriceDay { get; set; } = 0;
        [NotMapped]
        [Display(Name = "Main Image")]
        public string? MainImage { get; set; }
        public string? Includes { get; set; }
        public string? Rules { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        [ValidateNever]
        public string AgentId { get; set; }
        [ForeignKey("AgentId")]
        [ValidateNever]
        public AgentRegistrationModel Agent { get; set; }


        [Range(0, 10, ErrorMessage = "Rating range is within 0 - 10")]
        public int Rating { get; set; } = 0;

        public int MaxRentalDays { get; set; } = 0;

        [Display(Name = "Address")]
        public string ?Address { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }

        public string? Country { get; set; } = "Ghana";
        public string? State { get; set; }

        [Display(Name = "City")]
        [Required(ErrorMessage = "City is required")]
        public string City { get; set; } = "Ghana";
        [Display(Name = "Region")]
        [Required(ErrorMessage = "Region is required")]
        public string Region { get; set; } = String.Empty;
        [ValidateNever]
        public List<ProductImageModel> ProductImages { get; set; }
    }
}
