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
            // Assuming 'Id' is the name of the primary key and it's of type int
            var keyProperty = typeof(T).GetProperty("Id");
            if (keyProperty != null)
            {
                var key = keyProperty.GetValue(model);
                var existingEntity = _db.Set<T>().Find(key);

                if (existingEntity != null)
                {
                    // Update the existing entity
                    _db.Entry(existingEntity).CurrentValues.SetValues(model);
                    _db.SaveChanges(); // Make sure to save changes
                }
                else
                {
                    // If the entity is not tracked, attach it and set its state to modified
                    _db.Set<T>().Attach(model);
                    _db.Entry(model).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _db.SaveChanges(); // Make sure to save changes
                }
            }
            else
            {
                throw new InvalidOperationException("Could not find primary key property 'Id' on the model");
            }
        }

    }
}
