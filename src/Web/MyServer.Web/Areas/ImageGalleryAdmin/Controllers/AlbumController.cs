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

        private readonly IFileService fileService;

        public AlbumController(IAlbumService albumService, ILocationService locationService, IImageService imageService, IUserService userService, UserManager<User> userManager, SignInManager<User> signInManager, MyServerDbContext dbContext, IFileService fileService) : base(userService, userManager, signInManager, dbContext)
        {
            this.albumService = albumService;
            this.locationService = locationService;
            this.imageService = imageService;
            this.fileService = fileService;
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

        [HttpPost]
        [Route("AlbumDataPartial")]
        [ValidateAntiForgeryToken]
        public PartialViewResult AlbumDataPartial(AlbumEditViewModel model)
        {
            if (this.ModelState.IsValid && model != null)
            {
                var album = this.albumService.GetById(model.Id);

                //if (album == null)
                //{
                //    return this.NotFound();
                //}

                album.Title = model.Title;
                album.Description = model.Description;
                //album.IsPublic = model.IsPublic;

                this.albumService.Update(album);

                var result = this.albumService.GetAll().Where(a => a.Id == album.Id).To<AlbumEditViewModel>().First();

                return this.PartialView("_AlbumDataPartial", result);
            }

            return this.PartialView("_AlbumDataPartial", model);
        }

        [Route("Edit")]
        public IActionResult Edit(string id)
        {
            this.Response.Cookies.Append("AlbumId", id);
            var intId = Guid.Parse(id);
            var result =
                this.albumService.GetAllReqursive().Where(x => x.Id == intId).To<AlbumEditViewModel>().FirstOrDefault();

            if (result == null)
            {
               return this.NotFound("Album not found");
            }

            return this.View(result);
        }        
    }
}