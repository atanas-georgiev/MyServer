using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyServer.Data;
using MyServer.Data.Models;
using MyServer.Services.ImageGallery;
using MyServer.Services.Users;
using MyServer.Web.Areas.Shared.Controllers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace MyServer.Web.Areas.ImageGallery.Controllers
{
    [Area("ImageGallery")]
    public class JsonController : BaseController
    {
        private readonly IAlbumService albumService;

        private readonly IImageService imageService;

        public JsonController(IUserService userService, UserManager<User> userManager, SignInManager<User> signInManager, MyServerDbContext dbContext, IAlbumService albumService, IImageService imageService) : base(userService, userManager, signInManager, dbContext)
        {
            this.imageService = imageService;
            this.albumService = albumService;
        }

        public IActionResult ReadImageListTypes([DataSourceRequest] DataSourceRequest request)
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

            var values = Enum.GetValues(typeof(MyServer.Common.ImageListType)).Cast<object>()
                             .Select(v => new
                             {
                                 Text = GetDisplayName(v).ToString(),
                                 Value = v.ToString()
                             });

            return Json(values);
        }

        public IActionResult ReadImageListData(MyServer.Common.ImageListType? type)
        {
            if (type == null || type == Common.ImageListType.Date)
            {
                var dates =
                    this.imageService.GetAllReqursive()
                        .OrderByDescending(x => x.DateTaken)
                        .Where(x => x.DateTaken != null)
                        .OrderByDescending(x => x.DateTaken)
                        .Select(x => new { Value = x.DateTaken.Value.Date, Text = x.DateTaken.Value.ToString("dd MMMM yyyy") })
                        .Distinct()
                        .ToList();

                return this.Json(dates);
            }
            else if (type == Common.ImageListType.Location)
            {
                var locations =
                    this.imageService.GetAllReqursive()
                        .OrderByDescending(x => x.DateTaken)
                        .Where(x => x.ImageGpsData != null)
                        .Select(x => new { Value = x.ImageGpsData.LocationName, Text = x.ImageGpsData.LocationName })
                        .Distinct()
                        .OrderBy(x => x.Text)
                        .ToList();

                return this.Json(locations);
            }
            else if (type == Common.ImageListType.Album)
            {
                if (CultureInfo.CurrentCulture.ToString().Contains("bg"))
                {
                    var ids =
                    this.albumService.GetAllReqursive()
                        .OrderByDescending(x => x.Images.First().DateTaken)
                        .Where(x => x.TitleBg != null)
                        .Select(x => new { Value = x.Id, Text = x.TitleBg + "(" + x.Images.First().DateTaken.Value.ToString("MMMM yyyy") + ")" })
                        .Distinct()
                        .ToList();

                    return this.Json(ids);
                }
                else
                {
                    var ids =
                    this.albumService.GetAllReqursive()
                        .OrderByDescending(x => x.Images.First().DateTaken)
                        .Where(x => x.TitleEn != null)
                        .Select(x => new { Value = x.Id, Text = x.TitleEn + "(" + x.Images.First().DateTaken.Value.ToString("MMMM yyyy") + ")" })
                        .Distinct()
                        .ToList();

                    return this.Json(ids);
                }
            }

            return this.Json("");
        }
    }
}
