using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RentWise.DataAccess.Repository;
using RentWise.DataAccess.Repository.IRepository;
using RentWise.Models;
using RentWise.Models.Identity;
using RentWise.Utility;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace RentWise.Controllers
{
    //[Authorize(Roles="Admin")]
    public class AdminController : Controller
    {
        public readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly Microsoft.AspNetCore.SignalR.IHubContext<SignalRHub> _hubContext;


        public AdminController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, IWebHostEnvironment webHostEnvironment, Microsoft.AspNetCore.SignalR.IHubContext<SignalRHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _hubContext = hubContext;
        }
        public IActionResult Index()
        {
            IEnumerable<UsersDetailsModel> usersDetails = _unitOfWork.UsersDetails.GetAll(u => true, includeProperties: "Agent");
            return View(usersDetails);
        }

        public async Task<IActionResult> DeactivateAll(string Id)
        {
            IdentityUser user = await _userManager.FindByIdAsync(Id);
            if (user != null)
            {
                user.LockoutEnabled = true;
                await _userManager.UpdateAsync(user);
            }
            UsersDetailsModel usersDetails = _unitOfWork.UsersDetails.Get(u => u.Id == Id);
            usersDetails.Enabled = false;
            usersDetails.UpdatedAt = DateTime.Now;
            _unitOfWork.UsersDetails.Update(usersDetails);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ActivateAll(string Id)
        {
            IdentityUser user = await _userManager.FindByIdAsync(Id);
            if (user != null)
            {
                user.LockoutEnabled = false;
                await _userManager.UpdateAsync(user);
            }
            UsersDetailsModel usersDetails = _unitOfWork.UsersDetails.Get(u => u.Id == Id);
            usersDetails.Enabled = true;
            usersDetails.UpdatedAt = DateTime.Now;
            _unitOfWork.UsersDetails.Update(usersDetails);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeactivateAgent(string Id)
        {
            AgentRegistrationModel agent = _unitOfWork.AgentRegistration.Get(u => u.Id == Id);
            agent.Enabled = false;
            agent.UpdatedAt = DateTime.Now;
            _unitOfWork.AgentRegistration.Update(agent);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> ActivateAgent(string Id)
        {
            AgentRegistrationModel agent = _unitOfWork.AgentRegistration.Get(u => u.Id == Id);
            agent.Enabled = true;
            agent.UpdatedAt = DateTime.Now;
            _unitOfWork.AgentRegistration.Update(agent);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> MakeAdmin(string Id)
        {
            IdentityUser user = await _userManager.FindByIdAsync(Id);
            if (user != null)
            {
                await _userManager.AddToRoleAsync(user, Lookup.Roles[1]);

                await _userManager.RemoveFromRoleAsync(user, Lookup.Roles[3]);
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> RemoveAdmin(string Id)
        {
            IdentityUser user = await _userManager.FindByIdAsync(Id);
            if (user != null)
            {
                await _userManager.AddToRoleAsync(user, Lookup.Roles[3]);

                await _userManager.RemoveFromRoleAsync(user, Lookup.Roles[1]);
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Contact()
        {
            IEnumerable<ContactAdminModel> contactAdmins = _unitOfWork.ContactAdmin.GetAll();
            return View(contactAdmins);
        }

        public async Task<IActionResult> Preview(string Id)
        {
            UsersDetailsModel usersDetails = _unitOfWork.UsersDetails.Get(u => u.Id == Id, "Agent");
            IdentityUser user = await _userManager.FindByIdAsync(Id);
            ViewBag.User = user;
            string profilePicture = $"images/{usersDetails.Id}";
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string finalPath = Path.Combine(wwwRootPath, profilePicture);
            if (Directory.Exists(finalPath))
            {
                profilePicture = $"~/images/{usersDetails.Id}/{String.Join("", Lookup.Upload[5].Split(" "))}.png";
            }
            else
            {
                profilePicture = "~/img/profile.png";
            }
            ViewBag.ProfilePicture = profilePicture;
            IEnumerable<ProductModel> products = _unitOfWork.Product.GetAll(u => u.AgentId == Id);
            ViewBag.Products = products;
            return View(usersDetails);
        }

        public async Task<ActionResult> SendNotificationToAll(string message)
        {
            _hubContext.Clients.All.SendAsync("ReceiveMessage", "all", message);
            SharedFunctions.SendPushNotification("All", "Message from rentwise", message);
            return RedirectToAction(nameof(Contact));
        }
        public async Task<IActionResult> ReplyEmail(string name, string email, string header, string body,int Id)
        {
            string emailContent = SharedFunctions.EmailContentReply(name, header, body);
            ContactAdminModel contact = _unitOfWork.ContactAdmin.Get(u => u.Id == Id);
            contact.Enabled = false;
            _unitOfWork.ContactAdmin.Update(contact);
            _unitOfWork.Save();
            if (email == "All")
            {
                var users = await _userManager.Users.ToListAsync();
                foreach (var user in users)
                {
                    SharedFunctions.SendEmail(user.Email, "  Message from rentwise", emailContent);
                }

            }
            else
            {
                SharedFunctions.SendEmail(email, "  Message from rentwise", emailContent);
            }
            return RedirectToAction(nameof(Contact));
        }
    }

}
