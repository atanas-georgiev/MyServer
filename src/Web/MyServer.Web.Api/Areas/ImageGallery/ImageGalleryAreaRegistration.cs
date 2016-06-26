using System.Web.Mvc;

namespace MyServer.Web.Api.Areas.ImageGallery
{
    public class ImageGalleryAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ImageGallery";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "ImageGallery_default",
                "ImageGallery/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}