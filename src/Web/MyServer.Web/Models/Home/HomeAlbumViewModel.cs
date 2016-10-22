namespace MyServer.Web.Models.Home
{
    using System;
    using AutoMapper;

    using MyServer.Common;
    using MyServer.Data.Models;
    using MyServer.Services.Mappings;
    using MyServer.Web.Areas.Shared.Models;

    public class HomeAlbumViewModel : IMapFrom<Album>, IHaveCustomMappings
    {
        public MyServerAccessType Access { get; set; }

        public string Date { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid Id { get; set; }

        public string ImagesCountCover { get; set; }

        public string Title { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<Album, HomeAlbumViewModel>()
                .ForMember(m => m.Title, opt => opt.MapFrom(c => MappingFunctions.MapTitle(c)))
                .ForMember(m => m.ImagesCountCover, opt => opt.MapFrom(c => MappingFunctions.MapImagesCountCover(c)))
                .ForMember(m => m.Date, opt => opt.MapFrom(c => MappingFunctions.MapDate(c)));
        }
    }
}