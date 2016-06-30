namespace MyServer.Services.Users
{
    using System.Linq;

    using Microsoft.EntityFrameworkCore;

    using MyServer.Data.Common;
    using MyServer.Data.Models;

    public class UserService : IUserService
    {
        private readonly IRepository<User, string> users;

        public UserService(DbContext context, IRepository<User> users)
        {
            this.users = users;
        }

        public void Add(User user)
        {
            // user.UserName = user.Email;
            // user.CreatedOn = DateTime.Now;            
            // this.users.Update(user);            
        }

        public void Delete(string id)
        {
            // this.users.Delete(id);
            // var messagesDb = this.messages.All().Where(x => x.FromId == id || x.ToId == id).ToList();
            // foreach (var message in messagesDb)
            // {
            // this.messages.Delete(message);
            // }
        }

        public IQueryable<User> GetAll()
        {
            return this.users.All();
        }

        public User GetById(string id)
        {
            return this.users.GetById(id);
        }

        public void Update(User user)
        {
            this.users.Update(user);
        }
    }
}