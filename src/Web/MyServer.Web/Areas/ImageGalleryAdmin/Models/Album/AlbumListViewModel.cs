namespace MyServer.Web.Main.Areas.ImageGalleryAdmin.Models.Album
{
    using Microsoft.AspNetCore.Hosting;
    using MyServer.Common.ImageGallery;
    using Services.Mappings;
    using AutoMapper;
    using MyServer.Data.Models;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;
    using System.Linq;
    using Helpers;

    public class AlbumListViewModel : IMapFrom<Album>, IHaveCustomMappings
    {      
        public int Height { get; set; }
        
        public int Width { get; set; }

        public string CoverImage { get; set; }

        public string Date { get; set; }

        public Guid Id { get; set; }

        public string ImagesCountCover { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(200)]
        public string Title { get; set; }

        public bool IsPublic { get; set; }

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
                   ? Convert.ToInt32(Convert.ToDouble(Constants.ImageLowMaxSize / 2) / 1.5)
                   : source.Cover.LowHeight / 2;
        }

        static int MapWidth(Album source)
        {
            return source.CoverId == null
                   ? Constants.ImageLowMaxSize / 2
                   : source.Cover.LowWidth / 2;
        }

        static string MapCoverImage(Album source)
        {
            return PathHelper.WwwPath + Constants.MainContentFolder + "/" + source.Cover.AlbumId + "/" + Constants.ImageFolderLow + "/" + source.Cover.FileName;
        }

        static string MapImagesCountCover(Album source)
        {
            switch (source.Images.Count)
            {
                case 0:
                    return "No items";
                case 1:
                    return "1 item";
                default:
                    return source.Images.Count + " items";
            }
        }

        static string MapDate(Album source)
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
                    return firstDate.ToString("dd MMM yyyy");
                }
                else if (firstDate.Year == lastDate.Year && firstDate.Month == lastDate.Month)
                {
                    return firstDate.Day + "-" + lastDate.Day + " "
                           + firstDate.ToString("MMM yyyy");
                }
                else if (firstDate.Year == lastDate.Year)
                {
                    return firstDate.ToString("dd MMM") + "-"
                           + lastDate.ToString("dd MMM") + " "
                           + lastDate.ToString("yyyy");
                }
                else
                {
                    return firstDate.ToString("dd MMM yyyy") + "-"
                           + lastDate.ToString("dd MMM yyyy");
                }
            }
        }
    }
}