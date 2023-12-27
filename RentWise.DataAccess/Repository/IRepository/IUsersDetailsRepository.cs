using RentWise.Models;
using RentWise.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentWise.DataAccess.Repository.IRepository
{
    public interface IUsersDetailsRepository
    {
        void Update(UsersDetailsModel model);
    }
}
