using RentWise.DataAccess.Repository.IRepository;
using RentWise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentWise.DataAccess.Repository
{
    public class StateRepository:Repository<State>, IStateRepository
    {
        private readonly ApplicationDbContext _db;
        public StateRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }
        public void Update(State state)
        {
            _db.States.Update(state);
        }
    }
}
