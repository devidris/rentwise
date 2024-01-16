using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RentWise.DataAccess.Repository.IRepository;
using RentWise.Models;
using RentWise.Models.Identity;
using RentWise.Utility;
using System.Net;
using System.Security.Claims;

namespace RentWise.Agent.Controllers
{
    [Authorize(Roles = "Admin, Agent")]
    public class StoreController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public StoreController(UserManager<IdentityUser> userManager, IWebHostEnvironment webHostEnvironment, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _unitOfWork = unitOfWork;

        }

        public async Task<IActionResult> Index(int id = 0,string name = "")
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            ViewBag.Id = userId;
            AgentRegistrationModel agentDetails = _unitOfWork.AgentRegistration.Get(u => u.Id == userId);
            ViewBag.RegistrationDate = agentDetails.CreatedAt;
            List<LikeModel> like = _unitOfWork.Like.GetAll(u => u.AgentId == agentDetails.Id).ToList();
            ViewBag.LikeCount = like.Count;
            IEnumerable<ProductModel> userProducts = new List<ProductModel>();
            userProducts = _unitOfWork.Product.GetAll(u => u.AgentId == userId);
            ViewBag.TotalSubmited = userProducts.Count();
            if (id > 0)
            {
                userProducts = userProducts.Where(u => u.LkpCategory == id);
            }
            if (!String.IsNullOrEmpty(name))
            {
                userProducts = userProducts.Where(u=>u.Name.Contains(name));
                ViewBag.Name = name;
            }
            ViewBag.Categories = Lookup.Categories;
            ViewBag.TotalFilterCount = userProducts.Count();
            return View(userProducts);

        }

        public IActionResult Upsert(string? id)
        {
            ProductModel product = new ProductModel(){};
            if(id != null) {
                product = _unitOfWork.Product.Get(u => u.ProductId == id,"Agent,ProductImages");
            }
            return View(product);
        }
        [HttpPost]
        public async Task<IActionResult> Upsert(ProductModel model, IFormFile? mainImage, List<IFormFile>? otherImages)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            model.AgentId = userId;
            if(model.ProductId == null)
            {
                if (mainImage == null)
                {
                    ModelState.AddModelError(string.Join("", Lookup.Upload[9].Split(" ")), "Main Image upload is compulsory.");
                }
                if (otherImages == null || otherImages.Count < 4)
                {
                    ModelState.AddModelError(string.Join("", Lookup.Upload[10].Split(" ")), "Other Images upload is compulsory and must be more than or equal to 4 images.");
                }
            }
            if (ModelState.IsValid)
            {
                model.Description = SharedFunctions.Capitalize(model.Description);
                model.Name = SharedFunctions.Capitalize(model.Name);

                #region Saving Main Product
               if(mainImage != null)
                {
                    string mainImageName = String.Join("", Lookup.Upload[9].Split(" ")) + ".webp";

                    saveImage(model.AgentId, mainImageName, mainImage, model.ProductId);
                }
                #endregion

                #region Saving Other Images
               if(otherImages != null && otherImages.Any())
                {
                    for (int i = 0; i < otherImages.Count; i++)
                    {
                        IFormFile otherImage = otherImages[i];
                        string otherImageName = Guid.NewGuid().ToString() + ".webp";
                        saveImages(model.AgentId, otherImageName, otherImage, model.ProductId);
                    }
                }
                #endregion
                if (String.IsNullOrEmpty(model.Includes))
                {
                    model.Includes = ",,rw,,";
                }
                if (String.IsNullOrEmpty(model.Rules))
                {
                    model.Rules = ",,rw,,";
                }
                if(model.ProductId != null) {
                    _unitOfWork.Product.Update(model);
                } else
                {
                    model.ProductId = Guid.NewGuid().ToString();
                    _unitOfWork.Product.Add(model);
                }
                _unitOfWork.Save();

                return RedirectToAction("Preview", "Store", new { id = model.ProductId });
            }
            return View(model);
        }

        public void saveImage(string userId, string fileName, IFormFile file, string productId)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string filePath = @"images\" + "\\products\\" + userId + "\\" + productId + "\\";
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
        public void saveImages(string userId, string fileName, IFormFile file, string productId)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string filePath = @"images\" + "\\products\\" + userId + "\\" + productId + "\\";
            string finalPath = Path.Combine(wwwRootPath, filePath);

            if (!Directory.Exists(finalPath))
            {
                Directory.CreateDirectory(finalPath);
            }

            using (FileStream fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
            {
                file.CopyTo(fileStream);
            }
            ProductImageModel model = new ProductImageModel();
            model.ProductId = productId;
            model.Name = fileName;
            _unitOfWork.ProductImage.Add(model);
        }

        public async Task<IActionResult> Preview(string? id)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            ProductModel model = new ProductModel();
            if (String.IsNullOrEmpty(id))
            {
                model = _unitOfWork.Product.Get(u => u.AgentId == userId, "Agent,ProductImages");
            }
            else
            {
                model = _unitOfWork.Product.Get(u => u.ProductId == id, "Agent,ProductImages");

            }
            IEnumerable<ReviewModel> Reviews = _unitOfWork.Review.GetAll(u => u.ProductId == model.ProductId);
            ViewBag.Reviews = Reviews;
            ViewBag.HasAddRating = Reviews.FirstOrDefault(u => u.UserId == userId) != null;
            ViewBag.NoOfRating = Reviews.Count();
            IEnumerable<ProductModel> OtherProducts = _unitOfWork.Product.GetAll(u => u.AgentId == userId && u.ProductId != model.ProductId);
            ViewBag.OtherProducts = OtherProducts;
            ViewBag.NoOfOtherProducts = OtherProducts.Count();
            return View(model);
        }

        [HttpPost]
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


            return RedirectToAction("Preview", "Store");
        }

        [HttpDelete]
        public ActionResult DeleteImage(int Id)
        {
            ProductImageModel productImage = _unitOfWork.ProductImage.Get(u => u.Id == Id);
            _unitOfWork.ProductImage.Remove(productImage);
            _unitOfWork.Save();
            return Json(new
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Image deleted successfully",
                Success = true
            });
        }

        public IActionResult ModifyProduct(string Id,string type)
        {
            ProductModel product = _unitOfWork.Product.Get(u => u.ProductId == Id);
            if(type == "DELETE") { 
            _unitOfWork.Product.Remove(product);
            } 
            if(type == "ENABLE")
            {
                product.Enabled = true;
             _unitOfWork.Product.Update(product);
            }
            if(type == "DISABLE")
            {
                product.Enabled = false;
                _unitOfWork.Product.Update(product);
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

    }
}
