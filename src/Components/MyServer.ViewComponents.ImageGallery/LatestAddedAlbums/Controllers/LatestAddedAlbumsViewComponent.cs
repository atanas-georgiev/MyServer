namespace MyServer.ViewComponents.ImageGallery.LatestAddedAlbums.Controllers
{
    using System.Linq;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;

    using MyServer.Services.ImageGallery;
    using MyServer.Services.Mappings;
    using MyServer.ViewComponents.ImageGallery.LatestAddedAlbums.Models;
    using Resources;

    public class LatestAddedAlbumsViewComponent : ViewComponent
    {
        private readonly IAlbumService albumService;

        private readonly IStringLocalizer<ViewComponentResources> localizer;

        public LatestAddedAlbumsViewComponent(IAlbumService albumService, IStringLocalizer<ViewComponentResources> localizer)
        {
            this.albumService = albumService;
            this.localizer = localizer;
        }

        public IViewComponentResult Invoke()
        {
            IQueryable<LatestAddedAlbumsViewModel> albums = null;

            if (!this.User.Identity.IsAuthenticated)
            {
                albums =
                    this.albumService.GetAllReqursive()
                        .Where(x => x.Access == Common.MyServerAccessType.Public)
                        .To<LatestAddedAlbumsViewModel>();
            }
            else if (this.User.IsInRole(Common.MyServerRoles.User.ToString()))
            {
                albums =
                    this.albumService.GetAllReqursive()
                        .Where(x => x.Access != Common.MyServerAccessType.Private)
                        .To<LatestAddedAlbumsViewModel>();
            }
            else if (this.User.IsInRole(Common.MyServerRoles.Admin.ToString()))
            {
                albums = this.albumService.GetAllReqursive().To<LatestAddedAlbumsViewModel>();
            }

            this.ViewBag.StringLocalizer = this.localizer;
            return this.View("LastestAddedAlbums", albums);
        }
    }
}