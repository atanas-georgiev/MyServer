using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyServer.Data;
using MyServer.Data.Models;
using MyServer.Services.ImageGallery;
using MyServer.Services.Mappings;
using MyServer.Services.Users;
using MyServer.Web.Areas.ImageGallery.Models.Album;
using MyServer.Web.Areas.Shared.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyServer.Web.Areas.ImageGallery.Controllers
{
    [Area("ImageGallery")]
    public class AlbumController : BaseController
    {
        private readonly IAlbumService albumService;

        public AlbumController(IUserService userService, UserManager<User> userManager, SignInManager<User> signInManager, MyServerDbContext dbContext, IAlbumService albumService) : base(userService, userManager, signInManager, dbContext)
        {
            this.albumService = albumService;
        }

        public IActionResult Details(string id)
        {
            var album = this.albumService.GetAllReqursive().Where(x => x.Id.ToString() == id).To<AlbumViewModel>().FirstOrDefault();

            if (!this.User.Identity.IsAuthenticated && album.Access != Common.MyServerAccessType.Public)
            {
                return this.Unauthorized();
            }
            else if (User.IsInRole(Common.MyServerRoles.User) && (album.Access == Common.MyServerAccessType.Private))
            {
                return this.Unauthorized();
            }

            return this.View(album);
        }

        public IActionResult Download(string id)
        {
            var zip = this.albumService.GenerateZipArchive(Guid.Parse(id));
            return this.Content(zip.Replace("~", string.Empty));
        }

        public IActionResult Index()
        {
            List<AlbumViewModel> albums = new List<AlbumViewModel>();
            
            if (!this.User.Identity.IsAuthenticated)
            {
                albums = this.albumService.GetAllReqursive().Where(x => x.Access == Common.MyServerAccessType.Public).OrderByDescending(x => x.CreatedOn).To<AlbumViewModel>().ToList();
            }
            else if (User.IsInRole(Common.MyServerRoles.User))
            {
                albums = this.albumService.GetAllReqursive().Where(x => x.Access == Common.MyServerAccessType.Public || x.Access == Common.MyServerAccessType.Registrated).OrderByDescending(x => x.CreatedOn).To<AlbumViewModel>().ToList();
            }
            else if (User.IsInRole(Common.MyServerRoles.Admin))
            {
                albums = this.albumService.GetAllReqursive().OrderByDescending(x => x.CreatedOn).To<AlbumViewModel>().ToList();
            }

            return this.View(albums);
        }
    }
}