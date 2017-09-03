namespace MyServer.ViewComponents.ImageGallery.Components.LatestAddedAlbums.Models
{
    using System;

    using AutoMapper;

    using MyServer.Common;
    using MyServer.Data.Models;
    using MyServer.Services.Mappings;
    using MyServer.ViewComponents.ImageGallery._Common.Models;

    public class LatestAddedAlbumsViewModel : IMapFrom<Album>, IHaveCustomMappings
    {
        public MyServerAccessType Access { get; set; }

        public DateTime CreatedOn { get; set; }

        public string Date { get; set; }

        public Guid Id { get; set; }

        public string ImagesCountCover { get; set; }

        public string Title { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<Album, LatestAddedAlbumsViewModel>()
                .ForMember(m => m.Title, opt => opt.MapFrom(c => MappingFunctions.MapTitle(c))).ForMember(
                    m => m.ImagesCountCover,
                    opt => opt.MapFrom(c => MappingFunctions.MapImagesCountCover(c))).ForMember(
                    m => m.Date,
                    opt => opt.MapFrom(c => MappingFunctions.MapDate(c)));
        }
    }
}