using RentWise.DataAccess.Repository.IRepository;
using RentWise.Models;


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
