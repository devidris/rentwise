using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentWise.DataAccess.Repository.IRepository;
using RentWise.Models;
using System.Security.Claims;

namespace RentWise.Agent.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public DashboardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;       
        }
        public IActionResult Index()
        {
            string UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;    
            ViewBag.Orders = _unitOfWork.Order.GetAll(u=>u.AgentId == UserId,"Product");
            return View();
        }
        public IActionResult ApproveOrReject(int Id,int LkpStatus)
        {
            OrdersModel order = _unitOfWork.Order.Get(u => u.OrderId == Id);
            if (order != null)
            {
                string Message = "Your order has been rejected";
                if (Id == 2)
                {
                  Message = "Your order has been accepted";
                }  
                ChatModel chat = new()
                {
                    FromUserId = order.AgentId,
                    ToUserId = order.UserId,
                    Message = Message,
                    IsOrder = true,
                };

                _unitOfWork.Chat.Add(chat);
                order.LkpStatus = LkpStatus;
                _unitOfWork.Order.Update(order);
                _unitOfWork.Save();
            }
            return RedirectToAction("Index","Dashboard");
        }
    }
}
