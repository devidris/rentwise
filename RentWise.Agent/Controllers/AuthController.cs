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

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Authentication model)
        {

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
                    var userId = await _userManager.GetUserIdAsync(user);
                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = model.Email, returnUrl = model.ReturnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);

                        return LocalRedirect("/Home/Index");

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

        public async Task<IActionResult> RegisterAgent(AgentRegistrationModel model, IFormFile logo, IFormFile banner, IFormFile passport, IFormFile nationalCard, IFormFile profilePicture)
        {
            IdentityUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Register", "Login");
            }
            if (!model.Privacy)
            {
                ModelState.AddModelError("Privacy", "You have to accept terms and conditions.");
            }
            model.UserId = user.Id;

            if (logo == null)
            {
                ModelState.AddModelError(Lookup.Upload[1], "Logo upload is compulsory.");
            }

            if (banner == null)
            {
                ModelState.AddModelError(Lookup.Upload[2], "Banner upload is compulsory.");
            }
            if (passport == null)
            {
                ModelState.AddModelError(Lookup.Upload[3], "Passport upload is compulsory.");
            }
            if (nationalCard == null)
            {
                ModelState.AddModelError(string.Join("", Lookup.Upload[4].Split(" ")), "National Card upload is compulsory.");
            }
            if (profilePicture == null)
            {
                ModelState.AddModelError(string.Join("", Lookup.Upload[5].Split(" ")), "Profile Picture upload is compulsory.");
            }

            if (_unitOfWork.AgentRegistration.Get(u => u.PhoneNumber == model.PhoneNumber) != null)
            {
                ModelState.AddModelError(string.Join("", Lookup.Upload[5].Split(" ")), "Phone Number is already in use.");
            }
            if (_unitOfWork.AgentRegistration.Get(u => u.Slug == model.Slug) != null)
            {
                ModelState.AddModelError(Lookup.Upload[6], "Slug is already in use.");
            }
            if (_unitOfWork.AgentRegistration.Get(u => u.StoreName == model.StoreName) != null)
            {
                ModelState.AddModelError(string.Join("", Lookup.Upload[7].Split(" ")), "Store Name is already in use.");
            }

            if (ModelState.IsValid)
            {
                #region Saving Logo
                string logoName = Lookup.Upload[1] + Path.GetExtension(logo.FileName);

                saveImage(model.UserId, logoName, logo);
                #endregion
                #region Saving Banner
                string bannerName = Lookup.Upload[2] + Path.GetExtension(banner.FileName);

                saveImage(model.UserId, bannerName, banner);
                #endregion
                #region Saving Passport
                string passportName = Lookup.Upload[3] + Path.GetExtension(passport.FileName);

                saveImage(model.UserId, passportName, passport);
                #endregion
                #region Saving National Card
                string nationalCardName = String.Join("", Lookup.Upload[4].Split(" ")) + Path.GetExtension(nationalCard.FileName);

                saveImage(model.UserId, nationalCardName, nationalCard);
                #endregion
                #region Saving Profile Picture
                string profilePictureName = String.Join("", Lookup.Upload[5].Split(" ")) + Path.GetExtension(profilePicture.FileName);

                saveImage(model.UserId, profilePictureName, profilePicture);
                #endregion
                if (model.Slug == null) { model.Slug = model.UserId.ToString(); }

                _unitOfWork.AgentRegistration.Add(model);
                _unitOfWork.Save();

                IList<string> userRoles = await _userManager.GetRolesAsync(user);
                await _userManager.AddToRoleAsync(user, Lookup.Roles[2]);
                await _userManager.RemoveFromRoleAsync(user, Lookup.Roles[3]);



                return RedirectToAction("Index", "Store");
            }
            TempData.Put("Model", model);
            List<string> errorMessages = ModelState.Values
          .SelectMany(v => v.Errors)
          .Select(e => e.ErrorMessage)
          .ToList();

            TempData["ErrorMessages"] = errorMessages; // Store the error messages in TempData
            return RedirectToAction("Index", "Home");
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


    }
}
