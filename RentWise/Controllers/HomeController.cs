using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RentWise.DataAccess.Migrations;
using RentWise.DataAccess.Repository.IRepository;
using RentWise.Models;
using RentWise.Models.Identity;
using RentWise.Utility;
using System;
using System.Configuration;
using System.Diagnostics;

namespace RentWise.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOptions<RentWiseConfig> _config;
        private readonly UserManager<IdentityUser> _userManager;


        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, IOptions<RentWiseConfig> config, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _config = config;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            try {
                ViewBag.Link = _config.Value.AgentWebsiteLink;
                var random = new Random();
                var categories = new List<(int id, string viewBagKey)>
        {
            (1, "Constructions"),
            (2, "CarRentals"),
            (3, "OfficeItems"),
            (4, "Events"),
            (5, "AllProduct"),
            (8, "Motels")
        };

                foreach (var (id, viewBagKey) in categories)
                {
                    List<ProductModel> catProducts = new List<ProductModel>();
                    if(id != 5)
                    {
                        catProducts = _unitOfWork.Product.GetAll(u => u.Enabled == true && u.LkpCategory == id, "Agent,ProductImages").OrderBy(x => random.Next()).Take(8).ToList();
                    } else
                    {
                        catProducts = _unitOfWork.Product.GetAll(u => u.Enabled == true, "Agent,ProductImages").OrderBy(x => random.Next()).Take(10).ToList();
                    }
                    var displayPreviews = catProducts.Select(product => new DisplayPreview
                    {
                        Image = product.ProductImages.FirstOrDefault()?.Name != null ? _config.Value.AgentWebsiteLink + "/images/products/" + product.AgentId + "/" + product.ProductId + "/" + product.ProductImages.FirstOrDefault()?.Name : Url.Content("~/img/default-product.jpg"),
                        Name = product.Name,
                        Price = product.PriceDay,
                        Rating = product.Rating,
                        Location = product.Agent?.City ?? "Unknown"
                    }).ToList();
                    ViewData[viewBagKey] = displayPreviews;
                }

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
                string logInfo = $"Home page loaded";
                _logger.LogInformation(logInfo);
                return View();
            }
            catch (Exception ex) {
                _logger.LogError(ex.ToString());
                return View(nameof(Error));
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}