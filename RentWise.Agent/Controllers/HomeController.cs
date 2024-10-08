﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using RentWise.DataAccess.Repository;
using RentWise.DataAccess.Repository.IRepository;
using RentWise.Models;
using RentWise.Models.Identity;
using RentWise.Utility;
using System.Collections;
using System.Diagnostics;
using System.IO.Compression;

namespace RentWise.Agent.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        public readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager,IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index(string? onesignalId)
        {
            IdentityUser user = await _userManager.GetUserAsync(User);
            
            bool isUserAdminOrAgent = await _userManager.IsInRoleAsync(user, Lookup.Roles[1]) ||
                           await _userManager.IsInRoleAsync(user, Lookup.Roles[2]);
            AgentRegistrationModel agentDetails = _unitOfWork.AgentRegistration.Get(u => u.Id == user.Id);
            if (isUserAdminOrAgent && agentDetails != null)
            {
                if(onesignalId != null && onesignalId != "null")
                {
                    return RedirectToAction("Index", "Dashboard", new { onesignalId = onesignalId, message = "Login Successful" });
                } else
                {
                return RedirectToAction("Index", "Dashboard", new { message = "Login Successful" });
                }
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
            IEnumerable<State> states = _unitOfWork.State.GetAll(u => u.StateId != null, "Cities");
            foreach (var state in states)
            {
                state.Name = SharedFunctions.Capitalize(state.Name);
            }
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            ViewBag.States = states;
            ViewBag.JSONStates = JsonConvert.SerializeObject(states, settings);
            ViewBag.Categories = Lookup.Categories;
            model.ShowFooter = true;
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

        public IActionResult DownloadWwwroot()
        {
            // Temp file path
            string tempFilePath = Path.Combine(Path.GetTempPath(), "wwwroot.zip");

            // Ensure wwwroot directory exists
            string wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            if (!Directory.Exists(wwwrootPath))
            {
                return NotFound("The wwwroot directory does not exist.");
            }

            // Create a new ZIP file
            if (System.IO.File.Exists(tempFilePath))
            {
                System.IO.File.Delete(tempFilePath);
            }

            ZipFile.CreateFromDirectory(wwwrootPath, tempFilePath);

            // Return the file to download
            var bytes = System.IO.File.ReadAllBytes(tempFilePath);
            return File(bytes, "application/zip", "wwwroot.zip");
        }
    }
}