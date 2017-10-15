namespace MyServer.ViewComponents.Common.Components.Content.Controllers
{
    using System;
    using System.Globalization;

    using Microsoft.AspNetCore.Mvc;

    using MyServer.Common;
    using MyServer.Services.Content;

    public class ContentViewComponent : ViewComponent
    {
        private readonly IContentService contentService;

        public ContentViewComponent(IContentService contentService)
        {
            this.contentService = contentService;
        }

        public IViewComponentResult Invoke(string key, bool edit = true)
        {
            var culture = CultureInfo.CurrentCulture.ToString();
            var value = string.Empty;

            if (culture == "bg-BG")
            {
                value = this.contentService.Get("mykey", LanguageEnum.Bg);
            }
            else if (culture == "en-US")
            {
                value = this.contentService.Get("mykey", LanguageEnum.En);
            }

            return this.View(new Tuple<string, bool>(value, edit));
        }
    }
}