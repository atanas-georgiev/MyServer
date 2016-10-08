namespace MyServer.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using MyServer.Services.ImageGallery;

    public class HomeController : Controller
    {
        private readonly IAlbumService albumService;

        private readonly IFileService fileService;

        public HomeController(IFileService fileService, IAlbumService albumService)
        {
            this.fileService = fileService;
            this.albumService = albumService;
        }

        public IActionResult About()
        {
            this.ViewData["Message"] = "Your application description page.";

            return this.View();
        }

        public IActionResult Contact()
        {
            this.ViewData["Message"] = "Your contact page.";

            return this.View();
        }

        public IActionResult Error()
        {
            return this.View();
        }

        public IActionResult Index()
        {
            return this.View();
        }
    }
}