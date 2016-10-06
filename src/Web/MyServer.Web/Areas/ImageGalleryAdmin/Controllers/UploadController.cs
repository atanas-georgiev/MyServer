namespace MyServer.Web.Main.Areas.ImageGalleryAdmin.Controllers
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Identity;
    using Data;
    using Data.Models;
    using MyServer.Services.ImageGallery;
    using MyServer.Services.Users;
    using Web.Areas.Shared.Controllers;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;

    [Area("ImageGalleryAdmin")]
    public class UploadController : BaseController
    {
        private readonly IImageService imageService;

        public UploadController(IUserService userService, UserManager<User> userManager, SignInManager<User> signInManager, MyServerDbContext dbContext, IImageService imageService) : base(userService, userManager, signInManager, dbContext)
        {
            this.imageService = imageService;
        }

        public ActionResult Save(ICollection<IFormFile> files)
        {
            var albumId = Guid.Parse(this.Request.Cookies["AlbumId"]);

            // The Name of the Upload component is "files"
            if (files != null)
            {
                foreach (var file in files)
                {
                    this.imageService.Add(albumId, this.UserProfile.Id, file.OpenReadStream(), file.FileName);
                }
            }

            // Return an empty string to signify success
            return this.Content(string.Empty);
        }
    }
}