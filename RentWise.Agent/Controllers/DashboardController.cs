using Microsoft.AspNetCore.Mvc;

namespace RentWise.Agent.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
