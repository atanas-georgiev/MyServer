using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyServer.Data;
using MyServer.Data.Models;
using MyServer.Services.ImageGallery;
using MyServer.Services.Mappings;
using MyServer.Services.Users;
using MyServer.Web.Areas.ImageGalleryAdmin.Models.Album;
using MyServer.Web.Areas.Shared.Controllers;
using MyServer.Web.Main.Areas.ImageGalleryAdmin.Models.Album;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace MyServer.Web.Areas.ImageGalleryAdmin.Controllers
{
    [Area("ImageGalleryAdmin")]
    [Route("ImageGalleryAdmin/Album")]
    public class AlbumController : BaseController
    {
        private readonly IAlbumService albumService;

        private readonly ILocationService locationService;

        private readonly IImageService imageService;

        public AlbumController(IAlbumService albumService, ILocationService locationService, IImageService imageService, IUserService userService, UserManager<User> userManager, SignInManager<User> signInManager, MyServerDbContext dbContext) : base(userService, userManager, signInManager, dbContext)
        {
            this.albumService = albumService;
            this.locationService = locationService;
            this.imageService = imageService;
        }

        [Route("Index")]
        public IActionResult Index()
        {
            //this.albumService.Add(new Album()
            //{
            //    Title = "asdasdsa",
            //    Description = "desc"
            //});

            var albums = this.albumService.GetAllReqursive().To<AlbumListViewModel>().ToList();            
            return this.View(albums);
        }

        [Route("Create")]
        public IActionResult Create()
        {
            return this.View(new AddAlbumViewModel() { Title = "" });
        }

        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AddAlbumViewModel model)
        {
            if (this.ModelState.IsValid && model != null)
            {
                var album = new Album()
                {
                    Title = model.Title,
                    Description = model.Description,
                    CreatedOn = DateTime.UtcNow,
                    AddedBy = this.UserProfile,
                    //IsPublic = model.IsPublic,
                    Cover = this.imageService.GetAll().First()
                };

                this.albumService.Add(album);
                return this.RedirectToAction("Index");
            }

            return this.View(model);
        }

        [Route("Details")]
        public IActionResult Details(string id)
        {
            //this.Response.SetCookie(new HttpCookie("AlbumId", id));
            var intId = Guid.Parse(id);
            var result =
                this.albumService.GetAll().Where(x => x.Id == intId).To<AlbumDetailsViewModel>().FirstOrDefault();

            if (result == null)
            {
               // return this.HttpNotFound("Album not found");
            }

            return this.View(result);
        }
    }
}