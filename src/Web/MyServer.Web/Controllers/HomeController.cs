using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyServer.Services.ImageGallery;
using MyServer.Web.Areas.ImageGalleryAdmin.Models.Album;
using MyServer.Services.Mappings;
using MyServer.Data.Models;

namespace MyServer.Controllers
{
    public class HomeController : Controller
    {
        private readonly IFileService fileService;
        private readonly IAlbumService albumService;


        public HomeController(IFileService fileService, IAlbumService albumService)
        {
            this.fileService = fileService;
            this.albumService = albumService;
        }

        public IActionResult Index()
        {
            var album = new Album()
            {
                Id = Guid.NewGuid(),
                Description = "Mine"
            };
            var albums = new List<Album>();
            albums.Add(album);

            var result = albums.AsQueryable().To<AddAlbumViewModel>().FirstOrDefault();

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
