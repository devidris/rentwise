using RentWise.DataAccess.Repository.IRepository;
using RentWise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentWise.DataAccess.Repository
{
    public class ProductRepository : Repository<ProductModel>, IProductRepository
    {
        private ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db):base(db) 
        {

            _db = db;

        }
        public void Update(ProductModel obj)
        {
            _db.Products.Update(obj);
        }
    }
}
