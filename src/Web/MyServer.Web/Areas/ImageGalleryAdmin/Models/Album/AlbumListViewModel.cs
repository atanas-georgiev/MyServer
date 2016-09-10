namespace MyServer.Web.Main.Areas.ImageGalleryAdmin.Models.Album
{
    using Microsoft.AspNetCore.Hosting;
    using MyServer.Common.ImageGallery;
    using Shared.Models.Album;
    using Services.Mappings;
    using AutoMapper;
    using MyServer.Data.Models;
    using System;

    public class AlbumListViewModel : AlbumViewModel, IHaveCustomMappings
    {      
        public int Height { get; set; }
        
        public int Width { get; set; }

        public override void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<Album, AlbumListViewModel>()
            .ForMember(m => m.Width, opt => opt.MapFrom(c => MapWidth(c)))
            .ForMember(m => m.Height, opt => opt.MapFrom(c => MapHeight(c)));

            base.CreateMappings(configuration);
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
    }
}