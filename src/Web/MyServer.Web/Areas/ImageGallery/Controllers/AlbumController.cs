namespace MyServer.Web.Areas.ImageGallery.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using MyServer.Common.ImageGallery;
    using MyServer.Data;
    using MyServer.Data.Models;
    using MyServer.Services.ImageGallery;
    using MyServer.Services.Mappings;
    using MyServer.Services.Users;
    using MyServer.Web.Areas.ImageGallery.Models.Album;
    using MyServer.Web.Areas.Shared.Controllers;

    [Area("ImageGallery")]
    public class AlbumController : BaseController
    {
        private readonly IAlbumService albumService;

        public AlbumController(
            IUserService userService,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            MyServerDbContext dbContext,
            IAlbumService albumService)
            : base(userService, userManager, signInManager, dbContext)
        {
            this.albumService = albumService;
        }

        public IActionResult Details(string id)
        {
            var album =
                this.albumService.GetAllReqursive()
                    .Where(x => x.Id.ToString() == id)
                    .To<AlbumViewModel>()
                    .FirstOrDefault();

            if (!this.User.Identity.IsAuthenticated && album.Access != Common.MyServerAccessType.Public)
            {
                return this.Unauthorized();
            }
            else if (this.User.IsInRole(Common.MyServerRoles.User.ToString())
                     && (album.Access == Common.MyServerAccessType.Private))
            {
                return this.Unauthorized();
            }

            return this.View(album);
        }

        public IActionResult Download(string id, ImageType type)
        {
            var zip = this.albumService.GenerateZipArchive(Guid.Parse(id), type);
            return this.Content(zip.Replace("~", string.Empty));
        }

        public IActionResult Index()
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult SortFilter(SortFilterAlbumViewModel model)
        {
            return this.ViewComponent("AllAlbums", new { ViewDetailsUrl = "Album/Details/", Filter = model.SearchString });
        }
    }
}