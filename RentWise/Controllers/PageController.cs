using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RentWise.DataAccess.Repository.IRepository;
using RentWise.Models;
using RentWise.Models.Identity;
using RentWise.Utility;
using System.Security.Claims;

namespace RentWise.Controllers
{
    public class PageController : Controller
    {
        public readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public PageController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, IWebHostEnvironment webHostEnvironment)
        {

            _unitOfWork = _unitOfWork = unitOfWork;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult How()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }
        [Authorize]
        public async Task<IActionResult> Chat(string Id = "")
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            ViewBag.UserId = userId;

            var userChats = _unitOfWork.Chat
             .GetAll()
             .Where(c => c.FromUserId == userId || c.ToUserId == userId)
             .GroupBy(c => c.FromUserId == userId ? c.ToUserId : c.FromUserId)
             .ToList();

            UsersDetailsModel usersDetailsModel = _unitOfWork.UsersDetails.Get(u => u.Id == userId);
            if (usersDetailsModel != null)
            {
                usersDetailsModel.Messages = 0;
                _unitOfWork.UsersDetails.Update(usersDetailsModel);
                _unitOfWork.Save();
            }

            List<ChatSummary> chatSummaries = new List<ChatSummary>();

            // Mark all unread messages as read
            var unReadMessage = _unitOfWork.Chat.GetAll(u => (u.FromUserId == Id && u.ToUserId == userId)).ToList();
            foreach (var item in unReadMessage)
            {
                item.IsRead = true;
                _unitOfWork.Chat.Update(item);
            }   
           _unitOfWork.Save();

            foreach (var group in userChats)
            {
                if(group.Key != userId)
                {
                    var user = await _userManager.FindByIdAsync(group.Key);
                    string profilePicture = $"/images/{group.Key}";
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string finalPath = Path.Combine(wwwRootPath, profilePicture);
                    if (Directory.Exists(finalPath))
                    {
                        profilePicture = $"/images/{group.Key}/{String.Join("", Lookup.Upload[5].Split(" "))}.png";
                    }
                    else
                    {
                        profilePicture = "/img/profile.png";
                    }
                    string lastMessage = group.OrderByDescending(c => c.CreatedAt).FirstOrDefault()?.Message ?? string.Empty;

                    // Truncate the message if it exceeds 15 characters
                    string truncatedMessage = lastMessage.Length > 15 ? lastMessage.Substring(0, 15) + "..." : lastMessage;
                    var lastMessageTime = group.OrderByDescending(c => c.CreatedAt).FirstOrDefault()?.CreatedAt;
                    // Calculate the difference in minutes
                    int minutesAgo = (int)(DateTime.Now - lastMessageTime.GetValueOrDefault()).TotalMinutes;

                    // Set LastMessageMinutesAgo to 0 if it's less than 1
                    int lastMessageMinutesAgo = minutesAgo < 1 ? 0 : minutesAgo;

                    ChatSummary chatSummary = new ChatSummary
                    {
                        UserId = group.Key,
                        User = user,
                        LastMessage = group.OrderByDescending(c => c.CreatedAt).FirstOrDefault(),
                        LastChat = lastMessageMinutesAgo < 1 ? "now" : SharedFunctions.FormatDuration(TimeSpan.FromMinutes(lastMessageMinutesAgo)),
                        UnreadMessageCount = group.Count(c => !c.IsRead && c.ToUserId == userId),
                        ProfilePicture = profilePicture
                    };

                    chatSummaries.Add(chatSummary);
                }
            }
            ViewBag.IsChatEmpty = chatSummaries.Count() == 0;



            List<ChatModel> FullMessage = new List<ChatModel>();
            if (String.IsNullOrEmpty(Id))
            {
                ViewBag.OpenChat = false;
                ViewBag.FullMessage = JsonConvert.SerializeObject(FullMessage);
                ViewBag.ChatSummaries = JsonConvert.SerializeObject(chatSummaries);
                return View();
            }
            else
            {
                ViewBag.OpenChat = true;
                FullMessage = _unitOfWork.Chat.GetAll(u => (u.FromUserId == userId && u.ToUserId == Id) || (u.ToUserId == userId && u.FromUserId == Id)).ToList();
            ViewBag.ReceipientId = Id;
            }
            ViewBag.FullMessage = JsonConvert.SerializeObject(FullMessage);
            ViewBag.ChatSummaries = JsonConvert.SerializeObject(chatSummaries);
            return View();
        }

    }
}
