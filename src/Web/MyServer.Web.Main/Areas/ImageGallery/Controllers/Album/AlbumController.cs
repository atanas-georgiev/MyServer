namespace MyServer.Web.Main.Areas.ImageGallery.Controllers.Album
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;

    using AutoMapper;

    using MyServer.Services.ImageGallery;
    using MyServer.Web.Infrastructure.Mappings;
    using MyServer.Web.Main.Areas.ImageGallery.Models.Album;
    using MyServer.Web.Main.Areas.ImageGallery.Models.Image;
    using MyServer.Web.Main.Areas.ImageGalleryAdmin.Models.Album;

    public class AlbumController : Controller
    {
        private readonly IAlbumService albumService;

        public AlbumController(IAlbumService albumService)
        {
            this.albumService = albumService;
        }
        
        public ActionResult Index()
        {
            var albums = this.albumService.GetAll().To<AlbumViewModel>();            
            return this.View(albums);
        }

        public ActionResult Details(string id)
        {
            var images = this.albumService.GetById(Guid.Parse(id));
            var images2 = images.Images.AsQueryable().To<ImageViewModel>().ToList();

            return this.View(images2);
        }
    }
}