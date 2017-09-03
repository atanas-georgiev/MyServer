namespace MyServer.ViewComponents.Common.Components.Social.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class SocialViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return this.View();
        }
    }
}