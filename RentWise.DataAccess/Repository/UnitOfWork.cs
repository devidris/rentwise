using RentWise.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentWise.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;
        public IAgentRegistrationRepository AgentRegistration { get; private set; }
        public IProductRepository Product { get; private set; }

        public IReviewRepository Review { get; private set; }

        public ILikeRepository Like { get; private set; }

        public IChatRepository Chat { get; private set; }

        public IOrdersRepository Order { get; private set; }


        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            AgentRegistration = new AgentRegistrationRepository(_db);
            Product = new ProductRepository(_db);
            Review = new ReviewRepository(_db);
            Like = new LikeRepository(_db);
            Chat = new ChatRepository(_db);
            Order = new OrdersRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
