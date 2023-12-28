using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RentWise.DataAccess.Repository.IRepository;
using RentWise.Models;
using RentWise.Models.Identity;
using RentWise.Utility;
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

        public async Task<IActionResult> Index(int Category = 2,int Min = 0,int Max = 0,double Lng = 0, double Lat = 0)
        {
            ViewBag.Category = Category;
            ViewBag.Min = Min;
            ViewBag.Max = Max;
            ViewBag.Lng = Lng;
            ViewBag.Lat = Lat;
            ViewBag.Link = _config.Value.AgentWebsiteLink;
            List<ProductModel> products = _unitOfWork.Product.GetAll(u=>u.LkpCategory == Category,"Agent").ToList();
            if(Min > 0)
            {
                products = products.FindAll(product => product.PriceDay >= Min).ToList();
            }   
            if(Max > 0)
            {
                products = products.FindAll(product => product.PriceDay <= Max).ToList();
            }
            if (Lat != 0 && Lng != 0)
            {
                products = products.OrderBy(product => SharedFunctions.CalculateHaversineDistance(Lat, Lng, SharedFunctions.GetDoubleValue(product.Latitude), SharedFunctions.GetDoubleValue(product.Longitude))).ToList();
            }
            ViewBag.NoOfProducts = products.Count();
            var user = await _userManager.GetUserAsync(User);
            if (user != null) {
                if(_unitOfWork.UsersDetails.Get(u=>u.Id == user.Id) == null) { 
                
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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}