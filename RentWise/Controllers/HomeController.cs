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


        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, IOptions<RentWiseConfig> config)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _config = config;

        }

        public IActionResult Index(int Category = 2,int Min = 0,int Max = 0,double Lng = 0, double Lat = 0)
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