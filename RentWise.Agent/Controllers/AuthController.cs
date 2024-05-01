using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using RentWise.Models.Identity;
using System.Text.Encodings.Web;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using RentWise.Utility;
using System.Linq;
using RentWise.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using RentWise.Agent;
using RestSharp;
using RentWise.Models;
using System.Net;
using System.Web;

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
        private readonly IAgentRegistrationRepository _agentRegitration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IUnitOfWork _unitOfWork;
        public AuthController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<Authentication> logger,
            IEmailSender emailSender,
            IAgentRegistrationRepository agentRegistration,
            IWebHostEnvironment webHostEnvironment,
            IUnitOfWork unitOfWork)

        {

            _userManager = userManager;
            _roleManager = roleManager;
            _userStore = userStore;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _agentRegitration = agentRegistration;
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
            UsersDetailsModel usersDetails = _unitOfWork.UsersDetails.Get(u => u.PhoneNumber == model.PhoneNumber);
            if(usersDetails != null)
            {
                ModelState.AddModelError(string.Empty, "Phone number has been used to register an account");
            }

            if (phoneOtp == null)
            {
                ModelState.AddModelError(string.Empty, "Phone number otp is wrong");
            } else
            {
            if (HasPassed10Minutes(phoneOtp.UpdatedAt))
            {
                ModelState.AddModelError(string.Empty, "Phone number otp has expired");
            }

            }

            if (ModelState.IsValid)
            {
                if (!_roleManager.RoleExistsAsync(Lookup.Roles[1]).GetAwaiter().GetResult())
                {
                    _roleManager.CreateAsync(new IdentityRole(Lookup.Roles[1])).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new IdentityRole(Lookup.Roles[2])).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new IdentityRole(Lookup.Roles[3])).GetAwaiter().GetResult();
                }
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, model.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    await _userManager.AddToRoleAsync(user, Lookup.Roles[3]);
                    UsersDetailsModel usersDetailsModel = new UsersDetailsModel
                    {
                        Email = model.Email,
                        PhoneNumber = model.PhoneNumber,
                        Username = model.Email.Split('@')[0],
                        Id = user.Id,
                    };
                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = model.Email, returnUrl = model.ReturnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        string[] parts = model.ReturnUrl.Split('?');
                        if (parts.Length > 1)
                        {
                            // Get the query parameters
                            string query = parts[1];

                            // Parse the query string
                            var parsedQuery = HttpUtility.ParseQueryString(query);

                            // Get the value of the 'oneSignal' parameter
                            string oneSignalValue = parsedQuery["onesignalId"] ?? "Rentwise";

                            if (!string.IsNullOrEmpty(oneSignalValue) && oneSignalValue != "null")
                            {
                                usersDetailsModel.OneSignalId = oneSignalValue;
                                _unitOfWork.UsersDetails.Add(usersDetailsModel);
                                _unitOfWork.Save();
                                return RedirectToAction("Index", "Home", new { onesignalId = oneSignalValue, message = "Regitration Successful" });
                            }
                            else
                            {
                                _unitOfWork.UsersDetails.Add(usersDetailsModel);
                                _unitOfWork.Save();
                                return RedirectToAction("Index", "Home", new { message = "Regitration Successful" });
                            }
                        }
                        _unitOfWork.UsersDetails.Add(usersDetailsModel);
                        _unitOfWork.Save();
                        return RedirectToAction("Index", "Home", new { message = "Regitration Successful" });
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
            UsersDetailsModel usersDetails = _unitOfWork.UsersDetails.Get(u => u.PhoneNumber == model.Email);
            if (usersDetails!= null)
            {
                model.Email = usersDetails.Email;
            }

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    string[] parts = model.ReturnUrl.Split('?');
                    if (parts.Length > 1)
                    {
                        // Get the query parameters
                        string query = parts[1];

                        // Parse the query string
                        var parsedQuery = HttpUtility.ParseQueryString(query);

                        // Get the value of the 'oneSignal' parameter
                        string oneSignalValue = parsedQuery["onesignalId"] ?? "Rentwise";

                        if (!string.IsNullOrEmpty(oneSignalValue) && oneSignalValue != "null")
                        {
                            usersDetails = _unitOfWork.UsersDetails.Get(u => u.Email == model.Email);
                            if (usersDetails != null)
                            {
                                usersDetails.OneSignalId = oneSignalValue;
                                _unitOfWork.UsersDetails.Update(usersDetails);
                                _unitOfWork.Save();
                            }
                                return RedirectToAction("Index", "Home", new { onesignalId = oneSignalValue, message = "Login Successful"  });
                       
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home", new { message = "Regitration Successful" });
                        }


                    }
                    return RedirectToAction("Index", "Home", new { message = "Regitration Successful" });
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

        [Authorize]
        public async Task<IActionResult> RegisterAgent(AgentRegistrationModel model, IFormFile? logo, IFormFile? nationalCard)
        {
            IdentityUser user = await _userManager.GetUserAsync(User);
            if (!model.Privacy)
            {
                ModelState.AddModelError("Privacy", "You have to accept terms and conditions.");
            }
            TempData["Action"] = 6;
            bool isCreate = model.Id == null;
            AgentRegistrationModel agent = _unitOfWork.AgentRegistration.Get(u => u.Id == user.Id);
            bool changeNumber = true;
            if (agent != null)
            {
                changeNumber = agent.PhoneNumber != model.PhoneNumber;
            }
            bool changeSlug = true;
            if (agent != null)
            {
                changeSlug = agent.Slug != model.Slug;
            }
            bool changeStoreName = true;
            if (agent != null)
            {
                changeStoreName = agent.StoreName != model.StoreName;
            }
            if (isCreate)
            {
                if (logo == null || logo.Length < 1)
                {
                    ModelState.AddModelError(Lookup.Upload[1], "Logo upload is compulsory.");
                }
                if (logo!= null && logo.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError(Lookup.Upload[1], "Logo cannot be more than 5 MB.");
                }
                    if (nationalCard == null || nationalCard.Length < 1)
                {
                    ModelState.AddModelError(string.Join("", Lookup.Upload[4].Split(" ")), "National Card upload is compulsory.");
                }
                if (nationalCard != null && nationalCard.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError(Lookup.Upload[4], "National Card cannot be more than 5 MB.");
                }
            }

            if (_unitOfWork.AgentRegistration.Get(u => u.PhoneNumber == model.PhoneNumber) != null && changeNumber)
            {
                ModelState.AddModelError(string.Join("", Lookup.Upload[5].Split(" ")), "Phone Number is already in use.");
            }
            if (_unitOfWork.AgentRegistration.Get(u => u.Slug == model.Slug) != null && changeSlug)
            {
                ModelState.AddModelError(Lookup.Upload[6], "Slug is already in use.");
            }
            if (_unitOfWork.AgentRegistration.Get(u => u.StoreName == model.StoreName) != null && changeStoreName)
            {
                ModelState.AddModelError(string.Join("", Lookup.Upload[7].Split(" ")), "Store Name is already in use.");
            }

            if (ModelState.IsValid)
            {
                if (isCreate)
                {
                    model.Id = user.Id;
                    model.FirstName = SharedFunctions.Capitalize(model.FirstName);
                    model.LastName = SharedFunctions.Capitalize(model.LastName);
                }
                else
                {
                    model.FirstName = SharedFunctions.Capitalize(agent.FirstName);
                    model.LastName = SharedFunctions.Capitalize(agent.LastName);
                }
                model.StoreName = SharedFunctions.Capitalize(model.StoreName);

                if (logo != null)
                {
                    #region Saving Logo
                    string logoName = Lookup.Upload[1] + Path.GetExtension(logo.FileName);

                    saveImage(model.Id, logoName, logo);
                    #endregion
                }
                if (nationalCard != null)
                {
                    #region Saving National Card
                    string nationalCardName = String.Join("", Lookup.Upload[4].Split(" ")) + Path.GetExtension(nationalCard.FileName);

                    saveImage(model.Id, nationalCardName, nationalCard);
                    #endregion
                }
                
                if (model.Slug == null) { model.Slug = model.Id.ToString(); }

                if (isCreate)
                {
                    _unitOfWork.AgentRegistration.Add(model);

                    IList<string> userRoles = await _userManager.GetRolesAsync(user);
                    await _userManager.AddToRoleAsync(user, Lookup.Roles[2]);
                    await _userManager.RemoveFromRoleAsync(user, Lookup.Roles[3]);
                }
                else
                {
                    TempData["Success"] = "Update Successful";
                    model.UpdatedAt = DateTime.Now;
                    _unitOfWork.AgentRegistration.Update(model);
                }
                _unitOfWork.Save();


                return RedirectToAction("Login", "Auth", new { message = "Owner Regitration Successful, login again to activate accouunt" });
            }
            TempData.Put("Model", model);
            List<string> errorMessages = ModelState.Values
          .SelectMany(v => v.Errors)
          .Select(e => e.ErrorMessage)
          .ToList();

            TempData["ErrorMessages"] = errorMessages; // Store the error messages in TempData
            if (isCreate)
            {
               return RedirectToAction("Login", "Auth", new { message = "Owner Regitration Successful, login again to activate account" });
            }
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "Home");
            }
           return RedirectToAction("Login", "Auth", new { message = "Owner Regitration Successful, login again to activate account" });
        }

        public void saveImage(string userId, string fileName, IFormFile file)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string filePath = @"images\agent\" + userId + "\\registration";
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

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> SendOtp(string type, string value)
        {
            try
            {
                string otp = SharedFunctions.GenerateOTP();
                string message = $"Your verification code is: {otp}";
                if (_webHostEnvironment.IsProduction())
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
                if (type == "email" && _webHostEnvironment.IsProduction())
                {
                    SharedFunctions.SendEmail(value, "Rentwise Registration Token", message, false);
                }
           
                OtpVerification oldOtpVerification = _unitOfWork.Otp.Get(u => u.Value == value);
                if (oldOtpVerification != null)
                {
                    oldOtpVerification.Value = value;
                    oldOtpVerification.OTP = otp;
                    oldOtpVerification.UpdatedAt = DateTime.Now;
                    _unitOfWork.Otp.Update(oldOtpVerification);
                }
                else
                {
                    OtpVerification otpVerification = new OtpVerification
                    {
                        Value = value,
                        OTP = otp,
                    };
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
