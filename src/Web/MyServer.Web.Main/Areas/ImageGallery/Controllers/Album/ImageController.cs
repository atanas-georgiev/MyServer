namespace MyServer.Web.Main.Areas.ImageGallery.Controllers.Album
{
    using System;
    using System.IO;
    using System.Web.Mvc;

    using MyServer.Services.ImageGallery;

    public class ImageController : Controller
    {
        private readonly IImageService imageService;

        public ImageController(IImageService imageService)
        {
            this.imageService = imageService;
        }
    }
}