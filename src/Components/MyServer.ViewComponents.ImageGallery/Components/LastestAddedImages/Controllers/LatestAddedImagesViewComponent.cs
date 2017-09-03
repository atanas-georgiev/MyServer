namespace MyServer.ViewComponents.ImageGallery.Components.LastestAddedImages.Controllers
{
    using System.Linq;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;

    using MyServer.Services.ImageGallery;
    using MyServer.Services.Mappings;
    using MyServer.ViewComponents.ImageGallery.Components.LastestAddedImages.Models;
    using MyServer.ViewComponents.ImageGallery.Resources;
    using MyServer.ViewComponents.ImageGallery._Common.Models;

    public class LatestAddedImagesViewComponent : ViewComponent
    {
        private readonly IImageService imageService;

        private readonly IStringLocalizer<ViewComponentResources> localizer;

        public LatestAddedImagesViewComponent(
            IImageService imageService,
            IStringLocalizer<ViewComponentResources> localizer)
        {
            this.imageService = imageService;
            this.localizer = localizer;
            MappingFunctions.LoadResource(this.localizer);
        }

        public IViewComponentResult Invoke(string allImagesRoute)
        {
            var images = this.imageService.GetAllReqursive().OrderByDescending(x => x.CreatedOn).Take(18)
                .To<LatestAddedImagesViewModel>().ToList();

            this.ViewBag.StringLocalizer = this.localizer;
            this.ViewBag.AllImagesRoute = allImagesRoute;

            return this.View(images);
        }
    }
}