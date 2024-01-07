using RentWise.DataAccess.Repository.IRepository;
using RentWise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentWise.DataAccess.Repository
{
    public class ProductImageRepository : Repository<ProductImageModel>, IProductImageRepository
    {
        private ApplicationDbContext _db;
        public ProductImageRepository(ApplicationDbContext db) : base(db)
        {

            _db = db;

        }
        public void Update(ProductImageModel obj)
        {
            _db.ProductImages.Update(obj);
        }
    }
}
