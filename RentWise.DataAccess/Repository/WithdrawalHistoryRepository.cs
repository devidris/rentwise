using RentWise.DataAccess.Repository.IRepository;
using RentWise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentWise.DataAccess.Repository
{
    public class WithdrawalHistoryRepository : Repository<WithdrawalHistoryModel>, IWithdrawalHistoryRepository
    {
        private ApplicationDbContext _db;
        public WithdrawalHistoryRepository(ApplicationDbContext db) : base(db)
        {

            _db = db;

        }
        public void Update(WithdrawalHistoryModel obj)
        {
            _db.WithdrawalHistories.Update(obj);
        }
    }
}
