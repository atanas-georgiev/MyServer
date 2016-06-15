﻿namespace MyServer.Web.Main.Areas.ImageGalleryAdmin.Controllers
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
    using MyServer.Web.Main.Areas.ImageGalleryAdmin.Models.Album;

    using Newtonsoft.Json;

    public class AlbumController : BaseController
    {
        private readonly IAlbumService albumService;

        private readonly ILocationService locationService;

        private readonly IImageService imageService;

        public AlbumController(IUserService userService, IAlbumService albumService, ILocationService locationService, IImageService imageService)
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
                                    AddedById = this.User.Identity.GetUserId()
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
            var result = this.albumService.GetAll().Where(x => x.Id == intId).To<AlbumDetailsViewModel>().FirstOrDefault();

            if (result == null)
            {
                return this.HttpNotFound("Album not found");
            }

            return this.View(result);
        }

        public ActionResult UpdateAlbumCover(string id)
        {
            var albumId = Guid.Parse(this.Request.Cookies["AlbumId"].Value);
            var coverId = Guid.Parse(id);

            this.albumService.UpdateCoverImage(albumId, coverId);

            return this.RedirectToAction("Details", new { Id = albumId });
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
        public ActionResult UpdateGpsData(string items, string location)
        {
            var ids = JsonConvert.DeserializeObject<IEnumerable<Guid>>(items);
            var gpsData = this.locationService.GetGpsData(location);

            if (gpsData != null)
            {
                foreach (var id in ids)
                {
                    this.imageService.AddGpsDataToImage(id, gpsData);
                }
            }

            return this.Content(string.Empty);
        }
    }
}