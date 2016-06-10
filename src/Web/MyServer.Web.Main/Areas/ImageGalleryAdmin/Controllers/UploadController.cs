namespace MyServer.Web.Main.Areas.ImageGalleryAdmin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Mvc;

    using MyServer.Services.ImageGallery;
    using MyServer.Services.Users;

    public class UploadController : BaseController
    {
        private readonly IImageService imageService;

        public UploadController(IUserService userService, IImageService imageService)
            : base(userService)
        {
            this.imageService = imageService;
        }

        public ActionResult Save(IEnumerable<HttpPostedFileBase> files)
        {
            var albumId = Guid.Parse(this.Session["AlbumId"].ToString());

            // The Name of the Upload component is "files"
            if (files != null)
            {
                foreach (var file in files)
                {
                    this.imageService.Add(albumId, file, System.Web.HttpContext.Current.Server);
                }
            }

            // Return an empty string to signify success
            return this.Content(string.Empty);
        }
    }
}