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

        public IActionResult Index()
        {
            IEnumerable<ProductModel> Construction = _unitOfWork.Product.GetAll(u => u.LkpCategory == 1, "Agent").Take(9);
            IEnumerable<ProductModel> CarRental = _unitOfWork.Product.GetAll(u => u.LkpCategory == 2,"Agent").Take(9);
            IEnumerable<ProductModel> OfficeItem = _unitOfWork.Product.GetAll(u => u.LkpCategory == 3, "Agent").Take(9);
            IEnumerable<ProductModel> Event = _unitOfWork.Product.GetAll(u => u.LkpCategory == 4, "Agent").Take(9);
            IEnumerable<ProductModel> CarTracker = _unitOfWork.Product.GetAll(u => u.LkpCategory == 5, "Agent").Take(9);
            ViewBag.Construction = Construction;
            ViewBag.ConstructionCount = Construction.Count();
            ViewBag.CarRental = CarRental;
            ViewBag.CarRentalCount = CarRental.Count();
            ViewBag.OfficeItem = OfficeItem;
            ViewBag.OfficeItemCount = OfficeItem.Count();
            ViewBag.Events = Event;
            ViewBag.EventsCount = Event.Count();
            ViewBag.CarTracker = CarTracker;
            ViewBag.CarTrackerCount = CarTracker.Count();
            ViewBag.Link = _config.Value.AgentWebsiteLink;
            return View();
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