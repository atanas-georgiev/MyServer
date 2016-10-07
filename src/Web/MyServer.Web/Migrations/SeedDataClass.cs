using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyServer.Common;
using MyServer.Data.Models;
using MyServer.Web.Helpers;
using System;
using System.Linq;

namespace MyServer.Data.Migrations
{
    public static class SeedDataClass
    {
        public static void SeedData(this IServiceScopeFactory scopeFactory)
        {
            using (var serviceScope = scopeFactory.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<MyServerDbContext>();
                try
                {
                    if (!context.Users.Any())
                    {
                        SeedRoles(context);
                        SeedAdmin(context);
                    }
                }
                catch
                {
                    context.Database.Migrate();

                    if (!context.Users.Any())
                    {
                        SeedRoles(context);
                        SeedAdmin(context);
                    }
                }
            }

        }

        private static void SeedAdmin(MyServerDbContext context)
        {
            var user = new User
            {
                UserName = "atanasgeorgiev83@gmail.com",
                Email = "atanasgeorgiev83@gmail.com",
                FirstName = "Atanas",
                LastName = "Georgiev",
                CreatedOn = DateTime.Now
            };

            var res1 = PathHelper.UserManager.CreateAsync(user, "Godcheto!1").Result;
            context.SaveChanges();

            var role = context.Roles.First(x => x.Name == MyServerRoles.Admin);

            context.UserRoles.Add(new IdentityUserRole<string>() { RoleId = role.Id, UserId = user.Id });
            context.SaveChanges();

            var album = new Album(MyServer.Common.ImageGallery.Constants.NoCoverId)
            {
                CreatedOn = DateTime.Now,
                AddedById = user.Id,
                Title = "No Image Album"
            };

            context.Albums.Add(album);
            context.SaveChanges();

            var image = new Image(MyServer.Common.ImageGallery.Constants.NoCoverId)
            {
                CreatedOn = DateTime.Now,
                AddedById = user.Id,
                AlbumId = album.Id,

                // Cover = album,
                Height = 400,
                Width = 600,
                LowHeight = 400,
                LowWidth = 600,
                MidHeight = 400,
                MidWidth = 600,
                FileName = MyServer.Common.ImageGallery.Constants.NoCoverImage,
                OriginalFileName = MyServer.Common.ImageGallery.Constants.NoCoverImage
            };

            context.Images.Add(image);
            context.SaveChanges();

            album.CoverId = image.Id;
            context.Albums.Update(album);
            context.SaveChanges();
        }

        private static void SeedRoles(MyServerDbContext context)
        {
            context.Roles.AddRange(new IdentityRole(MyServerRoles.Admin), new IdentityRole(MyServerRoles.User), new IdentityRole(MyServerRoles.Public));
            context.SaveChanges();
        }
    }
}
