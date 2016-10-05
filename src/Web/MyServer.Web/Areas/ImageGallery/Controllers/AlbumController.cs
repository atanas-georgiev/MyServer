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
using System.Linq;

namespace MyServer.Web.Areas.ImageGallery.Controllers
{
    [Area("ImageGallery")]
    [Route("ImageGallery/Album")]
    public class AlbumController : BaseController
    {
        private readonly IAlbumService albumService;

        public AlbumController(IUserService userService, UserManager<User> userManager, SignInManager<User> signInManager, MyServerDbContext dbContext, IAlbumService albumService) : base(userService, userManager, signInManager, dbContext)
        {
            this.albumService = albumService;
        }

        [Route("Details/{id}")]
        public IActionResult Details(string id)
        {
            var album = this.albumService.GetAllReqursive().Where(x => x.Id.ToString() == id).To<AlbumViewModel>().FirstOrDefault();
            return this.View(album);
        }

        [Route("Download/{id}")]
        public IActionResult Download(string id)
        {
            var zip = this.albumService.GenerateZipArchive(Guid.Parse(id));
            return this.Content(zip.Replace("~", string.Empty));
        }

        [Route("Index")]
        public IActionResult Index()
        {
            var albums = this.albumService.GetAllReqursive().OrderByDescending(x => x.CreatedOn).To<AlbumViewModel>().ToList();
            return this.View(albums);
        }
    }
}