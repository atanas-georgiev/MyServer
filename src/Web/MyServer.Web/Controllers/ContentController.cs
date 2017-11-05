namespace MyServer.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Localization;
    using Microsoft.AspNetCore.Mvc;

    using MyServer.Common;
    using MyServer.Services.Content;

    [Route("api/[controller]")]   

    public class ContentController : Controller
    {
        private readonly IContentService contentService;

        private readonly LanguageEnum currentLanguage;

        public ContentController(IContentService contentService, IHttpContextAccessor httpContextAccessor)
        {
            this.contentService = contentService;
            var feature = httpContextAccessor.HttpContext.Features.Get<IRequestCultureFeature>();
            var currentLanguageStr = feature.RequestCulture.Culture.TwoLetterISOLanguageName.ToLower();

            switch (currentLanguageStr)
            {
                case "bg":
                    this.currentLanguage = LanguageEnum.Bg;
                    break;
                case "en":
                    this.currentLanguage = LanguageEnum.En;
                    break;
                default:
                    this.currentLanguage = LanguageEnum.En;
                    break;
            }
        }

        [HttpGet]
        public IActionResult Get(string key)
        {
            return this.Content(this.contentService.Get(key, this.currentLanguage));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Post(string key, string value)
        {
            this.contentService.Update(key, this.currentLanguage, value);
            return this.Ok();
        }
    }
}
