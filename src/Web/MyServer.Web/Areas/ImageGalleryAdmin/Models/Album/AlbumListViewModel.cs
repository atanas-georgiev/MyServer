namespace MyServer.Web.Areas.ImageGalleryAdmin.Models.Album
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    using AutoMapper;

    using MyServer.Common;
    using MyServer.Common.ImageGallery;
    using MyServer.Data.Models;
    using MyServer.Services.Mappings;
    using MyServer.Web.Areas.ImageGallery.Models.Image;

    public class AlbumListViewModel : IMapFrom<Album>, IHaveCustomMappings
    {
        public MyServerAccessType Access { get; set; }

        public string CoverImage { get; set; }

        public string Date { get; set; }

        public int Height { get; set; }

        public Guid Id { get; set; }

        public List<ImageViewModel> Images { get; set; }

        public string ImagesCountCover { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(200)]
        public string Title { get; set; }

        public int Width { get; set; }

        public static string MapCoverImage(Album source)
        {
            return Constants.MainContentFolder + "/" + source.Cover.AlbumId + "/" + Constants.ImageFolderLow + "/"
                   + source.Cover.FileName;
        }

        public static string MapDate(Album source)
        {
            if (source.Images == null)
            {
                return string.Empty;
            }

            var dates = source.Images.Where(x => x.DateTaken != null).Select(x => x.DateTaken).ToList();

            if (dates.Count == 0)
            {
                return string.Empty;
            }
            else
            {
                var firstDate = dates.OrderBy(x => x.Value).Select(x => x.Value).First();
                var lastDate = dates.OrderBy(x => x.Value).Select(x => x.Value).Last();

                if (firstDate.Date == lastDate.Date)
                {
                    return firstDate.ToString("dd MMMM yyyy");
                }
                else if (firstDate.Year == lastDate.Year && firstDate.Month == lastDate.Month)
                {
                    return firstDate.Day + "-" + lastDate.Day + " " + firstDate.ToString("MMMM yyyy");
                }
                else if (firstDate.Year == lastDate.Year)
                {
                    return firstDate.ToString("dd MMMM") + "-" + lastDate.ToString("dd MMMM") + " "
                           + lastDate.ToString("yyyy");
                }
                else
                {
                    return firstDate.ToString("dd MMMM yyyy") + "-" + lastDate.ToString("dd MMMM yyyy");
                }
            }
        }

        public static string MapImagesCountCover(Album source)
        {
            switch (source.Images.Count)
            {
                case 0:
                    return Startup.SharedLocalizer["NoItems"];
                case 1:
                    return "1 " + Startup.SharedLocalizer["Item"];
                default:
                    return source.Images.Count + " " + Startup.SharedLocalizer["Items"];
            }
        }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<Album, AlbumListViewModel>()
                .ForMember(m => m.Width, opt => opt.MapFrom(c => MapWidth(c)))
                .ForMember(m => m.Height, opt => opt.MapFrom(c => MapHeight(c)))
                .ForMember(m => m.ImagesCountCover, opt => opt.MapFrom(c => MapImagesCountCover(c)))
                .ForMember(m => m.CoverImage, opt => opt.MapFrom(c => MapCoverImage(c)))
                .ForMember(m => m.Date, opt => opt.MapFrom(c => MapDate(c)));
        }

        static int MapHeight(Album source)
        {
            return source.CoverId == null
                       ? Convert.ToInt32(Convert.ToDouble(Constants.ImageLowMaxSize) / 1.5)
                       : source.Cover.LowHeight;
        }

        static int MapWidth(Album source)
        {
            return source.CoverId == null ? Constants.ImageLowMaxSize : source.Cover.LowWidth;
        }
    }
}