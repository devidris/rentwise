using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RentWise.DataAccess.Repository.IRepository;
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
            AgentRegistrationModel agentDetails = _unitOfWork.AgentRegistration.Get(u=>u.UserId == user.Id);
            ViewBag.RegistrationDate = agentDetails.RegistrationDate;
            ViewBag.UserId = user.Id;
            ViewBag.Id = id;
            if(id == 0)
            {
                IEnumerable<ProductModel> userProducts = _unitOfWork.Product.GetAll(u => u.UserId == user.Id);
                return View(userProducts);
            }else
            {
            IEnumerable<ProductModel> userProducts = _unitOfWork.Product.GetAll(u => u.UserId == user.Id && u.LkpCategory == id);
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
            model.UserId = user.Id;
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
                #region Saving Main Product
                string mainImageName = String.Join("", Lookup.Upload[9].Split(" ")) + ".webp";

                saveImage(model.UserId, mainImageName, mainImage, model.ProductId);
                #endregion

                #region Saving Cancellation Policy
                string cancellationPolicyName = String.Join("", Lookup.Upload[11].Split(" ")) + ".pdf";

                saveImage(model.UserId, cancellationPolicyName, cancellationPolicy,model.ProductId);
                #endregion

                #region Saving Other Images
                for(int i = 0; i < otherImages.Count; i++) { 
                IFormFile otherImage = otherImages[i];
                string otherImageName = String.Join("", Lookup.Upload[10].Split(" ")) + "-" + i + ".webp";
                saveImage(model.UserId, otherImageName, otherImage,model.ProductId);
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

        public void saveImage(string userId, string fileName, IFormFile file,string productId)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string filePath = @"images\"  + "\\products\\" + userId +"\\"+ productId + "\\";
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
            AgentRegistrationModel agentRegistrationModel = _unitOfWork.AgentRegistration.Get(u => u.UserId == user.Id);
            ViewBag.StoreName = agentRegistrationModel.StoreName;
            ViewBag.StoreAddress = agentRegistrationModel.StoreAddress;
            ViewBag.RegistrationDate = agentRegistrationModel.RegistrationDate;
            if(String.IsNullOrEmpty(id))
            {
                model = _unitOfWork.Product.Get(u => u.UserId == user.Id);
            } else
            {
                model = _unitOfWork.Product.Get(u => u.ProductId == id);
            }
            return View(model);
        }
    }
}
