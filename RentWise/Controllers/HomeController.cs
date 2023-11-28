using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RentWise.DataAccess.Repository.IRepository;
using RentWise.Models;
using RentWise.Models.Identity;
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

        public IActionResult Index(int category = 2)
        {
            ViewBag.Category = category;
            ViewBag.Link = _config.Value.AgentWebsiteLink;
            List<ProductModel> products = _unitOfWork.Product.GetAll(u=>u.LkpCategory == category).ToList();
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