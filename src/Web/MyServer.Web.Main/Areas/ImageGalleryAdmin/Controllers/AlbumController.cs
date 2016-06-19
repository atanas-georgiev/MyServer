namespace MyServer.Web.Main.Areas.ImageGalleryAdmin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    using Microsoft.AspNet.Identity;

    using MyServer.Data.Models;
    using MyServer.Services.ImageGallery;
    using MyServer.Services.Users;
    using MyServer.Web.Infrastructure.Mappings;
    using MyServer.Web.Main.Areas.ImageGallery.Models.Image;
    using MyServer.Web.Main.Areas.ImageGalleryAdmin.Models.Album;
    using MyServer.Web.Main.Areas.ImageGalleryAdmin.Models.Image;

    using Newtonsoft.Json;

    public class AlbumController : BaseController
    {
        private readonly IAlbumService albumService;

        private readonly ILocationService locationService;

        private readonly IImageService imageService;

        public AlbumController(
            IUserService userService,
            IAlbumService albumService,
            ILocationService locationService,
            IImageService imageService)
            : base(userService)
        {
            this.albumService = albumService;
            this.locationService = locationService;
            this.imageService = imageService;
        }

        public ActionResult Add()
        {
            return this.View(new AddAlbumViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(AddAlbumViewModel model)
        {
            if (this.ModelState.IsValid && model != null)
            {
                var album = new Album()
                                {
                                    Title = model.Title,
                                    Description = model.Description,
                                    CreatedOn = DateTime.UtcNow,
                                    AddedBy = this.UserProfile,
                                    IsPublic = model.IsPublic,
                                    Cover = this.imageService.GetAll().First()
                };

                this.albumService.Add(album);
                return this.RedirectToAction("Index");
            }

            return this.View(model);
        }

        public ActionResult Details(string id)
        {
            this.Response.SetCookie(new HttpCookie("AlbumId", id));
            var intId = Guid.Parse(id);
            var result =
                this.albumService.GetAll().Where(x => x.Id == intId).To<AlbumDetailsViewModel>().FirstOrDefault();

            if (result == null)
            {
                return this.HttpNotFound("Album not found");
            }

            return this.View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(AlbumDetailsViewModel model)
        {
            if (this.ModelState.IsValid && model != null)
            {
                var album = this.albumService.GetById(model.Id);

                if (album == null)
                {
                    return this.HttpNotFound();
                }

                album.Title = model.Title;
                album.Description = model.Description;
                album.IsPublic = model.IsPublic;

                this.albumService.Update(album);

                return this.RedirectToAction("Details", album.Id);
            }

            return this.View(model);
        }

        public ActionResult Index()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateImageLocation(ImageUpdateViewModel model)
        {
            if (model != null && !string.IsNullOrEmpty(model.Items))
            {
                var ids = model.Items.Split(',');
                var gpsData = this.locationService.GetGpsData(model.Data);

                foreach (var id in ids)
                {
                    this.imageService.AddGpsDataToImage(Guid.Parse(id), gpsData);
                }

                var imageId = Guid.Parse(ids.First());
                var albumId = this.imageService.GetById(imageId).AlbumId;
                var album = this.albumService.GetAll().Where(x => x.Id == albumId).To<AlbumDetailsViewModel>().First();

                return this.PartialView("_ImageListPartial", album);
            }

            return this.Content(string.Empty);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateImageTitle(ImageUpdateViewModel model)
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

                var imageId = Guid.Parse(ids.First());
                var albumId = this.imageService.GetById(imageId).AlbumId;
                var album = this.albumService.GetAll().Where(x => x.Id == albumId).To<AlbumDetailsViewModel>().First();

                return this.PartialView("_ImageListPartial", album);
            }

            return this.Content(string.Empty);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateImageDate(ImageUpdateViewModel model)
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

                var imageId = Guid.Parse(ids.First());
                var albumId = this.imageService.GetById(imageId).AlbumId;
                var album = this.albumService.GetAll().Where(x => x.Id == albumId).To<AlbumDetailsViewModel>().First();

                return this.PartialView("_ImageListPartial", album);
            }

            return this.Content(string.Empty);
        }

        [HttpPost]
        public ActionResult DeleteImages(string id)
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

                var album = this.albumService.GetAll().Where(x => x.Id == albumId).To<AlbumDetailsViewModel>().First();

                return this.PartialView("_ImageListPartial", album);
            }

            return this.Content(string.Empty);
        }

        [HttpPost]
        public ActionResult UpdateAlbumCover(string id)
        {
            var albumId = Guid.Parse(this.Request.Cookies["AlbumId"].Value);
            var coverId = Guid.Parse(id);

            this.albumService.UpdateCoverImage(albumId, coverId);

            return this.Content(string.Empty);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public PartialViewResult AlbumDataPartial(AlbumDetailsViewModel model)
        {
            if (this.ModelState.IsValid && model != null)
            {
                var album = this.albumService.GetById(model.Id);

                //if (album == null)
                //{
                //    return this.HttpNotFound();
                //}

                album.Title = model.Title;
                album.Description = model.Description;
                album.IsPublic = model.IsPublic;

                this.albumService.Update(album);

                var result = this.albumService.GetAll().Where(a => a.Id == album.Id).To<AlbumDetailsViewModel>().First();

                return this.PartialView("_AlbumDataPartial", result);
            }

            return this.PartialView("_AlbumDataPartial", model);
        }

        [HttpPost]
        public PartialViewResult UpdateImages()
        {
            var albumId = Guid.Parse(this.Request.Cookies["AlbumId"].Value);
            var album = this.albumService.GetAll().Where(x => x.Id == albumId).To<AlbumDetailsViewModel>().First();

            return this.PartialView("_ImageListPartial", album);
        }
    }
}