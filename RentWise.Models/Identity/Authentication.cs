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
        [ForeignKey("Id")]
        [ValidateNever]
        public IdentityUser User { get; set; }
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
        [Display(Name = "Store Name")]
        public string StoreName { get; set; }
        [ValidateNever]
        public string Slug { get; set; }
        [Display(Name = "Store Address")]
        public string StoreAddress { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }

        public string? Country { get; set; }
        public string? State { get; set; }
        [ValidateNever]
        [NotMapped]
        public string Logo { get; set; }

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
        public string? ReturnController { get; set; } = "Dashboard";
        [NotMapped]
        [ValidateNever]
        public string? ReturnAction { get; set; } = "Index";

        public double PayWithCard { get; set; } = 0;
        public double PayWithCash { get; set; } = 0;
    }

    public class ForgetPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
    public class ChangePasswordModel
    {
        [Display(Name = "Old Password")]
        public string? OldPassword { get; set; }
        [Display(Name = "Password")]
        public string? NewPassword { get; set; }

        [Display(Name = "Confirm password")]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }

        public string Username { get; set; }

    }

    public class UsersDetailsModel:DefaultModel
    {
        [Key]
        public string Id { get; set; }
        public int Orders { get; set; } = 0;
        public int Messages { get; set; } = 0;
        public string Username { get; set; }
    }
}