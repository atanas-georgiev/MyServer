using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MyServer.Common;
using MyServer.Common.ImageGallery;
using MyServer.Services.ImageGallery;
using MyServer.Services.Mappings;
using MyServer.ViewComponents.ImageGallery._Common.Models;
using MyServer.ViewComponents.ImageGallery.Components.ImageList.Models;
using MyServer.ViewComponents.ImageGallery.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyServer.ViewComponents.ImageGallery.Components.ImageList.Controllers
{
    public class ImageListViewComponent : ViewComponent
    {
        private readonly IAlbumService albumService;

        private readonly IImageService imageService;

        private readonly IStringLocalizer<ViewComponentResources> localizer;

        public ImageListViewComponent(IAlbumService albumService, IImageService imageService, IStringLocalizer<ViewComponentResources> localizer)
        {
            this.albumService = albumService;
            this.imageService = imageService;
            this.localizer = localizer;
            MappingFunctions.LoadResource(this.localizer);
        }

        public IViewComponentResult Invoke(ImageListType type, string caption, object data)
        {
            this.ViewBag.Caption = caption;
            List<ImageListViewModel> images = null;

            if (type == ImageListType.Album)
            {
                var id = data as Guid?;
                images =
                    this.imageService.GetAllReqursive()
                        .Where(x => x.AlbumId == id)
                        .To<ImageListViewModel>()
                        .OrderBy(x => x.DateTaken)
                        .ToList();
            }
            else if (type == ImageListType.Date)
            {
                var date = (data as DateTime?).Value.Date;

                images =
                    this.imageService.GetAllReqursive()
                        .Where(x => x.DateTaken != null && x.DateTaken.Value.Date == date)
                        .To<ImageListViewModel>()
                        .OrderByDescending(x => x.DateTaken)
                        .ToList(); ;
            }
            else if (type == ImageListType.Location)
            {
                var location = data as string;

                images =
                    this.imageService.GetAllReqursive()
                        .Where(x => x.ImageGpsData != null && x.ImageGpsData.LocationName == location)
                        .To<ImageListViewModel>()
                        .OrderByDescending(x => x.DateTaken)
                        .ToList(); ;
            }

            return this.View(images);
        }
    }
}
