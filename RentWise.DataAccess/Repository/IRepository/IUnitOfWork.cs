using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentWise.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IAgentRegistrationRepository AgentRegistration { get; }
        IProductRepository Product { get; }
        IReviewRepository Review { get; }
        ILikeRepository Like { get; }

        IChatRepository Chat { get; }
        IOrdersRepository Order { get; }
        IUsersDetailsRepository UsersDetails { get; }
        IContactAdminRepository ContactAdmin { get; }
        void Save();
    }
}
