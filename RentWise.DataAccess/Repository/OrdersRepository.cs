using RentWise.DataAccess.Repository.IRepository;
using RentWise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentWise.DataAccess.Repository
{
    public class OrdersRepository:Repository<OrdersModel>,IOrdersRepository
    {
        public ApplicationDbContext _db;
        public OrdersRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }
        public void Update(OrdersModel obj)
        {
            _db.Orders.Update(obj);
        }
    }
}
