namespace MyServer.Web.Main.Areas.ImageGalleryAdmin.Models.Album
{
    using Microsoft.AspNetCore.Hosting;
    using MyServer.Common.ImageGallery;
    using Shared.Models.Album;
    using Services.Mappings;
    using AutoMapper;
    using MyServer.Data.Models;

    public class AlbumListViewModel : AlbumViewModel, IHaveCustomMappings
    {
        //private readonly IHostingEnvironment appEnvironment;

        //public AlbumListViewModel(IHostingEnvironment appEnvironment)
        //{
        //    this.appEnvironment = appEnvironment;
        //}

        public int Height { get; set; }
        //=>
        //    this.CoverId == null
        //            ? Convert.ToInt32(Convert.ToDouble(Constants.ImageLowMaxSize / 2) / 1.5)
        //            : this.Cover.LowHeight / 2;
        
        public int Width { get; set; }
        //=> 
        //    this.CoverId == null
        //            ? Constants.ImageLowMaxSize / 2
        //            : this.Cover.LowWidth / 2;

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            //configuration.CreateMap<Album, AlbumListViewModel>()
                //.ForMember(m => m.CoverImage, opt => opt.MapFrom(c => this.appEnvironment + Constants.MainContentFolder + "/" + this.Cover.AlbumId + "/" + Constants.ImageFolderLow + "/" + this.Cover.FileName))
               // .ForMember(m => m.Width, opt => opt.MapFrom(c => this.Cover.LowWidth / 2))
               // .ForMember(m => m.Height, opt => opt.MapFrom(c => this.Cover.LowHeight / 2));

        }
    }
}