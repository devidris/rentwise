using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using RentWise.Utility;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace RentWise.Models.Identity
{
    public class Authentication
    {

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string ReturnUrl { get; set; }
    }

    public class AuthenticationLogin
    {

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class AgentRegistrationModel : DefaultModel
    {
        [Key]
        [ValidateNever]
        public string Id { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        [Required]
        [Display(Name = "Residential Address")]
        public string ResidentialAddress { get; set; }
        [Required]
        [Display(Name = "Store Name")]
        public string StoreName { get; set; }
        [ValidateNever]
        public string Slug { get; set; }
        [Display(Name = "Store Address")]
        public string? StoreAddress { get; set; }
        [ValidateNever]
        [NotMapped]
        public string Logo { get; set; }
        [NotMapped]
        [ValidateNever]
        public string Banner { get; set; }
        [NotMapped]
        [ValidateNever]
        [Display(Name = "Profile Picture")]
        public string ProfilePicture { get; set; }
        [NotMapped]
        [ValidateNever]
        [Display(Name = "National Card")]
        public string NationalCard { get; set; }
        [NotMapped]
        [Required]
        public bool Privacy { get; set; }
        [NotMapped]
        [ValidateNever]
        public string Passport { get; set; }

    }
}
