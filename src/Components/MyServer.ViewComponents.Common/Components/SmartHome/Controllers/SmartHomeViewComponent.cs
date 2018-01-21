using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MyServer.Services.SmartHome;
using MyServer.ViewComponents.Common.Resources;

namespace MyServer.ViewComponents.Common.Components.SmartHome.Controllers
{
    public class SmartHomeViewComponent : ViewComponent
    {
        private readonly IHomeTemparatures homeTemparatures;

        private readonly IStringLocalizer<ViewComponentResources> localizer;

        public SmartHomeViewComponent(IHomeTemparatures homeTemparatures, IStringLocalizer<ViewComponentResources> localizer)
        {
            this.homeTemparatures = homeTemparatures;
            this.localizer = localizer;
        }

        public IViewComponentResult Invoke()
        {
            this.ViewBag.StringLocalizer = this.localizer;
            homeTemparatures.Update();
            this.ViewBag.Temeratures = homeTemparatures.GetTemeratures();
            return this.View();
        }
    }
}
