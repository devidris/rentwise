using RentWise.DataAccess.Repository.IRepository;
using RentWise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentWise.DataAccess.Repository
{
    public class ChatRepository:Repository<ChatModel>,IChatRepository
    {
        public ApplicationDbContext _db;
        public ChatRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }
        public void Update(ChatModel obj)
        {
            _db.Chats.Update(obj);
        }
    }
}
