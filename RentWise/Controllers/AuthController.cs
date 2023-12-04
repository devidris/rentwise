﻿using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using RentWise.Models.Identity;
using System.Text.Encodings.Web;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using RentWise.Utility;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using RentWise.Models;
using Microsoft.AspNetCore.Hosting;

namespace RentWise.Controllers
{
    public class AuthController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly ILogger<Authentication> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IOptions<RentWiseConfig> _config;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AuthController(
              UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<Authentication> logger,
            IEmailSender emailSender, IOptions<RentWiseConfig> config, IWebHostEnvironment webHostEnvironment)

        {

            _userManager = userManager;
            _roleManager = roleManager;
            _userStore = userStore;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _config = config;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Authentication model)
        {
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, model.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    await _userManager.AddToRoleAsync(user, Lookup.Roles[3]);
                    var userId = await _userManager.GetUserIdAsync(user);
                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = model.Email, returnUrl = model.ReturnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);

                        return LocalRedirect(model.ReturnUrl);

                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View();
        }

        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Login(AuthenticationLogin model)
        {


            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(model.ReturnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View();
                }
            }
            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                TempData["ToastMessage"] = "Please check your email to reset your password";
                if (user == null)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToAction("Index", "Home");
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(
                    model.Email,
                    "Reset Password",
                    $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            }
            return RedirectToAction("Index", "Home");
        }
        [Authorize]
        public IActionResult Profile()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string profilePicture = $"/images/{userId}/";
            if (!Directory.Exists(profilePicture))
            {
                profilePicture = $"/images/{userId}/{String.Join("", Lookup.Upload[5].Split(" "))}.png";
            }
            else
            {
                profilePicture = "~/img/profile.png";
            }
            ViewBag.ProfilePicture = profilePicture;
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Profile(ChangePasswordModel model, IFormFile? image)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    #region Saving Profile Image
                    string profileImageName = String.Join("", Lookup.Upload[5].Split(" ")) + ".png";
                    saveImage(userId, profileImageName, image);
                    #endregion
                }  
                string profilePicture = $"/images/{userId}/";
                if (!Directory.Exists(profilePicture))
                {
                    profilePicture = $"/images/{userId}/{String.Join("", Lookup.Upload[5].Split(" "))}.png";
                } else
                {
                    profilePicture = "~/img/profile.png";
                }

                ViewBag.ProfilePicture = profilePicture;

                if(!String.IsNullOrEmpty(model.OldPassword))
                {
                    var user = await _userManager.GetUserAsync(User);
                    var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (!changePasswordResult.Succeeded)
                    {
                        foreach (var error in changePasswordResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View();
                    }
                    await _signInManager.RefreshSignInAsync(user);
                }
                TempData["ToastMessage"] = "Password Updated Successfully";  
                return RedirectToAction("Profile", "Auth");
            }

            return RedirectToAction("Profile","Auth");
        }

        public void saveImage(string userId, string fileName, IFormFile file)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string filePath = @"images\" + userId + "\\";
            string finalPath = Path.Combine(wwwRootPath, filePath);

            if (!Directory.Exists(finalPath))
            {
                Directory.CreateDirectory(finalPath);
            }

            using (FileStream fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

        }

    }
}
