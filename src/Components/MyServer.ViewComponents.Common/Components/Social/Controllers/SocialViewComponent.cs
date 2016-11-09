using Microsoft.AspNetCore.Mvc;

namespace MyServer.ViewComponents.Common.Components.Social.Controllers
{
    public class SocialViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {        
            return this.View();
        }
    }
}
