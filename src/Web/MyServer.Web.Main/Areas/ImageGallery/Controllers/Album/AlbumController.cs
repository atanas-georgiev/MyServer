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

            foreach (var image in images2)
            {
                image.LowImageSource = VirtualPathUtility.ToAbsolute(image.LowImageSource);
                image.MiddleImageSource = VirtualPathUtility.ToAbsolute(image.MiddleImageSource);
                image.Info = this.GetImageInfo(image);
            }

            return this.View(images2);
        }

        private string GetImageInfo(ImageViewModel image)
        {
            var result = new StringBuilder();

            if (!string.IsNullOrEmpty(image.Title))
            {
                result.Append(image.Title + "<br/>");
            }

            result.Append("<small>");

            if (!string.IsNullOrEmpty(image.CameraMaker))
            {
                result.Append(image.CameraMaker + " ");
            }

            if (!string.IsNullOrEmpty(image.CameraModel))
            {
                result.Append(image.CameraModel + " ");
            }

            if (!string.IsNullOrEmpty(image.Lenses))
            {
                result.Append(image.Lenses + " ");
            }

            result.Append("<br/>");

            if (!string.IsNullOrEmpty(image.Aperture))
            {
                result.Append(image.Aperture + " ");
            }

            if (!string.IsNullOrEmpty(image.ShutterSpeed))
            {
                result.Append(image.ShutterSpeed + " ");
            }

            if (image.FocusLen != null)
            {
                result.Append(image.FocusLen + " mm ");
            }

            if (image.Iso != null)
            {
                result.Append("ISO" + image.FocusLen + " ");
            }

            if (image.ExposureBiasStep != null)
            {
                result.Append(image.ExposureBiasStep + " step ");
            }

            if (image.DateTaken != null)
            {
                result.Append("<br/>" + image.DateTaken.Value.ToString("yyyy-MMM-dd", CultureInfo.CreateSpecificCulture("en-US")));
            }

            result.Append("</small>");

            return result.ToString();
        }
    }
}