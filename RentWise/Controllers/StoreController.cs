﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RentWise.DataAccess.Repository;
using RentWise.DataAccess.Repository.IRepository;
using RentWise.Models;
using RentWise.Models.Identity;
using RentWise.Utility;
using System.Net;
using System.Security.Claims;

namespace RentWise.Controllers
{
    public class StoreController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOptions<RentWiseConfig> _config;
        private readonly UserManager<IdentityUser> _userManager;
        public StoreController(IUnitOfWork unitOfWork, IOptions<RentWiseConfig> config, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _config = config;
            _userManager = userManager;
        }
        public IActionResult Index(int Id = 0, string? Sort = null, string? Name = null, string? PriceRange = null, string? MaxDay = null, double Lat = 0, double Lng = 0)
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
            return View(products);
        }
        public async Task<IActionResult> View(string? id)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            ProductModel model = _unitOfWork.Product.Get(u => u.ProductId == id, "Agent");
            LikeModel like = _unitOfWork.Like.Get(u => u.UserId == userId && u.ProductId == id);
            ViewBag.IsLike = like != null;
            if (userId != null)
            {
                IEnumerable<ReviewModel> Reviews = _unitOfWork.Review.GetAll(u => u.ProductId == model.ProductId);
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
            _unitOfWork.Product.Update(Product);
            _unitOfWork.Save();


            return RedirectToAction("View", "Store");
        }
        [HttpPost]
        [Authorize]
        public ActionResult Like(string productId, string type)
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
                LikeModel Like = _unitOfWork.Like.Get(u => u.UserId == userId && u.ProductId == productId);
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
                    ProductId = productId
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

                _unitOfWork.Chat.Add(chat);
                _unitOfWork.Save();
                return Json(new
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = "Message sent successfully",
                    Data = Lookup.ResponseMessages[5],
                    Success = true
                });
            } catch (Exception ex)
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