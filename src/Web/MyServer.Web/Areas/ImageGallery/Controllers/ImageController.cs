using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyServer.Data;
using MyServer.Data.Models;
using MyServer.Services.ImageGallery;
using MyServer.Services.Users;
using MyServer.Web.Areas.ImageGallery.Models.Image;
using MyServer.Web.Areas.Shared.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyServer.Web.Areas.ImageGallery.Controllers
{
    [Area("ImageGallery")]
    public class ImageController : BaseController
    {
        private readonly IAlbumService albumService;

        private readonly IImageService imageService;

        public ImageController(IUserService userService, UserManager<User> userManager, SignInManager<User> signInManager, MyServerDbContext dbContext, IAlbumService albumService, IImageService imageService) : base(userService, userManager, signInManager, dbContext)
        {
            this.imageService = imageService;
            this.albumService = albumService;
        }

        public IActionResult Index()
        {
            var dates =
                this.imageService.GetAllReqursive()
                    .OrderByDescending(x => x.DateTaken)
                    .Where(x => x.DateTaken != null)
                    .Select(x => x.DateTaken.Value.Date)
                    .Distinct()
                    .ToList();

            this.TempData["cnt"] = 1;
            this.TempData["dates"] = dates;

            return this.View(new ImageListViewModel() { Type = Common.ImageGallery.ImageListType.Date, Caption = dates.First().ToString(), Data = dates.First() });
        }

        public IActionResult FinishRender(string id)
        {
            var data = this.TempData["dates"] as DateTime[];
            var cnt = this.TempData["cnt"] as Int32?;

            if (cnt.Value < data.Length)
            {
                this.TempData["cnt"] = cnt.Value + 1;
                this.TempData["dates"] = data;
                return this.ViewComponent("ImageList", new { type = Common.ImageGallery.ImageListType.Date, caption = data[cnt.Value].ToString(), data = data[cnt.Value] });
            }

            return this.NoContent();
        }
    }
}
