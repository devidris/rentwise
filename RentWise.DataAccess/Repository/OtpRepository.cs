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
    public class OtpRepository:Repository<OtpVerification>,IOtpRepository
    {
        private ApplicationDbContext _db;
        public OtpRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(OtpVerification obj)
        {
            _db.OtpVerifications.Update(obj);
        }
    }
}
