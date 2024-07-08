using RentWise.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentWise.DataAccess.Repository
{
    public class Database<T> : Repository<T>, IDatabase<T> where T : class
    {
        private ApplicationDbContext _db;
        public Database(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(T model)
        {
            // Check if the model exists in the context and is being tracked
            var existingEntity = _db.Set<T>().Find(model);

            if (existingEntity != null)
            {
                // Update the existing entity
                _db.Entry(existingEntity).CurrentValues.SetValues(model);
            }
            else
            {
                // If the entity is not tracked, attach it and set its state to modified
                _db.Set<T>().Attach(model);
                _db.Entry(model).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            }
        }
    }
}
