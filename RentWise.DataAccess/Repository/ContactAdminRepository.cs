using RentWise.DataAccess.Repository.IRepository;
using RentWise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentWise.DataAccess.Repository
{
    public class ContactAdminRepository: Repository<ContactAdminModel>,IContactAdminRepository
    {
        private ApplicationDbContext _db;
        public ContactAdminRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(ContactAdminModel model)
        {
            _db.ContactAdmin.Update(model);
        }
    }
}
