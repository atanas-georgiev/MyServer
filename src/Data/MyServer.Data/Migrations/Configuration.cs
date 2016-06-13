namespace MyServer.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;

    using ImageGallery.Data.Models;

    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    using MyServer.Common;

    public sealed class Configuration : DbMigrationsConfiguration<MyServerDbContext>
    {
        public Configuration()
        {
            this.AutomaticMigrationsEnabled = true;
            this.AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(MyServerDbContext context)
        {
            if (context.Roles.Any())
            {
                return;
            }

            this.SeedRoles(context);
            this.SeedAdmin(context);
        }

        private void SeedAdmin(MyServerDbContext context)
        {
            var userManager = new UserManager<User>(new UserStore<User>(context));

            var user = new User
                           {
                               UserName = "atanasgeorgiev83@gmail.com", 
                               Email = "atanasgeorgiev83@gmail.com", 
                               FirstName = "Atanas", 
                               LastName = "Georgiev", 
                               CreatedOn = DateTime.Now
                           };

            userManager.Create(user, "Godcheto1");
            context.SaveChanges();

            userManager.AddToRole(user.Id, MyServerRoles.Admin);
            context.SaveChanges();
        }

        private void SeedRoles(MyServerDbContext context)
        {
            context.Roles.AddOrUpdate(x => x.Name, new IdentityRole(MyServerRoles.Admin));
            context.Roles.AddOrUpdate(x => x.Name, new IdentityRole(MyServerRoles.User));
            context.SaveChanges();
        }
    }
}