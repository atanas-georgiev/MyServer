using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MyServer.Data.Models;
using MyServer.Services.ImageGallery;
using MyServer.Services.Mappings;
using MyServer.ViewComponents.ImageGallery._Common.Models;
using MyServer.ViewComponents.ImageGallery.Components.AllAlbums.Models;
using MyServer.ViewComponents.ImageGallery.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyServer.ViewComponents.ImageGallery.Components.AllAlbums.Controllers
{
    public class AllAlbumsViewComponent : ViewComponent
    {
        private readonly IAlbumService albumService;

        private readonly IStringLocalizer<ViewComponentResources> localizer;

        public AllAlbumsViewComponent(IAlbumService albumService, IStringLocalizer<ViewComponentResources> localizer)
        {
            this.albumService = albumService;
            this.localizer = localizer;
            MappingFunctions.LoadResource(this.localizer);
        }

        public IViewComponentResult Invoke(string ViewDetailsUrl, string NewAlbumUrl = null, string Filter = null)
        {
            var albums = this.albumService.GetAllReqursive();

            if (!this.User.Identity.IsAuthenticated)
            {
                albums = albums
                        .Where(x => x.Access == Common.MyServerAccessType.Public)
                        .OrderByDescending(x => x.Images.OrderBy(d => d.DateTaken).LastOrDefault() != null ? x.Images.OrderBy(d => d.DateTaken).LastOrDefault().DateTaken : null);
            }
            else if (this.User.IsInRole(Common.MyServerRoles.User.ToString()))
            {
                albums = albums
                        .Where(x => x.Access != Common.MyServerAccessType.Private)
                        .OrderByDescending(x => x.Images.OrderBy(d => d.DateTaken).LastOrDefault() != null ? x.Images.OrderBy(d => d.DateTaken).LastOrDefault().DateTaken : null);
            }
            else if (this.User.IsInRole(Common.MyServerRoles.Admin.ToString()))
            {
                albums = albums
                        .OrderByDescending(x => x.Images.OrderBy(d => d.DateTaken).LastOrDefault() != null ? x.Images.OrderBy(d => d.DateTaken).LastOrDefault().DateTaken : null);
            }

            if (!string.IsNullOrEmpty(Filter))
            {
                Filter = Filter.ToLower();
                albums = albums.Where(x => (x.TitleBg != null && x.TitleBg.ToLower().Contains(Filter))
                                        || (x.TitleEn != null && x.TitleEn.ToLower().Contains(Filter))
                                        || (x.DescriptionBg != null && x.DescriptionBg.ToLower().Contains(Filter))
                                        || (x.DescriptionEn != null && x.DescriptionEn.ToLower().Contains(Filter)));
            }

            this.ViewBag.StringLocalizer = this.localizer;
            this.ViewBag.ViewDetailsUrl = ViewDetailsUrl;
            this.ViewBag.NewAlbumUrl = NewAlbumUrl;

            try
            {
                return this.View(albums?.To<AllAlbumsViewModel>()?.ToList());
            }
            catch (NullReferenceException)
            {
                return View(new List<AllAlbumsViewModel>());
            }
            
        }
    }
}
