using Microsoft.AspNetCore.Identity.UI.Services;
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
using RentWise.DataAccess.Repository.IRepository;
using RestSharp;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        private readonly IUnitOfWork _unitOfWork;
        public AuthController(
              UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<Authentication> logger,
            IEmailSender emailSender, IOptions<RentWiseConfig> config, IWebHostEnvironment webHostEnvironment, IUnitOfWork unitOfWork)

        {

            _userManager = userManager;
            _roleManager = roleManager;
            _userStore = userStore;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _config = config;
            _webHostEnvironment = webHostEnvironment;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Register()
        {
            return View();
        }

        private bool HasPassed10Minutes(DateTime targetDateTime)
        {
            TimeSpan timeDifference = DateTime.Now - targetDateTime;
            return timeDifference.TotalMinutes >= 10;
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Authentication model)
        {
            OtpVerification phoneOtp = _unitOfWork.Otp.Get(u => u.Value == model.PhoneNumber && u.OTP == model.NumberOTP);
            OtpVerification emailOtp = _unitOfWork.Otp.Get(u => u.Value == model.PhoneNumber && u.OTP == model.NumberOTP);

            if (phoneOtp == null)
            {
                ModelState.AddModelError(string.Empty, "Phone number otp is wrong");
            }
            if (emailOtp == null)
            {
                ModelState.AddModelError(string.Empty, "Email otp is wrong");
            }
            if (HasPassed10Minutes(phoneOtp.UpdatedAt))
            {
                ModelState.AddModelError(string.Empty, "Phone number otp has expired");
            }
            if (HasPassed10Minutes(emailOtp.UpdatedAt))
            {
                ModelState.AddModelError(string.Empty, "Email otp has expired");
            }
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, model.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation("User created a new account with password.");
                    await _userManager.AddToRoleAsync(user, Lookup.Roles[3]);
                    UsersDetailsModel usersDetailsModel = new UsersDetailsModel
                    {
                        Email = model.Email,
                        PhoneNumber = model.PhoneNumber,
                        Username = model.Email.Split('@')[0],
                        Id = user.Id,
                    };
                    _unitOfWork.UsersDetails.Add(usersDetailsModel);
                    _unitOfWork.Save();
                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = model.Email, returnUrl = model.ReturnUrl });
                    }
                    else
                    {
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
            UsersDetailsModel usersDetailsModel = _unitOfWork.UsersDetails.Get(u => u.PhoneNumber == model.Email);
            if(usersDetailsModel != null)
            {
                model.Email = usersDetailsModel.Email;
            }

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
            ChangePasswordModel model = new ChangePasswordModel();
            UsersDetailsModel usersDetails = _unitOfWork.UsersDetails.Get(u => u.Id == userId);
            model.Username = usersDetails.Username;
            string profilePicture = $"images/{userId}";
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string finalPath = Path.Combine(wwwRootPath, profilePicture);
            if (Directory.Exists(finalPath))
            {
                profilePicture = $"~/images/{userId}/{System.String.Join("", Lookup.Upload[5].Split(" "))}.png";
            }
            else
            {
                profilePicture = "~/img/profile.png";
            }
            ViewBag.ProfilePicture = profilePicture;
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Profile(ChangePasswordModel model, IFormFile? image)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            ViewBag.UserId = userId;
            UsersDetailsModel usersDetails = _unitOfWork.UsersDetails.Get(u => u.Id == userId);
            if (_unitOfWork.UsersDetails.Get(u => u.Username == model.Username) != null && usersDetails.Username != model.Username)
            {
                ModelState.AddModelError("Username", "Username is already in use.");
            }
            if (usersDetails.Username != model.Username)
            {
                usersDetails.Username = model.Username;
            }
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    #region Saving Profile Image
                    string profileImageName = System.String.Join("", Lookup.Upload[5].Split(" ")) + ".png";
                    saveImage(userId, profileImageName, image);
                    #endregion
                }
                usersDetails.UpdatedAt = DateTime.Now;
                _unitOfWork.UsersDetails.Update(usersDetails);
                _unitOfWork.Save();
                string profilePicture = $"/images/{userId}/";
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string finalPath = Path.Combine(wwwRootPath, profilePicture);
                if (Directory.Exists(finalPath))
                {
                    profilePicture = $"/images/{userId}/{System.String.Join("", Lookup.Upload[5].Split(" "))}.png";
                }
                else
                {
                    profilePicture = "~/img/profile.png";
                }

                ViewBag.ProfilePicture = profilePicture;

                if (!System.String.IsNullOrEmpty(model.OldPassword))
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
                TempData["ToastMessage"] = "Profile Updated Successfully";
                return RedirectToAction("Profile", "Auth");
            }

            return RedirectToAction("Profile", "Auth");
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

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete(string user)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != user)
            {
                return RedirectToAction("Index", "Home");
            }
            // Get the user manager from your dependency injection system
            var userManager = HttpContext.RequestServices.GetService<UserManager<ApplicationUser>>();

            // Find the user by ID
            var userToDelete = await userManager.FindByIdAsync(userId);
            var result = await userManager.DeleteAsync(userToDelete);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> SendOtp(string type, string value)
        {
            try
            {
                string otp = SharedFunctions.GenerateOTP();
                string message = $"Your verification code is: {otp}";
                if (type == "number" && _webHostEnvironment.IsProduction())
                {
                    string endPoint = "https://api.mnotify.com/api/sms/quick";
                    string apiKey = "uuFHV8HVMVM3gt8rlEdJhUvhS";
                    Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "recipient", new List<string> {value } },
            { "sender", "Rentwise" },
            { "message", message },
            { "is_schedule", "false" },
            { "schedule_date", "" }
        };
                    string url = $"{endPoint}?key={apiKey}";
                    var options = new RestClientOptions(url);
                    var client = new RestClient(options);
                    var request = new RestRequest("");
                    request.AddHeader("Accept", "application/json");
                    request.AddJsonBody(data);

                    var response = await client.PostAsync(request);
                }
                if(type == "email" && _webHostEnvironment.IsProduction())
                {
                    SharedFunctions.SendEmail(value,"Rentwise Registration Token",message,false);
                }
                OtpVerification otpVerification = new OtpVerification
                {
                    Value = value,
                    OTP = otp,
                };
                OtpVerification oldOtpVerification = _unitOfWork.Otp.Get(u=>u.Value == value);
                if(oldOtpVerification != null)
                {
                    otpVerification.UpdatedAt = DateTime.Now;
                    _unitOfWork.Otp.Update(otpVerification);
                } else
                {
                    _unitOfWork.Otp.Add(otpVerification);
                }
                _unitOfWork.Save();
                return Json(new
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = "Message Sent Successfully",
                    Data = "Ok",
                    Success = true
                });
            }
            catch (Exception err)
            {
                return Json(new
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = Lookup.ResponseMessages[1],
                    Data = "Internal Server Error",
                    Success = false
                });
            }
        }

    }
}
