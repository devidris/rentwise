using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RentWise.DataAccess.Repository;
using RentWise.DataAccess.Repository.IRepository;
using RentWise.Models;
using RentWise.Models.Identity;
using RentWise.Utility;
using RestSharp;
using System.Net;
using System.Security.Claims;
using System.Text;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace RentWise.Controllers
{
    public class StoreController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOptions<RentWiseConfig> _config;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly Microsoft.AspNetCore.SignalR.IHubContext<SignalRHub> _hubContext;
        public StoreController(IUnitOfWork unitOfWork, IOptions<RentWiseConfig> config, UserManager<IdentityUser> userManager, Microsoft.AspNetCore.SignalR.IHubContext<SignalRHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _config = config;
            _userManager = userManager;
            _hubContext = hubContext;
        }
        public async Task<IActionResult> Index(int Id = 0, string? Sort = null, string? Name = null, string? PriceRange = null, string? MaxDay = null, double Lat = 0, double Lng = 0)
        {
            List<ProductModel> products = new List<ProductModel>();
            if (Id > 0)
            {
                products = _unitOfWork.Product.GetAll(u => u.LkpCategory == Id, "Agent").ToList();
            }
            else
            {
                products = _unitOfWork.Product.GetAll(u => u.LkpCategory > 0, "Agent").ToList();
            }
            if (Name != null)
            {
                products = products.FindAll(product => product.Name.ToLower().Contains(Name.ToLower()));
            }

            if (Sort == "pricelth")
            {
                products = products.OrderBy(product => product.PriceDay).ToList();
            }
            else if (Sort == "pricehtl")
            {
                products = products.OrderByDescending(product => product.PriceDay).ToList();
            }
            else if (Sort == "rating")
            {
                products = products.OrderByDescending(product => product.Rating).ToList();
            }

            if (!String.IsNullOrEmpty(PriceRange))
            {
                if (PriceRange == "1000+")
                {
                    products = products.FindAll(product => product.PriceDay >= 1000).ToList();
                }
                else
                {
                    string[] minMax = PriceRange.Split('t');
                    products = products.FindAll(product => product.PriceDay >= int.Parse(minMax[0]) && product.PriceDay <= int.Parse(minMax[1])).ToList();
                }
            }
            if (!String.IsNullOrEmpty(MaxDay))
            {
                if (MaxDay == "1month")
                {
                    products = products.FindAll(product => product.MaxRentalDays >= 30).ToList();
                }
                else
                {
                    string[] minMax = MaxDay.Split('t');
                    products = products.FindAll(product => product.MaxRentalDays >= int.Parse(minMax[0]) && product.MaxRentalDays <= int.Parse(minMax[1])).ToList();
                }
            }
            if (Lat != 0 && Lng != 0)
            {
                products = products.OrderBy(product => SharedFunctions.CalculateHaversineDistance(Lat, Lng, SharedFunctions.GetDoubleValue(product.Latitude), SharedFunctions.GetDoubleValue(product.Longitude))).ToList();
            }
            ViewBag.CategoryName = Id > 0 ? Lookup.Categories[Id] : "All Category";
            ViewBag.Link = _config.Value.AgentWebsiteLink;
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                if (_unitOfWork.UsersDetails.Get(u => u.Id == user.Id) == null)
                { 
                    UsersDetailsModel usersDetailsModel = new UsersDetailsModel
                {
                    Username = user.UserName.Split('@')[0],
                    Id = user.Id,
                };
                _unitOfWork.UsersDetails.Add(usersDetailsModel);
                _unitOfWork.Save();
                
                }
            }
            return View(products);
        }
        public async Task<IActionResult> View(string? id)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            ProductModel model = _unitOfWork.Product.Get(u => u.ProductId == id, "Agent,ProductImages");
            LikeModel like = _unitOfWork.Like.Get(u => u.UserId == userId && u.ProductId == id);
            ViewBag.IsLike = like != null;
            if (userId != null)
            {
                IEnumerable<ReviewModel> Reviews = _unitOfWork.Review.GetAll(u => u.ProductId == model.ProductId,"UserDetails");
                ViewBag.Reviews = Reviews;
                ViewBag.HasAddRating = Reviews.FirstOrDefault(u => u.UserId == userId) != null;
                ViewBag.NoOfRating = Reviews.Count();
            }
            IEnumerable<ProductModel> OtherProducts = _unitOfWork.Product.GetAll(u => u.AgentId == model.Agent.Id && u.ProductId != model.ProductId);
            ViewBag.OtherProducts = OtherProducts;
            ViewBag.NoOfOtherProducts = OtherProducts.Count();
            ViewBag.Link = _config.Value.AgentWebsiteLink;
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddRating(int RatingValue, string RatingDescription, string ProductId, string AgentId)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            ReviewModel Review = new()
            {
                RatingValue = RatingValue,
                RatingDescription = RatingDescription,
                UserId = userId,
                ProductId = ProductId,
                AgentId = AgentId
            };

            _unitOfWork.Review.Add(Review);

            int OldRating = _unitOfWork.Product.Get(u => u.ProductId == ProductId).Rating;
            int NoOfRating = _unitOfWork.Review.GetAll(u => u.ProductId == ProductId).Count();
            int NewRating = ((OldRating * NoOfRating) + RatingValue) / (NoOfRating + 1);

            ProductModel Product = _unitOfWork.Product.Get(u => u.ProductId == ProductId);
            Product.Rating = NewRating;
            Product.UpdatedAt = DateTime.Now;
            _unitOfWork.Product.Update(Product);
            _unitOfWork.Save();


            return RedirectToAction("View", "Store");
        }
        [HttpPost]
        [Authorize]
        public ActionResult Like(string productId, string type, string agentId)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Json(new
                {
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                    Message = "Not logged in",
                    Data = "Unauthorized",
                    Success = false
                });
            }
            if (type == "unlike")
            {
                LikeModel Like = _unitOfWork.Like.Get(u => u.UserId == userId && u.ProductId == productId && u.AgentId == agentId);
                _unitOfWork.Like.Remove(Like);
                _unitOfWork.Save();

                return Json(new
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = "Done",
                    Data = "OK",
                    Success = true
                });
            }
            else if (type == "like")
            {
                LikeModel Like = new()
                {
                    UserId = userId,
                    ProductId = productId,
                    AgentId = agentId
                };
                _unitOfWork.Like.Add(Like);
                _unitOfWork.Save();

                return Json(new
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = "Done",
                    Data = "OK",
                    Success = true
                });
            }
            return Json(new
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = "Something Went Wrong",
                Data = "Something Went Wrong"
            });

        }
        [HttpPost]
        [Authorize]
        [AllowAnonymous]
        public async Task<ActionResult> SendMessage(string Receipient, string Message)
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
                UsersDetailsModel usersDetailsModel2 = _unitOfWork.UsersDetails.Get(u => u.Id == userId);
                if (usersDetailsModel != null)
                {
                    usersDetailsModel.Messages += 1;
                    usersDetailsModel.UpdatedAt = DateTime.Now;
                    _unitOfWork.UsersDetails.Update(usersDetailsModel);
                }
                _unitOfWork.Chat.Add(chat);
                _unitOfWork.Save();
                string redirectUrl = "https://rentwisegh.com/Page/Chat/"+userId;
                UsersDetailsModel ToUsersDetails = _unitOfWork.UsersDetails.Get(u => u.Id == Receipient);
                SharedFunctions.SendPushNotification(ToUsersDetails.OneSignalId, "You have a new message from "+ SharedFunctions.CapitalizeAllWords(usersDetailsModel2.Username), Message, redirectUrl);
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", ToUsersDetails.Id, Message);
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
        [Authorize]
        [AllowAnonymous]
        public async Task<ActionResult> Book(string ProductId, int ProductQuantity, DateTime StartDate, DateTime EndDate, int TotalPrice, string AgentId, string Message)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                TempData["ToastMessage"] = "Please login to place an order";
                return Json(new
                {
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                    Message = Lookup.ResponseMessages[4],
                    Data = "Unauthorized",
                    Success = false
                });
            }
            IdentityUser user = await _userManager.FindByIdAsync(userId);
            OrdersModel model = new()
            {
                ProductId = ProductId,
                UserId = userId,
                AgentId = AgentId,
                ProductQuantity = ProductQuantity,
                StartDate = StartDate,
                EndDate = EndDate,
                TotalAmount = TotalPrice
            };
            ChatModel chat = new()
            {
                FromUserId = userId,
                ToUserId = AgentId,
                Message = Message,
                IsOrder = true
            };
            UsersDetailsModel usersDetailsModel = _unitOfWork.UsersDetails.Get(u => u.Id == AgentId);
            if (usersDetailsModel != null)
            {
                usersDetailsModel.Orders += 1;
                usersDetailsModel.Messages += 1;
                usersDetailsModel.UpdatedAt = DateTime.Now;
                _unitOfWork.UsersDetails.Update(usersDetailsModel);
            }
            _unitOfWork.Order.Add(model);
            _unitOfWork.Chat.Add(chat);
            _unitOfWork.Save();
            ProductModel product = _unitOfWork.Product.Get(u=>u.ProductId == ProductId);
            AgentRegistrationModel agent = _unitOfWork.AgentRegistration.Get(u=>u.Id == product.AgentId,"User");
            string emailContentClient = SharedFunctions.EmailContent(usersDetailsModel.Username, 1, product.Name,model.ProductQuantity, TotalPrice);
            string emailContentAgent = SharedFunctions.EmailContent(agent.FirstName, 2, product.Name, model.ProductQuantity, model.TotalAmount);
            SharedFunctions.SendEmail(user.UserName, "Reservation has been made", emailContentClient);
            SharedFunctions.SendEmail(agent.User.UserName, "Reservation has been made", emailContentAgent);
            UsersDetailsModel ToUsersDetails = _unitOfWork.UsersDetails.Get(u => u.Id == AgentId);
            SharedFunctions.SendPushNotification(ToUsersDetails.OneSignalId, "Reservation has been made", Message);
            return Json(new
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Order Placed Successfully",
                Data = Lookup.ResponseMessages[5],
                Success = true
            });
        }

        [Authorize]
        public IActionResult Orders(string orderId = "")
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            IEnumerable<OrdersModel> orders = _unitOfWork.Order.GetAll(u => u.UserId == userId, "Product");
            UsersDetailsModel usersDetailsModel = _unitOfWork.UsersDetails.Get(u => u.Id == userId);
            if (usersDetailsModel != null)
            {
                usersDetailsModel.Orders = 0;
                usersDetailsModel.UpdatedAt = DateTime.Now;
                _unitOfWork.UsersDetails.Update(usersDetailsModel);
                _unitOfWork.Save();
            }
            return View(orders);
        }
        public IActionResult Success(string orderId = "",string checokutId = "")
        {
            OrdersModel order = _unitOfWork.Order.Get(u => u.OrderId == int.Parse(orderId), "Product");
            order.Paid = true;
            order.LkpStatus = 4;
            string UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            AgentRegistrationModel agent = _unitOfWork.AgentRegistration.Get(u => u.Id == order.AgentId,"User");
            string agentPhoneNumber = agent.PhoneNumber;
            if (order != null)
            {
                string Message = "Payment received by Owner, contact Owner on "+  agentPhoneNumber;
                ChatModel chat = new()
                {
                    FromUserId = order.AgentId,
                    ToUserId = order.UserId,
                    Message = Message,
                    IsOrder = true,
                };
                _unitOfWork.Chat.Add(chat);
            order.UpdatedAt = DateTime.Now;
            _unitOfWork.Order.Update(order);
            agent.PayWithCard += order.TotalAmount;
            agent.UpdatedAt = DateTime.Now;
                _unitOfWork.AgentRegistration.Update(agent);
            string emailContentAgent = SharedFunctions.EmailContent(agent.FirstName, 5, order.Product.Name, order.ProductQuantity, order.TotalAmount);
                UsersDetailsModel ToUsersDetails = _unitOfWork.UsersDetails.Get(u => u.Id == agent.Id);
                SharedFunctions.SendEmail(agent.User.UserName, "Payment has  been made for Reservation", emailContentAgent);
            SharedFunctions.SendPushNotification(ToUsersDetails.OneSignalId, "Reservation payment status", "Client has paid for a reservation and will soon contact you soon");
            }
            _unitOfWork.Save();
            TempData["Success"] = "Payment for order no"+ orderId + "was successful";
            return RedirectToAction(nameof(Orders));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Pay(int orderId = 0, string type = "cash", string pageLink = "")
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                TempData["ToastMessage"] = "Please login to place an order";
                return Json(new
                {
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                    Message = Lookup.ResponseMessages[4],
                    Data = "Unauthorized",
                    Success = false
                });
            }
            OrdersModel order = _unitOfWork.Order.Get(u => u.OrderId == orderId, "Product");
            ProductModel product = _unitOfWork.Product.Get(u => u.ProductId == order.ProductId, "Agent");
            AgentRegistrationModel agent = _unitOfWork.AgentRegistration.Get(u => u.Id == product.AgentId,"User");
            Random random = new Random();
            int randomNumber = random.Next(1, 101);
            string reference = order.OrderId.ToString() + "-RENTWISE-" + randomNumber;
            if (type == "cash")
            {
                order.LkpPaymentMethod = 1;
                order.LkpStatus = 7;
                order.UpdatedAt = DateTime.Now;
                _unitOfWork.Order.Update(order);
                _unitOfWork.Save();
                string emailContentAgent = SharedFunctions.EmailContent(agent.FirstName, 6, order.Product.Name, order.ProductQuantity, order.TotalAmount);
                SharedFunctions.SendEmail(agent.User.UserName, "Payment with cash", emailContentAgent);
                UsersDetailsModel ToUsersDetails = _unitOfWork.UsersDetails.Get(u => u.Id == agent.Id);
                SharedFunctions.SendPushNotification(ToUsersDetails.OneSignalId, "Reservation payment status", "Client want to pay cash, make sure you recieve money before marking it as paid");
                return Json(new
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = "Order Placed Successfully",
                    Data = "Ok",
                    Success = true
                });
            }
            else if (type == "online")
            {
                order.LkpPaymentMethod = 2;
                order.UpdatedAt = DateTime.Now;
                _unitOfWork.Order.Update(order);
                _unitOfWork.Save();

                // Populate JSON payload from the order variable
                // Create variables for amount, description, and reference
                double totalAmount = order.Product.PriceDay * order.ProductQuantity;
                string description = order.Product.Description; // Adjust this based on your actual structure
                string clientReference = reference;
                string link = _config.Value.ClientWebsiteLink+ "/Store/Success?orderId="+order.OrderId;
                // Create the JSON string using variables
                string jsonBody = $"{{\"totalAmount\":{totalAmount},\"description\":\"{description}\",\"callbackUrl\":\"{pageLink}?orderId={order.OrderId}\",\"returnUrl\":\"{link}\",\"cancellationUrl\":\"{pageLink}\",\"merchantAccountNumber\":\"2018934\",\"clientReference\":\"{clientReference}\"}}";


                var options = new RestClientOptions("https://payproxyapi.hubtel.com/items/initiate");
                var client = new RestClient(options);
                var request = new RestRequest("");
                request.AddHeader("accept", "application/json");
                request.AddHeader("content-type", "application/json");
                request.AddHeader("authorization", "Basic RDBvUXpPbjo3YTFkYWI0M2I1MTQ0Y2Q3OTFjMjMwNGU2N2UzNTJjOQ==");
                request.AddJsonBody(jsonBody, false);
                try
                {
                    var response = await client.PostAsync(request);
                    return Json(new
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = "Order Placed Successfully",
                    Data = JsonConvert.SerializeObject(response),
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
            return Json(new
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Message = Lookup.ResponseMessages[1],
                Data = "Internal Server Error",
                Success = false
            });
        }

        public IActionResult ModifyProduct(string Id, string type)
        {
            ProductModel product = _unitOfWork.Product.Get(u => u.ProductId == Id);
            AgentRegistrationModel agent = _unitOfWork.AgentRegistration.Get(u => u.Id == product.AgentId, "User");
            string emailContent = SharedFunctions.EmailContent(agent.User.UserName,type);
                UsersDetailsModel ToUsersDetails = _unitOfWork.UsersDetails.Get(u => u.Id == product.AgentId);
            if (type == "ENABLE")
            {
                product.Enabled = true;
                _unitOfWork.Product.Update(product);
                SharedFunctions.SendPushNotification(ToUsersDetails.OneSignalId,"Reservation has been enabled by admin", product.Name + "reservation has been enabled by admin");
                SharedFunctions.SendEmail(agent.User.UserName, "Your list has been disabled by admin", emailContent);
            }
            if (type == "DISABLE")
            {
                product.Enabled = false;
                _unitOfWork.Product.Update(product);
                SharedFunctions.SendPushNotification(ToUsersDetails.OneSignalId, "Reservation has been disabled by admin", product.Name + "reservation has been disabled by admin");
                SharedFunctions.SendEmail(agent.User.UserName, "Your list has been disabled by admin", emailContent);
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        [HttpPost]

        public async Task<IActionResult> setOnesignalId(string? id)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(id == null || userId == null)
            {
                return Json(new
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = Lookup.ResponseMessages[1],
                    Data = "Internal Server Error",
                    Success = false
                });
            }
           UsersDetailsModel userDetails = _unitOfWork.UsersDetails.Get(u=>u.Id == userId);
           userDetails.OneSignalId = id;
            _unitOfWork.UsersDetails.Update(userDetails);
            _unitOfWork.Save();

            return Json(new
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "ID saved Successfully",
                Data = Lookup.ResponseMessages[5],
                Success = true
            });
        }
    }

}
