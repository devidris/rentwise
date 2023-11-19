using RentWise.DataAccess.Repository.IRepository;
using RentWise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentWise.DataAccess.Repository
{

    public class ReviewRepository : Repository<ReviewModel>, IReviewRepository
    {
        private ApplicationDbContext _db;
        public ReviewRepository(ApplicationDbContext db) : base(db)
        {

            _db = db;

        }
        public void Update(ReviewModel obj)
        {
            _db.Reviews.Update(obj);
        }
    }
}
