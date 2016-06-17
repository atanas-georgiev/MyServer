namespace MyServer.Web.Main.Areas.ImageGalleryAdmin.Controllers
{
    using System.Web.Mvc;

    using MyServer.Services.Users;
    using MyServer.Web.Main.Areas.ImageGallery.Models.Image;

    public class ImageController : BaseController
    {
        public ImageController(IUserService userService)
            : base(userService)
        {
        }
    }
}