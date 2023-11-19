using RentWise.DataAccess.Repository.IRepository;
using RentWise.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentWise.DataAccess.Repository
{
    public class AgentRegistrationRepository : Repository<AgentRegistrationModel>, IAgentRegistrationRepository
    {
        private ApplicationDbContext _db;
        public AgentRegistrationRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(AgentRegistrationModel obj)
        {
            _db.AgentRegistrations.Update(obj);
        }
    }
}
