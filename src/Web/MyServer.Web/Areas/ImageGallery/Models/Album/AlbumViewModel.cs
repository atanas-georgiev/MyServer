using MyServer.Services.Mappings;
using MyServer.Data.Models;
using System;
using MyServer.Web.Areas.ImageGallery.Models.Image;
using AutoMapper;
using MyServer.Web.Main.Areas.ImageGalleryAdmin.Models.Album;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using MyServer.Common;

namespace MyServer.Web.Areas.ImageGallery.Models.Album
{
    public class AlbumViewModel : IMapFrom<MyServer.Data.Models.Album>, IHaveCustomMappings
    {
        public Guid? CoverId { get; set; }

        public string CoverImage { get; set; }

      //  public ImageViewModel Cover { get; set; }

        public string Date { get; set; }

        [MaxLength(3000)]
        public string Description { get; set; }

        public Guid Id { get; set; }

        public IEnumerable<GpsDataViewModel> ImageCoordinates { get; set; }

        public ICollection<ImageViewModel> Images { get; set; }

        public string ImagesCountCover { get; set; }

        public MyServerAccessType Access { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(200)]
        public string Title { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<MyServer.Data.Models.Album, AlbumViewModel>()
                .ForMember(m => m.ImagesCountCover, opt => opt.MapFrom(c => AlbumListViewModel.MapImagesCountCover(c)))
                .ForMember(m => m.CoverImage, opt => opt.MapFrom(c => AlbumListViewModel.MapCoverImage(c)))
               // .ForMember(m => m.ImageCoordinates, opt => opt.MapFrom(c => MapImageCoordinates(c)))
                .ForMember(m => m.Date, opt => opt.MapFrom(c => AlbumListViewModel.MapDate(c)));
        }

        public static IEnumerable<GpsDataViewModel> MapImageCoordinates(MyServer.Data.Models.Album source)
        {
            return null;// source.Images?.Where(x => x.ImageGpsData != null).Select(x => x.ImageGpsData).Distinct();
        }
    }
}