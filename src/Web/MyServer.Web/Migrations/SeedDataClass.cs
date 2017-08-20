using Microsoft.AspNetCore.Identity;

namespace MyServer.Web.Migrations
{
    using System;
    using System.Linq;

    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    using MyServer.Common;
    using MyServer.Data;
    using MyServer.Data.Models;
    using MyServer.Web.Helpers;

    public static class SeedDataClass
    {
        public static void SeedData(this IServiceScopeFactory scopeFactory)
        {
            using (var serviceScope = scopeFactory.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<MyServerDbContext>();
                try
                {
                    if (!Queryable.Any<User>(context.Users))
                    {
                        SeedRoles(context);
                        SeedAdmin(context);
                    }
                }
                catch
                {
                    context.Database.Migrate();

                    if (!Queryable.Any<User>(context.Users))
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

            var res1 = PathHelper.UserManager.CreateAsync(user, "chnageme").Result;
            context.SaveChanges();

            var role = Queryable.First<IdentityRole>(context.Roles, x => x.Name == MyServerRoles.Admin.ToString());

            context.UserRoles.Add(new IdentityUserRole<string>() { RoleId = role.Id, UserId = user.Id });
            context.SaveChanges();

            var album = new Album(Common.ImageGallery.Constants.NoCoverId)
                            {
                                CreatedOn = DateTime.Now,
                                AddedById = user.Id,
                                TitleBg = "No Image Album",
                                TitleEn = "No Image Album",
                            };

            context.Albums.Add(album);
            context.SaveChanges();

            var image = new Image(Common.ImageGallery.Constants.NoCoverId)
                            {
                                CreatedOn = DateTime.Now,
                                AddedById = user.Id,
                                AlbumId = album.Id,

                                // Cover = album,
                                Height = 267,
                                Width = 400,
                                LowHeight = 267,
                                LowWidth = 400,
                                MidHeight = 267,
                                MidWidth = 400,
                                FileName = Common.ImageGallery.Constants.NoCoverImage,
                                OriginalFileName = Common.ImageGallery.Constants.NoCoverImage
                            };

            context.Images.Add(image);
            context.SaveChanges();

            album.CoverId = image.Id;
            context.Albums.Update(album);
            context.SaveChanges();
        }

        private static void SeedRoles(MyServerDbContext context)
        {
            context.Roles.AddRange(
                new IdentityRole(MyServerRoles.Admin.ToString()),
                new IdentityRole(MyServerRoles.User.ToString()));
            context.SaveChanges();
        }
    }
}