namespace MyServer.ViewComponents.Common.Components.Content.Controllers
{
    using System;
    using System.Globalization;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;

    using MyServer.Common;
    using MyServer.Services.Content;
    using MyServer.ViewComponents.Common.Resources;

    public class ContentViewComponent : ViewComponent
    {
        private readonly IStringLocalizer<ViewComponentResources> localizer;

        public ContentViewComponent(IStringLocalizer<ViewComponentResources> localizer)
        {
            this.localizer = localizer;
        }

        public IViewComponentResult Invoke(string key)
        {
            this.ViewBag.StringLocalizer = this.localizer;
            return this.View((object)key);
        }
    }
}