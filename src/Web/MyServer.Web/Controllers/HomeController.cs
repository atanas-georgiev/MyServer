namespace MyServer.Web.Controllers
{
    using System;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Localization;
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

        public IActionResult Error()
        {
            return this.View();
        }

        public IActionResult Index()
        {
            return this.View();
        }
        
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            this.Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });

            return this.LocalRedirect(returnUrl);
        }
    }
}