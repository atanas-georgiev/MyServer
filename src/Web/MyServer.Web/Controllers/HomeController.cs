namespace MyServer.Web.Controllers
{
    using System;
    using System.Linq;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Localization;
    using Microsoft.AspNetCore.Mvc;

    using MyServer.Services.ImageGallery;
    using MyServer.Services.Mappings;
    using MyServer.Web.Models.Home;

    public class HomeController : Controller
    {
        public IActionResult Error()
        {
            return this.View();
        }

        public IActionResult Index()
        {
            return this.View();
        }

        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            this.Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });

            return this.LocalRedirect(returnUrl);
        }
    }
}