using Microsoft.AspNetCore.Mvc;

namespace RentWise.Controllers
{
    public class Page : Controller
    {
        public IActionResult About()
        {
            return View();
        }
    }
}
