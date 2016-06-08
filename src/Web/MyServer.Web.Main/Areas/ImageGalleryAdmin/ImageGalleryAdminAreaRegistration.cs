using System.Web.Mvc;

namespace MyServer.Web.Main.Areas.ImageGalleryAdmin
{
    public class ImageGalleryAdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ImageGalleryAdmin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "ImageGalleryAdmin_default",
                "ImageGalleryAdmin/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}