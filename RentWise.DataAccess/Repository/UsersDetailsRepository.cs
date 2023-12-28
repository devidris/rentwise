using RentWise.DataAccess.Repository.IRepository;
using RentWise.Models;
using RentWise.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentWise.DataAccess.Repository
{
    public class UsersDetailsRepository: Repository<UsersDetailsModel>,IUsersDetailsRepository
    {
        private ApplicationDbContext _db;
        public UsersDetailsRepository(ApplicationDbContext db) : base(db)
        {

            _db = db;

        }
        public void Update(UsersDetailsModel obj)
        {
            _db.UsersDetails.Update(obj);
        }
    }
}
