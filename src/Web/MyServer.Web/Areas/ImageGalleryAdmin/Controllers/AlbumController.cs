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
using MyServer.Web.Main.Areas.ImageGalleryAdmin.Models.Image;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyServer.Common;

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
            var albums = this.albumService.GetAllReqursive().To<AlbumListViewModel>().ToList();            
            return this.View(albums);
        }

        [Route("Create")]
        public IActionResult Create()
        {
            ViewBag.AccessTypes = new SelectList(new List<string>() { MyServerAccessType.Public.ToString(), MyServerAccessType.Registrated.ToString(), MyServerAccessType.Private.ToString() });
            return this.View(new AddAlbumViewModel() { Title = "", Access = Common.MyServerAccessType.Registrated });
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
                    Access = model.Access,
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
                album.Access = model.Access;

                this.albumService.Update(album);

                var result = this.albumService.GetAll().Where(a => a.Id == album.Id).To<AlbumEditViewModel>().First();

                return this.PartialView("_AlbumDataPartial", result);
            }

            return this.PartialView("_AlbumDataPartial", model);
        }

        [Route("Edit")]
        public IActionResult Edit(string id)
        {
            ViewBag.AccessTypes = new SelectList(new List<string>() { MyServerAccessType.Public.ToString(), MyServerAccessType.Registrated.ToString(), MyServerAccessType.Private.ToString() });
            this.Response.Cookies.Append("AlbumId", id);
            var intId = Guid.Parse(id);
            var result = this.albumService.GetAllReqursive().Where(x => x.Id == intId).To<AlbumEditViewModel>().FirstOrDefault();

            if (result == null)
            {
               return this.NotFound("Album not found");
            }

            return this.View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("UpdateImageLocation")]
        public IActionResult UpdateImageLocation(ImageUpdateViewModel model)
        {
            if (model != null && !string.IsNullOrEmpty(model.Items))
            {
                var ids = model.Items.Split(',');
                var gpsData = this.locationService.GetGpsData(model.Data).Result;

                foreach (var id in ids)
                {
                    this.imageService.AddGpsDataToImage(Guid.Parse(id), gpsData);
                }

                var imageId = Guid.Parse(ids.First());
                var albumId = this.imageService.GetById(imageId).AlbumId;
                var album = this.albumService.GetAll().Where(x => x.Id == albumId).To<AlbumEditViewModel>().First();

                return this.PartialView("_ImageListPartial", album);
            }

            return this.Content(string.Empty);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("UpdateImageTitle")]
        public IActionResult UpdateImageTitle(ImageUpdateViewModel model)
        {
            if (model != null && !string.IsNullOrEmpty(model.Items))
            {
                var ids = model.Items.Split(',');

                foreach (var id in ids)
                {
                    var image = this.imageService.GetById(Guid.Parse(id));
                    image.Title = model.Data;
                    this.imageService.Update(image);
                }

                var albumId = Guid.Parse(this.Request.Cookies["AlbumId"]);
                var album = this.albumService.GetAllReqursive().Where(x => x.Id == albumId).To<AlbumEditViewModel>().First();
                return this.PartialView("_ImageListPartial", album);
            }

            return this.Content(string.Empty);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("UpdateImageDate")]
        public IActionResult UpdateImageDate(ImageUpdateViewModel model)
        {
            if (model != null && !string.IsNullOrEmpty(model.Items))
            {
                var ids = model.Items.Split(',');

                foreach (var id in ids)
                {
                    var image = this.imageService.GetById(Guid.Parse(id));
                    var date = DateTime.Parse(model.Data);
                    image.DateTaken = date;
                    this.imageService.Update(image);
                }

                var albumId = Guid.Parse(this.Request.Cookies["AlbumId"]);
                var album = this.albumService.GetAllReqursive().Where(x => x.Id == albumId).To<AlbumEditViewModel>().First();
                return this.PartialView("_ImageListPartial", album);
            }

            return this.Content(string.Empty);
        }

        [HttpPost]
        [Route("DeleteImages/{id}")]
        public IActionResult DeleteImages(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var ids = id.Split(',');
                var imageId = Guid.Parse(ids.First());
                var albumId = this.imageService.GetById(imageId).AlbumId;

                foreach (var item in ids)
                {
                    this.imageService.Remove(Guid.Parse(item));
                }

                var album = this.albumService.GetAllReqursive().Where(x => x.Id == albumId).To<AlbumEditViewModel>().First();
                return this.PartialView("_ImageListPartial", album);
            }

            return this.Content(string.Empty);
        }

        [HttpPost]
        [Route("UpdateAlbumCover/{id}")]
        public IActionResult UpdateAlbumCover(string id)
        {
            var albumId = Guid.Parse(this.Request.Cookies["AlbumId"]);
            var coverId = Guid.Parse(id);

            this.albumService.UpdateCoverImage(albumId, coverId);
            return this.Content(string.Empty);
        }

        [HttpPost]
        [Route("UpdateImages")]
        public PartialViewResult UpdateImages()
        {
            var albumId = Guid.Parse(this.Request.Cookies["AlbumId"]);
            var album = this.albumService.GetAllReqursive().Where(x => x.Id == albumId).To<AlbumEditViewModel>().First();
            return this.PartialView("_ImageListPartial", album);
        }

    }
}