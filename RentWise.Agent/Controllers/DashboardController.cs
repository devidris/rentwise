using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RentWise.DataAccess.Repository.IRepository;
using RentWise.Models;
using RentWise.Models.Identity;
using RentWise.Utility;
using System.Net;
using System.Security.Claims;

namespace RentWise.Agent.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public DashboardController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index(string Id = "")
        {
            if (TempData["Action"] != null)
            {
            ViewBag.Action = TempData["Action"];
            } else
            {
                ViewBag.Action = 1;
            }
            var errorMessages = TempData["ErrorMessages"];
            if (errorMessages != null)
            {
                // Check if the object is a string array
                if (errorMessages is string[] errorMessageArray)
                {
                    foreach (var errorMessage in errorMessageArray)
                    {
                        ModelState.AddModelError(string.Empty, errorMessage);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, errorMessages.ToString());
                }
            }
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value; 
            AgentRegistrationModel agent = _unitOfWork.AgentRegistration.Get(u => u.Id == userId);
            if(agent != null)
            {
                agent.ReturnController = "Dashboard";
                agent.ReturnAction = "Index";
            }
            ViewBag.Agent = agent;
            IEnumerable<OrdersModel> orders = _unitOfWork.Order.GetAll(u=>u.AgentId == userId,"Product");
            ViewBag.Orders = orders;
            ViewBag.NoOfOrders = orders.Count();  
            ViewBag.JSONOrders = JsonConvert.SerializeObject(orders);
            IEnumerable<ReviewModel> reviews = _unitOfWork.Review.GetAll(u => u.AgentId == userId,"User,Product");
            ViewBag.Reviews = reviews;
            IEnumerable<LikeModel> likes = _unitOfWork.Like.GetAll(u=> u.AgentId == userId);
            ViewBag.Likes = likes;

            IEnumerable<OrdersModel> filteredOrders = orders.Where(order => order.LkpStatus == 4);
            ViewBag.NoOfFilteredOrders = filteredOrders.Count();
            IEnumerable<OrdersModel> pendingOrders = orders.Where(order => order.LkpStatus == 1 || order.LkpStatus == 2 || order.LkpStatus == 7);
            ViewBag.NoOfPendingOrders = pendingOrders.Count();
             ViewBag.totalAmount = filteredOrders.Any() ? filteredOrders.Sum(order => order.TotalAmount) : 0;
            ViewBag.averageTotalAmount = filteredOrders.Any() ? filteredOrders.Average(order => order.TotalAmount) : 0;
            ViewBag.totalPendingAmount = pendingOrders.Any() ? pendingOrders.Sum(order => order.TotalAmount) : 0;

            ViewBag.totalReview = reviews.Any() ? reviews.Count() : 0;
            ViewBag.avergeRating = reviews.Any() ? reviews.Average(review => review.RatingValue) : 0;

            ViewBag.totalLikes = likes.Count();

            ViewBag.UserId = userId;

            var userChats = _unitOfWork.Chat
             .GetAll()
             .Where(c => c.FromUserId == userId || c.ToUserId == userId)
             .GroupBy(c => c.FromUserId == userId ? c.ToUserId : c.FromUserId)
             .ToList();

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
                if (group.Key != userId)
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
        public IActionResult ApproveOrReject(int Id,int LkpStatus)
        {
            string UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            OrdersModel order = _unitOfWork.Order.Get(u => u.OrderId == Id && u.AgentId == UserId);
            if (order != null)
            {
                UsersDetailsModel usersDetailsModel = _unitOfWork.UsersDetails.Get(u=>u.Id == order.UserId);
                if (usersDetailsModel != null)
                {
                    usersDetailsModel.Orders += 1;
                    usersDetailsModel.Messages += 1;
                    _unitOfWork.UsersDetails.Update(usersDetailsModel);
                    _unitOfWork.Save();
                }
                string Message = "Your order has been rejected";
                if (Id == 2)
                {
                  Message = "Your order has been accepted";
                }  
                ChatModel chat = new()
                {
                    FromUserId = order.AgentId,
                    ToUserId = order.UserId,
                    Message = Message,
                    IsOrder = true,
                };

                _unitOfWork.Chat.Add(chat);
                order.LkpStatus = LkpStatus;
                _unitOfWork.Order.Update(order);
                _unitOfWork.Save();
            }
            TempData["active"] = 4;
            return RedirectToAction("Index","Dashboard");
        }
        public IActionResult PaymentReceived(int Id)
        {
            string UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            OrdersModel order = _unitOfWork.Order.Get(u => u.OrderId == Id && u.AgentId == UserId);
            if (order != null)
            {
                string Message = "Payment received by renter";
                ChatModel chat = new()
                {
                    FromUserId = order.AgentId,
                    ToUserId = order.UserId,
                    Message = Message,
                    IsOrder = true,
                };

                _unitOfWork.Chat.Add(chat);
                order.LkpStatus = 4;
                order.Paid = true;
                _unitOfWork.Order.Update(order);
                _unitOfWork.Save();
            }
            return RedirectToAction("Index", "Dashboard");
        }

        [HttpPost]
        [Authorize]
        [AllowAnonymous]
        public ActionResult SendMessage(string Receipient, string Message)
        {
            try
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return Json(new
                    {
                        StatusCode = (int)HttpStatusCode.Unauthorized,
                        Message = Lookup.ResponseMessages[4],
                        Data = "Unauthorized",
                        Success = false
                    });
                }
                ChatModel chat = new()
                {
                    FromUserId = userId,
                    ToUserId = Receipient,
                    Message = Message
                };
                UsersDetailsModel usersDetailsModel = _unitOfWork.UsersDetails.Get(u => u.Id == Receipient);
                if (usersDetailsModel != null)
                {
                    usersDetailsModel.Messages += 1;
                    _unitOfWork.UsersDetails.Update(usersDetailsModel);
                }
                _unitOfWork.Chat.Add(chat);
                _unitOfWork.Save();
                return Json(new
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = "Message sent successfully",
                    Data = Lookup.ResponseMessages[5],
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = Lookup.ResponseMessages[1],
                    Data = "Server Error",
                    Success = false
                });
            }
        }
    }
}
