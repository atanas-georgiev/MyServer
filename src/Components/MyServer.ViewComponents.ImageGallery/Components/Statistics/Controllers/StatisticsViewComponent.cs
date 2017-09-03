namespace MyServer.ViewComponents.ImageGallery.Components.Statistics.Controllers
{
    using System.Linq;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;

    using MyServer.Services.ImageGallery;
    using MyServer.ViewComponents.ImageGallery.Resources;
    using MyServer.ViewComponents.ImageGallery._Common.Models;

    public class StatisticsViewComponent : ViewComponent
    {
        private readonly IAlbumService albumService;

        private readonly IFileService fileService;

        private readonly IImageService imageService;

        private readonly IStringLocalizer<ViewComponentResources> localizer;

        public StatisticsViewComponent(
            IFileService fileService,
            IAlbumService albumService,
            IImageService imageService,
            IStringLocalizer<ViewComponentResources> localizer)
        {
            this.fileService = fileService;
            this.albumService = albumService;
            this.imageService = imageService;
            this.localizer = localizer;
            MappingFunctions.LoadResource(this.localizer);
        }

        public IViewComponentResult Invoke()
        {
            this.ViewBag.AlbumsCount = this.albumService.GetAllReqursive().Count();
            this.ViewBag.ImagesCount = this.imageService.GetAllReqursive().Count();
            this.ViewBag.AllSize = this.fileService.GetImageFolderSize();
            this.ViewBag.StringLocalizer = this.localizer;

            return this.View();
        }
    }
}