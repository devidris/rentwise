using RentWise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentWise.DataAccess.Repository.IRepository
{
    public interface IProductImageRepository : IRepository<ProductImageModel>
    {
        void Update(ProductImageModel model);
    }
}
