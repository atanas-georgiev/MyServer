﻿namespace MyServer.Web.Main.Areas.ImageGalleryAdmin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using Kendo.Mvc.Extensions;
    using Kendo.Mvc.UI;

    using MyServer.Services.ImageGallery;
    using MyServer.Services.Users;
    using MyServer.Web.Infrastructure.Mappings;
    using MyServer.Web.Main.Areas.ImageGalleryAdmin.Models.Album;
    using MyServer.Web.Main.Areas.ImageGalleryAdmin.Models.Image;

    public class JsonController : BaseController
    {
        private readonly IAlbumService albumService;

        private readonly IImageService imageService;

        public JsonController(IUserService userService, IAlbumService albumService, IImageService imageService)
            : base(userService)
        {
            this.albumService = albumService;
            this.imageService = imageService;
        }

        public ActionResult Albums_Read([DataSourceRequest] DataSourceRequest request)
        {
            var tasks = this.albumService.GetAll().To<AlbumListViewModel>().ToList();
            return this.Json(tasks.ToDataSourceResult(request));
        }

        public ActionResult ImagesGridRead([DataSourceRequest] DataSourceRequest request)
        {
            var id = Guid.Parse(this.Request.Cookies["AlbumId"].Value);
            var result = this.imageService.GetAll().Where(x => x.Album.Id == id).To<ImageDetailsViewModel>(); 
            return this.Json(result.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ImagesGridUpdate([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<ImageDetailsViewModel> images)
        {
            if (images != null && this.ModelState.IsValid)
            {
                foreach (var image in images)
                {
                    var itemToUpdate = this.imageService.GetById(image.Id);
                    itemToUpdate.Title = image.Title;
                    this.imageService.Update(itemToUpdate);
                }
            }

            return this.Json(images.ToDataSourceResult(request, this.ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ImagesGridDestroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<ImageDetailsViewModel> images)
        {
            if (images.Any())
            {
                foreach (var image in images)
                {
                    this.imageService.Remove(image.Id);
                }
            }

            return this.Json(images.ToDataSourceResult(request, this.ModelState));
        }
    }
}