namespace MyServer.Web.Main.Areas.ImageGallery.Models.Image
{
    using AutoMapper;

    using MyServer.Common.ImageGallery;
    using MyServer.Data.Models.ImageGallery;
    using MyServer.Web.Infrastructure.Mappings;

    public class ImageViewModel : IMapFrom<Image>, IHaveCustomMappings
    {
        public string tumbsrc { get; set; }

        //public string msrc { get; set; }

        public string src { get; set; }

        public int w { get; set; }

        public int h { get; set; }

        public string title { get; set; }

        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<Image, ImageViewModel>(string.Empty)
                .ForMember(m => m.src,
                    opt =>
                        opt.MapFrom(
                            c =>
                                Constants.MainContentFolder + "\\" + c.Album.Id + "\\" +
                                Constants.ImageFolderMiddle + "\\" + c.FileName))
//                .ForMember(m => m.msrc,
//                    opt =>
//                        opt.MapFrom(
//                            c =>
//                                Common.Constants.MainContentFolder + "\\" + c.AlbumId + "\\" +
//                                Common.Constants.ImageFolderMiddle + "\\" + c.FileName))
                .ForMember(m => m.tumbsrc,
                    opt =>
                        opt.MapFrom(
                            c =>
                                Constants.MainContentFolder + "\\" + c.Album.Id + "\\" +
                                Constants.ImageFolderLow + "\\" + c.FileName))
                .ForMember(m => m.title, opt => opt.MapFrom(c => c.Title))
                .ForMember(m => m.w, opt => opt.MapFrom(c => c.MidWidth))
                .ForMember(m => m.h, opt => opt.MapFrom(c => c.MidHeight));
        }
    }
}