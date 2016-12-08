namespace MyServer.Services.Users
{
    using System.Linq;

    using MyServer.Data.Models;
    using MyServer.Common;
    using System.Threading.Tasks;

    public interface IUserService
    {        
        Task<string> Add(string email, string firstName = "", string lastName = "", string password = UserConstants.InitialPassword, string role = null);

        void Delete(string id);

        IQueryable<User> GetAll();

        User GetById(string id);

        void Update(User user, string role = null);
    }
}