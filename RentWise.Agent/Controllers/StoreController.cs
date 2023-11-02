using Microsoft.AspNetCore.Mvc;

namespace RentWise.Agent.Controllers
{
    public class StoreController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
