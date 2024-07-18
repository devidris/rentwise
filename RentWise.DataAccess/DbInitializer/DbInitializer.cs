using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RentWise.DataAccess.Repository.IRepository;
using RentWise.Models;
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
        public readonly IUnitOfWork _unitOfWork;

        public DbInitializer(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db, IUnitOfWork unitOfWork)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _db = db;
            _unitOfWork = unitOfWork;
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

            EnsureAllSettingsExist();
            return;
        }

        public void EnsureAllSettingsExist()
        {
            IEnumerable<SettingModel> existingSettings = _unitOfWork.Setting.GetAll();

            foreach (var setting in Lookup.Settings)
            {
                if (!existingSettings.Any(s => s.LookupId == setting.Key))
                {
                    SettingModel settingModel = new SettingModel
                    {
                        LookupId = setting.Key,
                        Name = setting.Value.Name,
                        Value = setting.Value.Value
                    };

                    _unitOfWork.Setting.Add(settingModel);
                }
            }

            _unitOfWork.Save();
        }
    }
}
