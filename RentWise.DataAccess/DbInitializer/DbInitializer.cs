using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RentWise.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentWise.DataAccess.DbInitializer
{
    public  class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public DbInitializer(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _db = db;
        }
        public void Initialize()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex) { }
            if (!_roleManager.RoleExistsAsync(Lookup.Roles[1]).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(Lookup.Roles[1])).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(Lookup.Roles[2])).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(Lookup.Roles[3])).GetAwaiter().GetResult();
            }
            return;
        }
    }
}
