using RentWise.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentWise.DataAccess.Repository.IRepository
{
    public interface IWithdrawalHistoryRepository : IRepository<WithdrawalHistoryModel>
    {
        void Update(WithdrawalHistoryModel model);
    }
}
