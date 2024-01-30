using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RentWise.DataAccess.Repository.IRepository;
using RentWise.Models;
using RentWise.Models.Identity;
using RentWise.Utility;
using System.Net;
using System.Security.Claims;
using RestSharp;
using static NuGet.Packaging.PackagingConstants;

namespace RentWise.Agent.Controllers
{
    [Authorize(Roles = "Admin, Agent")]
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IOptions<RentWiseConfig> _config;
        public DashboardController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, IWebHostEnvironment webHostEnvironment, IOptions<RentWiseConfig> config)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _config = config;
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
            ViewBag.Banks = Lookup.Banks;
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
            IEnumerable<ReviewModel> reviews = _unitOfWork.Review.GetAll(u => u.AgentId == userId,"UserDetails,Product");
            ViewBag.Reviews = reviews;
            IEnumerable<LikeModel> likes = _unitOfWork.Like.GetAll(u=> u.AgentId == userId);
            ViewBag.Likes = likes;

            IEnumerable<OrdersModel> filteredOrders = orders.Where(order => order.LkpStatus == 4);
            ViewBag.NoOfFilteredOrders = filteredOrders.Count();
            IEnumerable<OrdersModel> pendingOrders = orders.Where(order => order.LkpStatus == 1 || order.LkpStatus == 2 || order.LkpStatus == 7);
            ViewBag.NoOfPendingOrders = pendingOrders.Count();
             ViewBag.totalAmount = Math.Round(filteredOrders.Any() ? filteredOrders.Sum(order => order.TotalAmount) : 0, 2);
            ViewBag.averageTotalAmount =Math.Round(filteredOrders.Any() ? filteredOrders.Average(order => order.TotalAmount) : 0,2);
            ViewBag.totalPendingAmount = Math.Round(pendingOrders.Any() ? pendingOrders.Sum(order => order.TotalAmount) : 0,2);

            IEnumerable<WithdrawalHistoryModel> withdrawals = _unitOfWork.WithdrawalHistory.GetAll(u => u.AgentId == userId);
            ViewBag.Withdrawals = withdrawals;


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
                item.UpdatedAt = DateTime.Now;
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
            OrdersModel order = _unitOfWork.Order.Get(u => u.OrderId == Id && u.AgentId == UserId,"Product");
            if (order != null)
            {
                UsersDetailsModel usersDetailsModel = _unitOfWork.UsersDetails.Get(u=>u.Id == order.UserId);
                if (usersDetailsModel != null)
                {
                    usersDetailsModel.Orders += 1;
                    usersDetailsModel.Messages += 1;
                    usersDetailsModel.UpdatedAt = DateTime.Now;
                    _unitOfWork.UsersDetails.Update(usersDetailsModel);
                    _unitOfWork.Save();
                }
                string Message = "Your order has been rejected";
                string emailContentClient = String.Empty;
                if (Id == 2)
                {
                  Message = "Your order has been accepted";
                    emailContentClient = SharedFunctions.EmailContent(usersDetailsModel.Username, 3, order.Product.Name, order.ProductQuantity, order.TotalAmount);
                }   else
                {
                    emailContentClient = SharedFunctions.EmailContent(usersDetailsModel.Username, 4, order.Product.Name, order.ProductQuantity, order.TotalAmount);
                }
                    SharedFunctions.SendEmail(usersDetailsModel.Username, "There is an update on your Reservation", emailContentClient);
                ChatModel chat = new()
                {
                    FromUserId = order.AgentId,
                    ToUserId = order.UserId,
                    Message = Message,
                    IsOrder = true,
                };

                _unitOfWork.Chat.Add(chat);
                order.LkpStatus = LkpStatus;
                order.UpdatedAt = DateTime.Now;
                _unitOfWork.Order.Update(order);
                _unitOfWork.Save();
            }
            TempData["Action"] = 4;
            return RedirectToAction("Index","Dashboard");
        }
        public async Task<IActionResult> PaymentReceived(int Id)
        {
            string UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            IdentityUser user = await _userManager.FindByIdAsync(UserId);
            UsersDetailsModel usersDetails = _unitOfWork.UsersDetails.Get(u=>u.Id == UserId);
            OrdersModel order = _unitOfWork.Order.Get(u => u.OrderId == Id && u.AgentId == UserId);
            AgentRegistrationModel agent = _unitOfWork.AgentRegistration.Get(u => u.Id == order.AgentId, "User");
            string agentPhoneNumber = agent.PhoneNumber;
            if (order != null)
            {
                string Message = "Payment received by renter, contact renter on"+  agentPhoneNumber;
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
                order.UpdatedAt = DateTime.Now;
                _unitOfWork.Order.Update(order);
                agent.PayWithCash += order.TotalAmount;
                agent.UpdatedAt = DateTime.Now;
                _unitOfWork.AgentRegistration.Update(agent);
                _unitOfWork.Save();
                string emailContentClient = SharedFunctions.EmailContent(usersDetails.Username, 7, order.Product.Name, order.ProductQuantity, order.TotalAmount);
                SharedFunctions.SendEmail(user.UserName, "Payment has  been recieved for Reservation", emailContentClient);
            }
            TempData["Action"] = 4;
            return RedirectToAction("Index", "Dashboard");
        }

        [HttpPost]
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
                    usersDetailsModel.UpdatedAt = DateTime.Now;
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

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Withdrawal(string lkpBank,string name,string acc)
        {
            string UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            AgentRegistrationModel agent = _unitOfWork.AgentRegistration.Get(u => u.Id == UserId);
            bool error = false;
            if(String.IsNullOrEmpty(lkpBank))
            {
                TempData["Error"] = "Please select a bank";
                error = true;
            }
            if(String.IsNullOrEmpty(name))
            {
                TempData["Error"] = "Please enter your name to proceed";
                error = true;
            }
            if (String.IsNullOrEmpty(acc))
            {
                TempData["Error"] = "Please enter your account number";
                error = true;
            }
            if (!error)
            {
                double totalPaidCash = agent.PayWithCash;
                double totalPaidOnline = agent.PayWithCard;
                double totalPaid = totalPaidCash + totalPaidOnline;

                double totalOwingCash = agent.PayWithCash * 0.1;
                double totalOwingOnline = agent.PayWithCard * 0.1;
                double totalOwing = totalOwingCash + totalOwingOnline;

                double totalPending = totalPaidOnline - totalOwing;
                WithdrawalHistoryModel withdrawalHistoryModel = new WithdrawalHistoryModel{
                    AgentId = UserId,
                    WithdrawalAmount = totalPending,
                    FullName = name,
                    LkpBankName = Int32.Parse(lkpBank),
                    AccountDetails = acc,
                    Pending = false
                };
                _unitOfWork.WithdrawalHistory.Add(withdrawalHistoryModel);
                agent.PayWithCard = 0;
                agent.PayWithCash = 0;
                agent.UpdatedAt = DateTime.Now;
                _unitOfWork.AgentRegistration.Update(agent);
                _unitOfWork.Save();
            }
            TempData["Action"] = 7;
            return RedirectToAction("Index", "Dashboard");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Pay()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                TempData["ToastMessage"] = "Please login to pay";
                return Json(new
                {
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                    Message = Lookup.ResponseMessages[4],
                    Data = "Unauthorized",
                    Success = false
                });
            }
            AgentRegistrationModel agent = _unitOfWork.AgentRegistration.Get(u => u.Id == userId);
            Random random = new Random();
            int randomNumber = random.Next(1, 101);
            string reference = agent.Id.ToString() + "-RENTWISE-" + randomNumber;

            // Populate JSON payload from the order variable
            // Create variables for amount, description, and reference
            double totalPaidCash = agent.PayWithCash;
            double totalPaidOnline = agent.PayWithCard;
            double totalPaid = totalPaidCash + totalPaidOnline;

            double totalOwingCash = agent.PayWithCash * 0.1;
            double totalOwingOnline = agent.PayWithCard * 0.1;
            double totalOwing = totalOwingCash + totalOwingOnline;

            double totalPending = totalOwing - totalPaidOnline;
            double totalAmount = totalPending;
                string description = agent.Id; // Adjust this based on your actual structure
                string clientReference = reference;
                string link = _config.Value.ClientWebsiteLink + "/Dashboard/Success?id=" + agent.Id;
            string pageLink = _config.Value.AgentWebsiteLink + "/Dashboard/Index?active=7";
                // Create the JSON string using variables
                string jsonBody = $"{{\"totalAmount\":{totalAmount},\"description\":\"{description}\",\"callbackUrl\":\"{pageLink}?orderId={agent}\",\"returnUrl\":\"{link}\",\"cancellationUrl\":\"{pageLink}\",\"merchantAccountNumber\":\"2018934\",\"clientReference\":\"{clientReference}\"}}";


                var options = new RestClientOptions("https://payproxyapi.hubtel.com/items/initiate");
                var client = new RestClient(options);
                var request = new RestRequest("");
                request.AddHeader("accept", "application/json");
                request.AddHeader("authorization", "Basic bkc2dldCRTo1YzU0NmY1YTZkNTI0Y2VkYjYzOGUzNmQxNmVlYjM5MQ==");
                request.AddJsonBody(jsonBody, false);
                var response = await client.PostAsync(request);
                return Json(new
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = "Order Placed Successfully",
                    Data = JsonConvert.SerializeObject(response),
                    Success = true
                });
        }

        public IActionResult Success(string id = "", string checokutId = "")
        {
            AgentRegistrationModel agent = _unitOfWork.AgentRegistration.Get(u=>u.Id == id);
            agent.PayWithCard = 0;
            agent.PayWithCash = 0;
            agent.UpdatedAt = DateTime.Now;
            _unitOfWork.AgentRegistration.Update(agent);
            _unitOfWork.Save();
            TempData["Success"] = "Payment successful";
            return RedirectToAction(nameof(Index));
        }
    }
}
