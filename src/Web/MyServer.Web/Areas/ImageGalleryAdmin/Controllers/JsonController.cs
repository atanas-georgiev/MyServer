using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyServer.Data;
using MyServer.Data.Models;
using MyServer.Services.ImageGallery;
using MyServer.Services.Mappings;
using MyServer.Services.Users;
using MyServer.Web.Areas.ImageGallery.Models.Image;
using MyServer.Web.Areas.Shared.Controllers;
using MyServer.Web.Main.Areas.ImageGalleryAdmin.Models.Album;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyServer.Web.Areas.ImageGalleryAdmin.Controllers
{
    [Area("ImageGalleryAdmin")]
    [Route("ImageGalleryAdmin/Json")]
    public class JsonController : BaseController
    {
        private readonly IAlbumService albumService;

        private readonly IImageService imageService;

        public JsonController(IUserService userService, UserManager<User> userManager, SignInManager<User> signInManager, MyServerDbContext dbContext, IAlbumService albumService, IImageService imageService) : base(userService, userManager, signInManager, dbContext)
        {
            this.albumService = albumService;
            this.imageService = imageService;
        }

        [Route("AlbumsRead")]
        [HttpPost]
        public ActionResult AlbumsRead([DataSourceRequest] DataSourceRequest request)
        {
            var tasks = this.albumService.GetAllReqursive().To<AlbumListViewModel>().ToList();
            return this.Json(tasks.ToDataSourceResult(request));
        }

        [Route("AlbumsDestroy")]
        [HttpPost]
        public ActionResult AlbumsDestroy([DataSourceRequest] DataSourceRequest request, AlbumListViewModel album)
        {
            if (album != null)
            {               
                foreach (var image in album.Images)
                {
                    this.imageService.Remove(image.Id);
                }

                this.albumService.Remove(album.Id);
            }

            return this.Json(this.ModelState.ToDataSourceResult());
        }

        [Route("ImagesGridRead")]
        public ActionResult ImagesGridRead([DataSourceRequest] DataSourceRequest request)
        {
            var albumId = Guid.Parse(this.Request.Cookies["AlbumId"]);
            var result = this.imageService.GetAllReqursive().Where(x => x.AlbumId == albumId).To<ImageViewModel>().ToList();
            return this.Json(result.ToDataSourceResult(request));
        }

        [Route("ImagesGridUpdate")]
        [HttpPost]
        public ActionResult ImagesGridUpdate([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<ImageViewModel> images)
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

        [Route("ImagesGridDestroy")]
        [HttpPost]
        public ActionResult ImagesGridDestroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<ImageViewModel> images)
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