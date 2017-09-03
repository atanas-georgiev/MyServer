namespace MyServer.ViewComponents.ImageGallery.Components.AllAlbums.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;

    using MyServer.Common;
    using MyServer.Services.ImageGallery;
    using MyServer.Services.Mappings;
    using MyServer.ViewComponents.ImageGallery.Components.AllAlbums.Models;
    using MyServer.ViewComponents.ImageGallery.Resources;
    using MyServer.ViewComponents.ImageGallery._Common.Models;

    public class AllAlbumsViewComponent : ViewComponent
    {
        private readonly IAlbumService albumService;

        private readonly IStringLocalizer<ViewComponentResources> localizer;

        public AllAlbumsViewComponent(IAlbumService albumService, IStringLocalizer<ViewComponentResources> localizer)
        {
            this.albumService = albumService;
            this.localizer = localizer;
            MappingFunctions.LoadResource(this.localizer);
        }

        public IViewComponentResult Invoke(
            string ViewDetailsUrl,
            string NewAlbumUrl = null,
            string Filter = null,
            MyServerSortType Sort = MyServerSortType.SortImagesDateDesc)
        {
            var albums = this.albumService.GetAllReqursive();

            switch (Sort)
            {
                case MyServerSortType.SortDateAddedAsc:
                    albums = albums.OrderBy(x => x.CreatedOn);
                    break;
                case MyServerSortType.SortDateAddedDesc:
                    albums = albums.OrderByDescending(x => x.CreatedOn);
                    break;
                case MyServerSortType.SortImagesCountAsc:
                    albums = albums.OrderBy(x => x.Images.Count);
                    break;
                case MyServerSortType.SortImagesCountDesc:
                    albums = albums.OrderByDescending(x => x.Images.Count);
                    break;
                case MyServerSortType.SortImagesDateAsc:
                    albums = albums.OrderBy(
                        x => x.Images.OrderBy(d => d.DateTaken).LastOrDefault() != null
                                 ? x.Images.OrderBy(d => d.DateTaken).LastOrDefault().DateTaken
                                 : null);
                    break;
                case MyServerSortType.SortImagesDateDesc:
                    albums = albums.OrderByDescending(
                        x => x.Images.OrderBy(d => d.DateTaken).LastOrDefault() != null
                                 ? x.Images.OrderBy(d => d.DateTaken).LastOrDefault().DateTaken
                                 : null);
                    break;
            }

            if (!string.IsNullOrEmpty(Filter))
            {
                Filter = Filter.ToLower();
                albums = albums.Where(
                    x => (x.TitleBg != null && x.TitleBg.ToLower().Contains(Filter))
                         || (x.TitleEn != null && x.TitleEn.ToLower().Contains(Filter))
                         || (x.DescriptionBg != null && x.DescriptionBg.ToLower().Contains(Filter))
                         || (x.DescriptionEn != null && x.DescriptionEn.ToLower().Contains(Filter)));
            }

            this.ViewBag.StringLocalizer = this.localizer;
            this.ViewBag.ViewDetailsUrl = ViewDetailsUrl;
            this.ViewBag.NewAlbumUrl = NewAlbumUrl;

            try
            {
                return this.View(albums?.To<AllAlbumsViewModel>()?.ToList());
            }
            catch (NullReferenceException)
            {
                return this.View(new List<AllAlbumsViewModel>());
            }
        }
    }
}