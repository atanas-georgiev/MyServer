namespace MyServer.Services.Users
{
    using System;
    using System.Linq;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

    using MyServer.Data;
    using MyServer.Data.Common;
    using MyServer.Data.Models;

    public class UserService : IUserService
    {
        private const string InitialPassword = "changeme";

        private readonly MyServerDbContext context;

        private readonly UserManager<User> userManager;

        private readonly IRepository<User, string> users;

        public UserService(IRepository<User> users, MyServerDbContext context, UserManager<User> userManager)
        {
            this.users = users;
            this.context = context;
            this.userManager = userManager;
        }

        public void Add(User user, string role)
        {
            if (user != null && !string.IsNullOrEmpty(role))
            {
                user.CreatedOn = DateTime.Now;
                user.UserName = user.Email;
                var result = this.userManager.CreateAsync(user, InitialPassword).Result;

                if (result.Succeeded)
                {
                    this.Update(user, role);
                }
            }
        }

        public void Delete(string id)
        {
            var userRoles = this.context.UserRoles.Where(x => x.UserId == id);
            this.context.UserRoles.RemoveRange(userRoles);
            this.context.SaveChanges();

            this.users.Delete(id);
        }

        public IQueryable<User> GetAll()
        {
            return this.users.All();
        }

        public User GetById(string id)
        {
            return this.users.GetById(id);
        }

        public void Update(User user, string role = null)
        {
            if (!string.IsNullOrEmpty(role))
            {
                var userRoleToUpdate = this.context.Roles.First(x => x.Name == role);
                var userRoles = this.context.UserRoles.Where(x => x.UserId == user.Id);
                this.context.UserRoles.RemoveRange(userRoles);
                this.context.SaveChanges();
                this.context.UserRoles.Add(
                    new IdentityUserRole<string>() { RoleId = userRoleToUpdate.Id, UserId = user.Id });
                this.context.SaveChanges();
            }

            this.users.Update(user);
        }
    }
}