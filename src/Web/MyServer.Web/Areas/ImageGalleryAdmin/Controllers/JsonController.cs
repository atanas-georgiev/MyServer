using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyServer.Data;
using MyServer.Data.Models;
using MyServer.Services.ImageGallery;
using MyServer.Services.Users;
using MyServer.Web.Areas.Shared.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MyServer.Web.Areas.ImageGalleryAdmin.Controllers
{
    [Area("ImageGalleryAdmin")]
    public class JsonController : BaseController
    {
        private readonly IAlbumService albumService;

        private readonly IImageService imageService;

        public JsonController(IUserService userService, UserManager<User> userManager, SignInManager<User> signInManager, MyServerDbContext dbContext, IAlbumService albumService, IImageService imageService) : base(userService, userManager, signInManager, dbContext)
        {
            this.imageService = imageService;
            this.albumService = albumService;
        }

        public IActionResult ReadSortListTypes([DataSourceRequest] DataSourceRequest request)
        {
            Func<object, string> GetDisplayName = o =>
            {
                var result = null as string;
                var display = o.GetType()
                               .GetTypeInfo()
                               .GetMember(o.ToString()).First()
                               .GetCustomAttributes(false)
                               .OfType<DisplayAttribute>()
                               .LastOrDefault();
                if (display != null)
                {
                    result = display.GetName();
                }

                return result ?? o.ToString();
            };

            var values = Enum.GetValues(typeof(MyServer.Common.MyServerSortType)).Cast<object>()
                             .Select(v => new
                             {
                                 Text = GetDisplayName(v).ToString(),
                                 Value = v.ToString()
                             });

            return Json(values);
        }
    }
}
