﻿namespace MyServer.Web.Controllers
{
    using System;
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
            IQueryable<HomeAlbumViewModel> albums = null;
            var albumsCount = this.albumService.GetAllReqursive().Count();
            var imagesCount = this.imageService.GetAllReqursive().Count();
            var allSize = this.fileService.GetImageFolderSize();

            if (!this.User.Identity.IsAuthenticated)
            {
                albums =
                    this.albumService.GetAllReqursive()
                        .Where(x => x.Access == Common.MyServerAccessType.Public)
                        .To<HomeAlbumViewModel>();
            }
            else if (this.User.IsInRole(Common.MyServerRoles.User.ToString()))
            {
                albums =
                    this.albumService.GetAllReqursive()
                        .Where(x => x.Access != Common.MyServerAccessType.Private)
                        .To<HomeAlbumViewModel>();
            }
            else if (this.User.IsInRole(Common.MyServerRoles.Admin.ToString()))
            {
                albums = this.albumService.GetAllReqursive().To<HomeAlbumViewModel>();
            }

            this.ViewData["LatestAlbums"] = albums;
            this.ViewData["AlbumsCount"] = albumsCount;
            this.ViewData["ImagesCount"] = imagesCount;
            this.ViewData["AllSize"] = allSize;
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