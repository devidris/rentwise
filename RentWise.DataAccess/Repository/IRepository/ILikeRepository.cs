using RentWise.DataAccess.Repository.IRepository;
using RentWise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentWise.DataAccess.Repository.IRepository
{
    public interface ILikeRepository : IRepository<LikeModel>
    {
        void Update(LikeModel model);
    }
}
