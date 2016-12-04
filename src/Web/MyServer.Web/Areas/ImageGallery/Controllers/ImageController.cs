using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyServer.Data;
using MyServer.Data.Models;
using MyServer.Services.ImageGallery;
using MyServer.Services.Users;
using MyServer.Web.Areas.ImageGallery.Models.Image;
using MyServer.Web.Areas.Shared.Controllers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MyServer.Web.Areas.ImageGallery.Controllers
{
    [Area("ImageGallery")]
    public class ImageController : BaseController
    {
        private readonly IAlbumService albumService;

        private readonly IImageService imageService;

        public ImageController(IUserService userService, UserManager<User> userManager, SignInManager<User> signInManager, MyServerDbContext dbContext, IAlbumService albumService, IImageService imageService) : base(userService, userManager, signInManager, dbContext)
        {
            this.imageService = imageService;
            this.albumService = albumService;
        }

        public IActionResult Index()
        {
            var dates =
                this.imageService.GetAllReqursive()
                    .OrderByDescending(x => x.DateTaken)
                    .Where(x => x.DateTaken != null)
                    .Select(x => x.DateTaken.Value.Date)
                    .Distinct()
                    .ToList();

            this.TempData["type"] = Common.ImageListType.Date;
            this.TempData["cnt"] = 1;
            this.TempData["data"] = dates;

            return this.View(new ImageListViewModel() { Type = Common.ImageListType.Date, Caption = dates.First().ToString("dd MMMM yyyy"), Data = dates.First() });
        }

        public IActionResult GetImageListData(Common.ImageListType type = Common.ImageListType.Date, string data = null)
        {
            if (type == Common.ImageListType.Date)
            {
                var dates = new List<DateTime>();

                if (data == null)
                {
                    dates =
                        this.imageService.GetAllReqursive()
                            .OrderByDescending(x => x.DateTaken)
                            .Where(x => x.DateTaken != null)
                            .Select(x => x.DateTaken.Value.Date)
                            .Distinct()
                            .ToList();               
                }
                else
                {
                    var filter = data.Split('|').Select(x => DateTime.Parse(x));

                    dates =
                        this.imageService.GetAllReqursive()
                            .OrderByDescending(x => x.DateTaken)
                            .Where(x => x.DateTaken != null && filter.Any(f => f.Date == x.DateTaken.Value.Date))
                            .Select(x => x.DateTaken.Value.Date)
                            .Distinct()
                            .ToList();
                }

                if (dates.Count > 0)
                {
                    this.TempData["cnt"] = 1;
                    this.TempData["type"] = type;
                    this.TempData["data"] = dates;

                    return this.ViewComponent("ImageList", new { type = Common.ImageListType.Date, caption = dates[0].ToString("dd MMMM yyyy"), data = dates[0] });
                }
            }
            else if (type == Common.ImageListType.Location)
            {
                var locations = new List<string>();

                if (data == null)
                {
                    locations =
                        this.imageService.GetAllReqursive()
                            .Where(x => x.ImageGpsData != null)
                            .Select(x => x.ImageGpsData.LocationName)
                            .Distinct()
                            .ToList();
                }
                else
                {
                    var filter = data.Split('|');

                    locations =
                        this.imageService.GetAllReqursive()
                            .Where(x => x.ImageGpsData != null && filter.Any(f => f.Contains(x.ImageGpsData.LocationName)))
                            .Select(x => x.ImageGpsData.LocationName)
                            .Distinct()
                            .ToList();
                }

                if (locations.Count > 0)
                {
                    this.TempData["cnt"] = 1;
                    this.TempData["type"] = type;
                    this.TempData["data"] = locations;

                    return this.ViewComponent("ImageList", new { type = Common.ImageListType.Location, caption = locations[0], data = locations[0] });
                }
            }
            else if (type == Common.ImageListType.Album)
            {
                var ids = new List<Guid>();

                if (data == null)
                {
                    ids =
                        this.albumService.GetAllReqursive()
                            .Where(x => x.TitleEn != "No Image Album")
                            .Select(x => x.Id)
                            .ToList();
                }
                else
                {
                    var filter = data.Split('|').Select(x => Guid.Parse(x));

                    ids =
                        this.albumService.GetAllReqursive()
                            .Where(x => x.TitleEn != "No Image Album" && filter.Any(f => f.ToString().Contains(x.Id.ToString())))
                            .Select(x => x.Id)
                            .ToList();
                }

                if (ids.Count > 0)
                {
                    this.TempData["cnt"] = 1;
                    this.TempData["type"] = type;
                    this.TempData["data"] = ids;

                    var caption = string.Empty;

                    if (CultureInfo.CurrentCulture.ToString().Contains("bg"))
                    {
                        caption = this.albumService.GetById(ids[0]).TitleBg;
                    }
                    else
                    {
                        caption = this.albumService.GetById(ids[0]).TitleEn;
                    }

                    return this.ViewComponent("ImageList", new { type = Common.ImageListType.Album, caption = caption, data = ids[0] });
                }
            }

            return this.NoContent();
        }

        public IActionResult FinishRender(string id)
        {
            var type = this.TempData["type"] as int?;
            var cnt = this.TempData["cnt"] as Int32?;

            if (type == (int)Common.ImageListType.Date)
            {
                var data = this.TempData["data"] as DateTime[];

                if (cnt.Value < data.Length)
                {
                    this.TempData["type"] = type;
                    this.TempData["cnt"] = cnt.Value + 1;
                    this.TempData["data"] = data;
                    return this.ViewComponent("ImageList", new { type = Common.ImageListType.Date, caption = data[cnt.Value].ToString("dd MMMM yyyy"), data = data[cnt.Value] });
                }
            }
            else if (type == (int)Common.ImageListType.Location)
            {
                var data = this.TempData["data"] as string[];

                if (cnt.Value < data.Length)
                {
                    this.TempData["type"] = type;
                    this.TempData["cnt"] = cnt.Value + 1;
                    this.TempData["data"] = data;
                    return this.ViewComponent("ImageList", new { type = Common.ImageListType.Location, caption = data[cnt.Value], data = data[cnt.Value] });
                }
            }
            else if (type == (int)Common.ImageListType.Album)
            {
                var data = this.TempData["data"] as Guid[];

                if (cnt.Value < data.Length)
                {
                    this.TempData["type"] = type;
                    this.TempData["cnt"] = cnt.Value + 1;
                    this.TempData["data"] = data;
                    var caption = string.Empty;

                    if (CultureInfo.CurrentCulture.ToString().Contains("bg"))
                    {
                        caption = this.albumService.GetById(data[cnt.Value]).TitleBg;
                    }
                    else
                    {
                        caption = this.albumService.GetById(data[cnt.Value]).TitleEn;
                    }

                    return this.ViewComponent("ImageList", new { type = Common.ImageListType.Album, caption = caption, data = data[cnt.Value] });
                }
            }

            return this.NoContent();
        }
    }
}
