using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using RentWise.Models;
using RentWise.Models.Identity;
using RentWise.Utility;
using System.Collections;
using System.Diagnostics;

namespace RentWise.Agent.Controllers
{
    public class HomeController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ILogger<HomeController> logger, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            if (!_signInManager.IsSignedIn(User))
            {
             return RedirectToAction("Register","Auth");
            }
          

            IdentityUser user = await _userManager.GetUserAsync(User);
            bool isUser = await _userManager.IsInRoleAsync(user, Lookup.Roles[3]);
            if (!isUser)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            ViewBag.UserId = user.Id;
            AgentRegistrationModel model = new AgentRegistrationModel();
            if (TempData.Get<AgentRegistrationModel>("Model") != null)
            {
                model = TempData.Get<AgentRegistrationModel>("Model");

            }

            IEnumerable<string> errorMessages = TempData["ErrorMessages"] as IEnumerable<string>;

            if (errorMessages != null && errorMessages.Any())
            {
                foreach (var errorMessage in errorMessages)
                {
                    ModelState.AddModelError("", errorMessage); // Add errors back to ModelState
                }
            }

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}