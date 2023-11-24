using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RentWise.DataAccess.Repository.IRepository;
using RentWise.Models;
using RentWise.Models.Identity;
using RentWise.Utility;

namespace RentWise.Agent.Controllers
{
    public class StoreController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public StoreController(UserManager<IdentityUser> userManager,IWebHostEnvironment webHostEnvironment,IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _unitOfWork = unitOfWork;

        }

        public async Task<IActionResult> Index(int id = 0)
        {
            IdentityUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Register", "Login");
            }
            AgentRegistrationModel agentDetails = _unitOfWork.AgentRegistration.Get(u=>u.Id == user.Id);
            ViewBag.RegistrationDate = agentDetails.CreatedAt;
            ViewBag.Id = user.Id;
            ViewBag.Id = id;
            if(id == 0)
            {
                IEnumerable<ProductModel> userProducts = _unitOfWork.Product.GetAll(u => u.AgentId == user.Id);
                return View(userProducts);
            }else
            {
            IEnumerable<ProductModel> userProducts = _unitOfWork.Product.GetAll(u => u.AgentId == user.Id && u.LkpCategory == id);
                return View(userProducts);
            }
            
        }

        public IActionResult Upsert()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Upsert(ProductModel model, IFormFile? mainImage, IFormFile? cancellationPolicy, List<IFormFile> otherImages)
        {
            IdentityUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Register", "Login");
            }
            model.AgentId = user.Id;
            if (mainImage == null)
            {
                ModelState.AddModelError(string.Join("", Lookup.Upload[9].Split(" ")), "Main Image upload is compulsory.");
            }
            if (otherImages == null || otherImages.Count < 7)
            {
                ModelState.AddModelError(string.Join("", Lookup.Upload[10].Split(" ")), "Other Images upload is compulsory and must be more than or equal to 7 images.");
            }
            if (cancellationPolicy == null)
            {
                ModelState.AddModelError(string.Join("", Lookup.Upload[11].Split(" ")), "Cancellation Policy upload is compulsory.");
            }
            if (ModelState.IsValid) {
                model.Description = SharedFunctions.Capitalize(model.Description);
                model.Name = SharedFunctions.Capitalize(model.Name);

                #region Saving Main Product
                string mainImageName = String.Join("", Lookup.Upload[9].Split(" ")) + ".webp";

                saveImage(model.AgentId, mainImageName, mainImage, model.ProductId);
                #endregion

                #region Saving Cancellation Policy
                string cancellationPolicyName = String.Join("", Lookup.Upload[11].Split(" ")) + ".pdf";

                saveImage(model.AgentId, cancellationPolicyName, cancellationPolicy,model.ProductId);
                #endregion

                #region Saving Other Images
                for(int i = 0; i < otherImages.Count; i++) { 
                IFormFile otherImage = otherImages[i];
                string otherImageName = String.Join("", Lookup.Upload[10].Split(" ")) + "-" + i + ".webp";
                saveImage(model.AgentId, otherImageName, otherImage,model.ProductId);
                }
                #endregion

                model.NoOfImages = otherImages.Count;
                if (String.IsNullOrEmpty(model.Includes))
                {
                    model.Includes = ",,rw,,";
                }
                if (String.IsNullOrEmpty(model.Rules))
                {
                    model.Rules = ",,rw,,";
                }
                _unitOfWork.Product.Add(model);
                _unitOfWork.Save();

                return RedirectToAction("Preview", "Store",new { id = model.ProductId });
            }
            return View();
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

        public async Task<IActionResult> Preview(string ?id)
        {
            IdentityUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Register", "Login");
            }
            ProductModel model = new ProductModel();
            if(String.IsNullOrEmpty(id))
            {
                model = _unitOfWork.Product.Get(u => u.AgentId == user.Id, "Agent");
            } else
            {
                model = _unitOfWork.Product.Get(u => u.ProductId == id, "Agent");

            }
            IEnumerable<ReviewModel> Reviews = _unitOfWork.Review.GetAll(u=>u.ProductId == model.ProductId);
            ViewBag.Reviews = Reviews;
            ViewBag.HasAddRating = Reviews.FirstOrDefault(u => u.UserId == user.Id) != null;
            ViewBag.NoOfRating = Reviews.Count();
            IEnumerable<ProductModel> OtherProducts = _unitOfWork.Product.GetAll(u=>u.AgentId == user.Id && u.ProductId != model.ProductId);
            ViewBag.OtherProducts = OtherProducts;
            ViewBag.NoOfOtherProducts = OtherProducts.Count();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddRating(int RatingValue, string RatingDescription, string ProductId, string AgentId)
        {
            IdentityUser user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Register", "Login");
            }

            ReviewModel Review = new()
            {
                RatingValue = RatingValue,
                RatingDescription = RatingDescription,
                UserId = user.Id,
                ProductId = ProductId,
                AgentId = AgentId
            };

            _unitOfWork.Review.Add(Review);

            int OldRating = _unitOfWork.Product.Get(u=>u.ProductId == ProductId).Rating;
            int NoOfRating = _unitOfWork.Review.GetAll(u => u.ProductId == ProductId).Count();
            int NewRating = ((OldRating * NoOfRating) + RatingValue) / (NoOfRating + 1);

            ProductModel Product = _unitOfWork.Product.Get(u => u.ProductId == ProductId);
            Product.Rating = NewRating;
            _unitOfWork.Product.Update(Product);
            _unitOfWork.Save();


            return RedirectToAction("Preview", "Store");
        }
    }
}
