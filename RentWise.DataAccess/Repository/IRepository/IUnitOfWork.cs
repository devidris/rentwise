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
        void Save();
    }
}
