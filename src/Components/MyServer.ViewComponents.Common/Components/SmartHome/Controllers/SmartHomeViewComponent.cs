namespace MyServer.ViewComponents.Common.Components.SmartHome.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;

    using MyServer.Services.SmartHome;
    using MyServer.ViewComponents.Common.Resources;

    public class SmartHomeViewComponent : ViewComponent
    {
        private readonly IHomeTemparatures homeTemparatures;

        private readonly IStringLocalizer<ViewComponentResources> localizer;

        public SmartHomeViewComponent(
            IHomeTemparatures homeTemparatures,
            IStringLocalizer<ViewComponentResources> localizer)
        {
            this.homeTemparatures = homeTemparatures;
            this.localizer = localizer;
        }

        public IViewComponentResult Invoke()
        {
            this.ViewBag.StringLocalizer = this.localizer;
            this.ViewBag.Temeratures = this.homeTemparatures.GetTemeratures();
            return this.View();
        }
    }
}