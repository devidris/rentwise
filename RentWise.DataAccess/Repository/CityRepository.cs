using RentWise.DataAccess.Repository.IRepository;
using RentWise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentWise.DataAccess.Repository
{
    public class CityRepository: Repository<City>, ICityRepository
    {
        private  ApplicationDbContext _db;
        public CityRepository(ApplicationDbContext db): base(db)
        {
            _db = db;
        }

        public void Update(City model)
        {
            _db.Cities.Update(model);
        }
    }   

}
