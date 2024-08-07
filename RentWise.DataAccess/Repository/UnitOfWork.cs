﻿using RentWise.DataAccess.Repository.IRepository;
using RentWise.Models;
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
        public IProductRepository Product { get; private set; }

        public IReviewRepository Review { get; private set; }

        public ILikeRepository Like { get; private set; }

        public IChatRepository Chat { get; private set; }

        public IOrdersRepository Order { get; private set; }

        public IUsersDetailsRepository UsersDetails { get; private set; } 
        public IContactAdminRepository ContactAdmin { get; private set; }

        public IProductImageRepository ProductImage { get; private set; }

        public IWithdrawalHistoryRepository WithdrawalHistory { get; private set; }

        public IOtpRepository Otp { get; private set; }

        public IStateRepository State{ get; private set; }
        public ICityRepository City { get; private set; }

        public IDatabase<SettingModel> Setting { get; private set; }
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            AgentRegistration = new AgentRegistrationRepository(_db);
            Product = new ProductRepository(_db);
            Review = new ReviewRepository(_db);
            Like = new LikeRepository(_db);
            Chat = new ChatRepository(_db);
            Order = new OrdersRepository(_db);
            UsersDetails = new UsersDetailsRepository(_db);
            ContactAdmin = new ContactAdminRepository(_db);
            ProductImage = new ProductImageRepository(_db);
            WithdrawalHistory = new WithdrawalHistoryRepository(_db);
            Otp = new OtpRepository(_db);
            State = new StateRepository(_db);
            City = new CityRepository(_db);
            Setting = new Database<SettingModel>(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
