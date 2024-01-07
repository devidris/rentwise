
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
        public string ProductId { get; set; } = Guid.NewGuid().ToString();
        [Range(0, int.MaxValue, ErrorMessage = "The daily rental price must be higher than zero")]
        [Display(Name = "Price per day")]
        public int PriceDay { get; set; } = 0;
        [Range(0, int.MaxValue, ErrorMessage = "The weekend rental price must be higher than zero")]
        [Display(Name = "Price per weekend")]
        public int PriceWeekend { get; set; } = 0;
        [Range(0, int.MaxValue, ErrorMessage = "The weekly rental price must be higher than zero")]
        [Display(Name = "Price per week")]
        public int PriceWeek { get; set; } = 0;
        [NotMapped]
        [Display(Name = "Main Image")]
        public string? MainImage { get; set; }
        public int NoOfImages { get; set; }
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
        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }

        public string? Country { get; set; }
        public string? State { get; set; }
        [ValidateNever]
        public List<ProductImageModel> ProductImages { get; set; }
    }
}
