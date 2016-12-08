namespace MyServer.Services.Users
{
    using System;
    using System.Linq;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

    using MyServer.Data;
    using MyServer.Data.Common;
    using MyServer.Data.Models;
    using MyServer.Common;
    using System.Threading.Tasks;

    public class UserService : IUserService
    {     
        private readonly MyServerDbContext context;

        private readonly UserManager<User> userManager;

        private readonly RoleManager<User> roleManager;

        private readonly IRepository<User, string> users;

        public UserService(IRepository<User> users, MyServerDbContext context, UserManager<User> userManager, RoleManager<User> roleManager)
        {
            this.users = users;
            this.context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task<string> Add(string email, string firstName = "", string lastName = "", string password = UserConstants.InitialPassword, string role = null)
        {
            if (RegexUtilities.IsValidEmail(email))
            {
                var user = new User
                {
                    UserName = email,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    CreatedOn = DateTime.UtcNow
                };

                var userResult = await this.userManager.CreateAsync(user, password);

                if (userResult.Succeeded)
                {
                    role = role ?? MyServerRoles.User.ToString();

                    var roleResult = await this.userManager.AddToRoleAsync(user, role);
                    if (!roleResult.Succeeded)
                    {
                        return roleResult.Errors.First().ToString();
                    }

                    return string.Empty;
                }
                else
                {
                    return userResult.Errors.First().ToString();
                }         
            }
            else
            {
                return "Email is not valid.";
            }

            return "Fail.";
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