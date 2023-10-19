using Microsoft.AspNetCore.Mvc;
using RentWise.Models;
using RentWise.Utility;

namespace RentWise.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index(int Id)
        {
            List<DisplayPreview> CategoryData = new()
    {
    new DisplayPreview
    {
        Image="/img/construction.png",
        Name="Shell Forest",
        Price=1000,
        Rating=4.5,
        Location="Accra, Ghana",
        AgentImage="/img/profile.png",
        AgentName="Lois"
    }, new DisplayPreview
    {
        Image="/img/construction.png",
        Name="Shell Forest",
        Price=1000,
        Rating=4.5,
        Location="Accra, Ghana",
        AgentImage="/img/profile.png",
        AgentName="Lois"
    }, new DisplayPreview
    {
        Image="/img/construction.png",
        Name="Shell Forest",
        Price=1000,
        Rating=4.5,
        Location="Accra, Ghana",
        AgentImage="/img/profile.png",
        AgentName="Lois"
    },
     new DisplayPreview
    {
        Image="/img/construction.png",
        Name="Shell Forest",
        Price=1000,
        Rating=4.5,
        Location="Accra, Ghana",
        AgentImage="/img/profile.png",
        AgentName="Lois"
    }, new DisplayPreview
    {
        Image="/img/construction.png",
        Name="Shell Forest",
        Price=1000,
        Rating=4.5,
        Location="Accra, Ghana",
        AgentImage="/img/profile.png",
        AgentName="Lois"
    }, new DisplayPreview
    {
        Image="/img/construction.png",
        Name="Shell Forest",
        Price=1000,
        Rating=4.5,
        Location="Accra, Ghana",
        AgentImage="/img/profile.png",
        AgentName="Lois"
    }, new DisplayPreview
    {
        Image="/img/construction.png",
        Name="Shell Forest",
        Price=1000,
        Rating=4.5,
        Location="Accra, Ghana",
        AgentImage="/img/profile.png",
        AgentName="Lois"
    }, new DisplayPreview
    {
        Image="/img/construction.png",
        Name="Shell Forest",
        Price=1000,
        Rating=4.5,
        Location="Accra, Ghana",
        AgentImage="/img/profile.png",
        AgentName="Lois"
    }, new DisplayPreview
    {
        Image="/img/construction.png",
        Name="Shell Forest",
        Price=1000,
        Rating=4.5,
        Location="Accra, Ghana",
        AgentImage="/img/profile.png",
        AgentName="Lois"
    }, new DisplayPreview
    {
        Image="/img/construction.png",
        Name="Shell Forest",
        Price=1000,
        Rating=4.5,
        Location="Accra, Ghana",
        AgentImage="/img/profile.png",
        AgentName="Lois"
    }, new DisplayPreview
    {
        Image="/img/construction.png",
        Name="Shell Forest",
        Price=1000,
        Rating=4.5,
        Location="Accra, Ghana",
        AgentImage="/img/profile.png",
        AgentName="Lois"
    }, new DisplayPreview
    {
        Image="/img/construction.png",
        Name="Shell Forest",
        Price=1000,
        Rating=4.5,
        Location="Accra, Ghana",
        AgentImage="/img/profile.png",
        AgentName="Lois"
    }, new DisplayPreview
    {
        Image="/img/construction.png",
        Name="Shell Forest",
        Price=1000,
        Rating=4.5,
        Location="Accra, Ghana",
        AgentImage="/img/profile.png",
        AgentName="Lois"
    },
    }
;
            ViewBag.CategoryName = Lookup.Value[Id];
            return View(CategoryData);
        }
    }
}
