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
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            AgentRegistration = new AgentRegistrationRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
