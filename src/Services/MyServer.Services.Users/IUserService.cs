namespace MyServer.Services.Users
{
    using System.Linq;

    using ImageGallery.Data.Models;

    public interface IUserService
    {
        void Add(User user);

        void Delete(string id);

        IQueryable<User> GetAll();

        User GetById(string id);

        void Update(User user);
    }
}