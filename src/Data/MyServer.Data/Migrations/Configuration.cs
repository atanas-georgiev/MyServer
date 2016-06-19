namespace MyServer.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.IO;
    using System.Linq;

    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    using MyServer.Common;
    using MyServer.Data.Models;

    using Constants = MyServer.Common.ImageGallery.Constants;

    public sealed class Configuration : DbMigrationsConfiguration<MyServerDbContext>
    {
        public Configuration()
        {
            this.AutomaticMigrationsEnabled = true;
#if DEBUG
            this.AutomaticMigrationDataLossAllowed = true;
#else
            this.AutomaticMigrationDataLossAllowed = true; //false;
#endif
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

            var album = new Album(Constants.NoCoverId)
            {
                CreatedOn = DateTime.Now,
                AddedBy = user,
                Title = "No Image Album",
                IsPublic = false,
            };

            context.Albums.AddOrUpdate(album);
            context.SaveChanges();

            var image = new Image(Constants.NoCoverId)
            {
                CreatedOn = DateTime.Now,
                AddedBy = user,
                Album = album,
                //Cover = album,
                Height = 400,
                Width = 600,
                LowHeight = 400,
                LowWidth = 600,
                MidHeight = 400,
                MidWidth = 600,
                FileName = Constants.NoCoverImage,
                OriginalFileName = Constants.NoCoverImage
            };

            context.Images.AddOrUpdate(image);
            context.SaveChanges();

            album.CoverId = image.Id;
            context.Albums.AddOrUpdate(album);
            context.SaveChanges();
        }

        private void SeedRoles(MyServerDbContext context)
        {
            context.Roles.AddOrUpdate(x => x.Name, new IdentityRole(MyServerRoles.Admin));
            context.Roles.AddOrUpdate(x => x.Name, new IdentityRole(MyServerRoles.User));
            context.Roles.AddOrUpdate(x => x.Name, new IdentityRole(MyServerRoles.Public));
            context.SaveChanges();
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}