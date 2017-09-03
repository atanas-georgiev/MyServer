namespace MyServer.ViewComponents.ImageGallery.Components.LatestAddedAlbums.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;

    using MyServer.Services.ImageGallery;
    using MyServer.Services.Mappings;
    using MyServer.ViewComponents.ImageGallery.Components.LatestAddedAlbums.Models;
    using MyServer.ViewComponents.ImageGallery.Resources;
    using MyServer.ViewComponents.ImageGallery._Common.Models;

    public class LatestAddedAlbumsViewComponent : ViewComponent
    {
        private readonly IAlbumService albumService;

        private readonly IStringLocalizer<ViewComponentResources> localizer;

        public LatestAddedAlbumsViewComponent(
            IAlbumService albumService,
            IStringLocalizer<ViewComponentResources> localizer)
        {
            this.albumService = albumService;
            this.localizer = localizer;
            MappingFunctions.LoadResource(this.localizer);
        }

        public IViewComponentResult Invoke(string allAlbumsRoute)
        {
            var albums = this.albumService.GetAllReqursive().To<LatestAddedAlbumsViewModel>();

            this.ViewBag.StringLocalizer = this.localizer;
            this.ViewBag.AllAlbumsRoute = allAlbumsRoute;

            return this.View(albums);
        }
    }
}