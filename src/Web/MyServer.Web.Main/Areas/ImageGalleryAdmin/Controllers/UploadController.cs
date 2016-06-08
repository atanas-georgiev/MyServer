namespace ImageGallery.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web;
    using System.Web.Mvc;

    using ImageGallery.Services.Album;
    using ImageGallery.Services.Image;
    using ImageGallery.Services.User;

    public class UploadController : BaseController
    {
        private readonly IAlbumService albumService;

        private readonly IImageService imageService;

        public UploadController(IUserService userService, IAlbumService albumService, IImageService imageService)
            : base(userService)
        {
            this.albumService = albumService;
            this.imageService = imageService;
        }

        public ActionResult Remove(string[] fileNames)
        {
            // The parameter of the Remove action must be called "fileNames"
            if (fileNames != null)
            {
                foreach (var fullName in fileNames)
                {
                    var fileName = Path.GetFileName(fullName);
                    var physicalPath = Path.Combine(this.Server.MapPath("~/App_Data"), fileName);

                    // TODO: Verify user permissions
                    if (System.IO.File.Exists(physicalPath))
                    {
                        // The files are not actually removed in this demo
                        // System.IO.File.Delete(physicalPath);
                    }
                }
            }

            // Return an empty string to signify success
            return this.Content(string.Empty);
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

                    // Some browsers send file names with full path.
                    // We are only interested in the file name.
                    // var fileName = Path.GetFileName(file.FileName);
                    // var physicalPath = Path.Combine(Server.MapPath("~/App_Data"), fileName);

                    // The files are not actually saved in this demo
                    // file.SaveAs(physicalPath);
                }
            }

            // Return an empty string to signify success
            return this.Content(string.Empty);
        }
    }
}