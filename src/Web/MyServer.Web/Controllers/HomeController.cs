namespace MyServer.Web.Controllers
{
    using System;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Localization;
    using Microsoft.AspNetCore.Mvc;
    using NLog;

    public class HomeController : Controller
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        public IActionResult Index()
        {
            var user = string.IsNullOrEmpty(this.User.Identity.Name) ? "Annonymous" : this.User.Identity.Name;
            Logger.Info("Home page, user -> " + user + " -> " +  this.Request.HttpContext.Connection.RemoteIpAddress);
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