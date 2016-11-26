namespace MyServer.Web.Areas.ImageGalleryAdmin.Models.Album
{
    using System;

    using AutoMapper;

    using MyServer.Data.Models;
    using MyServer.Services.Mappings;
    using MyServer.Web.Areas.Shared.Models;

    public class AlbumListViewModel : IMapFrom<Album>, IHaveCustomMappings
    {
        public string CoverImage { get; set; }

        public string Date { get; set; }

        public int Height { get; set; }

        public Guid Id { get; set; }

        public string ImagesCountCover { get; set; }

        public string Title { get; set; }

        public int Width { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<Album, AlbumListViewModel>()
                .ForMember(m => m.Title, opt => opt.MapFrom(c => MappingFunctions.MapTitle(c)))
                .ForMember(m => m.Width, opt => opt.MapFrom(c => MappingFunctions.MapWidth(c)))
                .ForMember(m => m.Height, opt => opt.MapFrom(c => MappingFunctions.MapHeight(c)))
                .ForMember(m => m.ImagesCountCover, opt => opt.MapFrom(c => MappingFunctions.MapImagesCountCover(c)))
                .ForMember(m => m.CoverImage, opt => opt.MapFrom(c => MappingFunctions.MapCoverImage(c)))
                .ForMember(m => m.Date, opt => opt.MapFrom(c => MappingFunctions.MapDate(c)));
        }
    }
}