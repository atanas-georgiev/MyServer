namespace MyServer.Web.Main.Areas.ImageGallery.Controllers.Album
{
    using System;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    using MyServer.Services.ImageGallery;
    using MyServer.Web.Infrastructure.Mappings;
    using MyServer.Web.Main.Areas.ImageGallery.Models.Album;

    public class AlbumController : Controller
    {
        private readonly IAlbumService albumService;

        public AlbumController(IAlbumService albumService)
        {
            this.albumService = albumService;
        }

        public ActionResult Details(string id)
        {
            var album =
                this.albumService.GetAll().Where(x => x.Id.ToString() == id).To<AlbumViewModel>().FirstOrDefault();
            return this.View(album);
        }

        public ActionResult Download(string id)
        {
            var zip = this.albumService.GenerateZipArchive(Guid.Parse(id), this.HttpContext.Server);
            return this.Content(zip.Replace("~", string.Empty));
        }

        public ActionResult Index()
        {
            var albums = this.albumService.GetAll().To<AlbumViewModel>();
            return this.View(albums);

        }
    }
}