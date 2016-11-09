using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MyServer.Services.ImageGallery;
using MyServer.Services.Mappings;
using MyServer.ViewComponents.ImageGallery._Common.Models;
using MyServer.ViewComponents.ImageGallery.Components.AllAlbums.Models;
using MyServer.ViewComponents.ImageGallery.Resources;
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

        public IViewComponentResult Invoke()
        {
            List<AllAlbumsViewModel> albums = new List<AllAlbumsViewModel>();

            if (!this.User.Identity.IsAuthenticated)
            {
                albums =
                    this.albumService.GetAllReqursive()
                        .Where(x => x.Access == Common.MyServerAccessType.Public)
                        .OrderByDescending(x => x.Images.OrderBy(d => d.DateTaken).LastOrDefault() != null ? x.Images.OrderBy(d => d.DateTaken).LastOrDefault().DateTaken : null)
                        .To<AllAlbumsViewModel>()
                        .ToList();
            }
            else if (this.User.IsInRole(Common.MyServerRoles.User.ToString()))
            {
                albums =
                    this.albumService.GetAllReqursive()
                        .Where(x => x.Access != Common.MyServerAccessType.Private)
                        .OrderByDescending(x => x.Images.OrderBy(d => d.DateTaken).LastOrDefault() != null ? x.Images.OrderBy(d => d.DateTaken).LastOrDefault().DateTaken : null)
                        .To<AllAlbumsViewModel>()
                        .ToList();
            }
            else if (this.User.IsInRole(Common.MyServerRoles.Admin.ToString()))
            {
                albums =
                    this.albumService.GetAllReqursive()
                        .OrderByDescending(x => x.Images.OrderBy(d => d.DateTaken).LastOrDefault() != null ? x.Images.OrderBy(d => d.DateTaken).LastOrDefault().DateTaken : null)
                        .To<AllAlbumsViewModel>()
                        .ToList();
            }

            this.ViewBag.StringLocalizer = this.localizer;
            return this.View(albums);
        }
    }
}
