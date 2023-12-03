using RentWise.DataAccess.Repository.IRepository;
using RentWise.DataAccess.Repository;
using RentWise.DataAccess;
using RentWise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentWise.DataAccess.Repository
{
    public class LikeRepository: Repository<LikeModel>, ILikeRepository
    {
        private ApplicationDbContext _db;
        public LikeRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }
        public void Update(LikeModel obj)
        {
            _db.Likes.Update(obj);
        }
    }
}
