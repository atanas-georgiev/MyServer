namespace MyServer.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Localization;
    using Microsoft.AspNetCore.Mvc;

    using MyServer.Services.ImageGallery;
    using MyServer.Services.Mappings;
    using MyServer.Web.Models.Home;

    public class HomeController : Controller
    {
        private readonly IAlbumService albumService;

        private readonly IFileService fileService;

        private readonly IImageService imageService;

        public HomeController(IFileService fileService, IAlbumService albumService, IImageService imageService)
        {
            this.fileService = fileService;
            this.albumService = albumService;
            this.imageService = imageService;
        }

        public IActionResult Error()
        {
            return this.View();
        }

        public IActionResult Index()
        {
            var albums = new List<HomeAlbumViewModel>();

            if (!this.User.Identity.IsAuthenticated)
            {
                albums =
                    this.albumService.GetAllReqursive()
                        .Where(x => x.Access == Common.MyServerAccessType.Public)
                        .OrderByDescending(x => x.CreatedOn)
                        .Take(5)
                        .To<HomeAlbumViewModel>()
                        .ToList();
            }
            else if (this.User.IsInRole(Common.MyServerRoles.User))
            {
                albums =
                    this.albumService.GetAllReqursive()
                        .Where(x => x.Access != Common.MyServerAccessType.Private)
                        .OrderByDescending(x => x.CreatedOn)
                        .Take(5)
                        .To<HomeAlbumViewModel>()
                        .ToList();
            }
            else if (this.User.IsInRole(Common.MyServerRoles.Admin))
            {
                albums =
                    this.albumService.GetAllReqursive()
                        .OrderByDescending(x => x.CreatedOn)
                        .Take(5)
                        .To<HomeAlbumViewModel>()
                        .ToList();
            }

            this.ViewData["LatestAlbums"] = albums;
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