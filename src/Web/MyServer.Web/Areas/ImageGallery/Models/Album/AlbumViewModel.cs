namespace MyServer.Web.Areas.ImageGallery.Models.Album
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using AutoMapper;

    using MyServer.Common;
    using MyServer.Data.Models;
    using MyServer.Services.Mappings;
    using MyServer.Web.Areas.ImageGallery.Models.Image;
    using MyServer.Web.Areas.ImageGalleryAdmin.Models.Album;
    using System.Linq;

    using MyServer.Web.Areas.Shared.Models;

    public class AlbumViewModel : IMapFrom<Album>, IHaveCustomMappings
    {
        public MyServerAccessType Access { get; set; }

        public Guid? CoverId { get; set; }

        public string CoverImage { get; set; }

        public string Date { get; set; }

        [MaxLength(3000)]
        public string Description { get; set; }

        public Guid Id { get; set; }

        public IEnumerable<ImageGpsData> ImageCoordinates { get; set; }

        public List<ImageViewModel> Images { get; set; }

        public string ImagesCountCover { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(200)]
        public string Title { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<Album, AlbumViewModel>()
                .ForMember(m => m.Title, opt => opt.MapFrom(c => MappingFunctions.MapTitle(c)))
                .ForMember(m => m.Description, opt => opt.MapFrom(c => MappingFunctions.MapDescription(c)))
                .ForMember(m => m.ImagesCountCover, opt => opt.MapFrom(c => MappingFunctions.MapImagesCountCover(c)))
                .ForMember(m => m.CoverImage, opt => opt.MapFrom(c => MappingFunctions.MapCoverImage(c)))
                .ForMember(m => m.ImageCoordinates, opt => opt.MapFrom(c => MappingFunctions.MapImageCoordinates(c)))
                .ForMember(m => m.Date, opt => opt.MapFrom(c => MappingFunctions.MapDate(c)));
        }
    }
}