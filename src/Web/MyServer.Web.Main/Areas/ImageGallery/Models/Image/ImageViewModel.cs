namespace MyServer.Web.Main.Areas.ImageGallery.Models.Image
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using AutoMapper;

    using MyServer.Common.ImageGallery;
    using MyServer.Data.Models.ImageGallery;
    using MyServer.Web.Infrastructure.Mappings;

    public class ImageViewModel : IMapFrom<Image>, IHaveCustomMappings
    {
        public string Aperture { get; set; }

        public string CameraMaker { get; set; }

        public string CameraModel { get; set; }

        public DateTime? DateTaken { get; set; }

        public double? ExposureBiasStep { get; set; }

        public string FileName { get; set; }

        public double? FocusLen { get; set; }

        public int Height { get; set; }

        //public string ImageGpsDataLocationName { get; set; }

        //public double? ImageGpsDataLatitude { get; set; }

        //public double? ImageGpsDataLongitude { get; set; }

        public int? Iso { get; set; }

        public string Lenses { get; set; }

        public int LowHeight { get; set; }

        public int LowWidth { get; set; }

        public int MidHeight { get; set; }

        public int MidWidth { get; set; }

        public string OriginalFileName { get; set; }

        public string ShutterSpeed { get; set; }

        [MaxLength(150)]
        public string Title { get; set; }

        public int Width { get; set; }

        public string LowImageSource { get; set; }

        public string MiddleImageSource { get; set; }

        public string Info { get; set; }

        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<Image, ImageViewModel>(string.Empty)
                .ForMember(
                    m => m.LowImageSource,
                    opt =>
                    opt.MapFrom(
                        c =>
                        Constants.MainContentFolder + "\\" + c.Album.Id + "\\" + Constants.ImageFolderLow + "\\"
                        + c.FileName))
                .ForMember(
                    m => m.MiddleImageSource,
                    opt =>
                    opt.MapFrom(
                        c =>
                        Constants.MainContentFolder + "\\" + c.Album.Id + "\\" + Constants.ImageFolderMiddle + "\\"
                        + c.FileName));
        }
    }
}