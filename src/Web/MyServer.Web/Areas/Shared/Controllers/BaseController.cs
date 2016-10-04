﻿namespace MyServer.Web.Areas.Shared.Controllers
{
    using System;
    using System.Linq;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    using MyServer.Common;
    using MyServer.Data;
    using MyServer.Data.Models;
    using MyServer.Services.Users;
    using System.Threading.Tasks;

    public class BaseController : Controller
    {
        protected readonly MyServerDbContext dbContext;        

        protected readonly SignInManager<User> signInManager;

        protected readonly UserManager<User> userManager;

        public BaseController(
            IUserService userService, 
            UserManager<User> userManager, 
            SignInManager<User> signInManager,             
            MyServerDbContext dbContext)
        {
            this.UserService = userService;
            this.userManager = userManager;
            this.signInManager = signInManager;            
            this.dbContext = dbContext;
        }

        protected User UserProfile { get; private set; }

        protected IUserService UserService { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var usersCount = this.UserService.GetAll().Count();

            if (usersCount == 0)
            {
                this.SeedRoles(this.dbContext);
                this.SeedAdmin(this.dbContext);
            }

            this.UserProfile = this.UserService.GetAll().FirstOrDefault(u => u.UserName == context.HttpContext.User.Identity.Name);
            base.OnActionExecuting(context);
        }

        protected void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
            //    this.ModelState.AddModelError(string.Empty, error);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.userManager != null)
                {
                    this.userManager.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        protected ActionResult RedirectToLocal(string returnUrl)
        {
            if (this.Url.IsLocalUrl(returnUrl))
            {
                return this.Redirect(returnUrl);
            }

            return this.RedirectToAction("Index", "Home", new { area = string.Empty });
        }

        private void SeedAdmin(MyServerDbContext context)
        {
            var user = new User
                           {
                               UserName = "atanasgeorgiev83@gmail.com", 
                               Email = "atanasgeorgiev83@gmail.com", 
                               FirstName = "Atanas", 
                               LastName = "Georgiev", 
                               CreatedOn = DateTime.Now
                           };

            var res1 = this.userManager.CreateAsync(user, "Godcheto!1").Result;            
            this.dbContext.SaveChanges();

            var role = context.Roles.First(x => x.Name == MyServerRoles.Admin);

            this.dbContext.UserRoles.Add(new IdentityUserRole<string>() { RoleId = role.Id, UserId = user.Id });
            this.dbContext.SaveChanges();

            // var res2 = this.userManager.AddToRoleAsync(user, MyServerRoles.Admin).Result;

            var album = new Album(Common.ImageGallery.Constants.NoCoverId)
                            {
                                CreatedOn = DateTime.Now, 
                                AddedBy = user, 
                                Title = "No Image Album"
                            };

            context.Albums.Add(album);
            context.SaveChanges();

            var image = new Image(MyServer.Common.ImageGallery.Constants.NoCoverId)
                            {
                                CreatedOn = DateTime.Now, 
                                AddedBy = user, 
                                Album = album, 

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

        private void SeedRoles(MyServerDbContext context)
        {
            context.Roles.AddRange(new IdentityRole(MyServerRoles.Admin), new IdentityRole(MyServerRoles.User), new IdentityRole(MyServerRoles.Public));
            context.SaveChanges();
        }
    }
}