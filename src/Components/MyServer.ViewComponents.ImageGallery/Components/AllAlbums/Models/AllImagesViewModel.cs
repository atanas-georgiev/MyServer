namespace MyServer.ViewComponents.ImageGallery.Components.AllAlbums.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using AutoMapper;

    using MyServer.Common.ImageGallery;
    using MyServer.Data.Models;
    using MyServer.Services.Mappings;
    using MyServer.ViewComponents.ImageGallery._Common.Models;

    public class AllImagesViewModel : IMapFrom<Image>, IHaveCustomMappings
    {
        public Guid? AlbumId { get; set; }

        [MaxLength(50)]
        public string Aperture { get; set; }

        [MaxLength(50)]
        public string CameraMaker { get; set; }

        [MaxLength(50)]
        public string CameraModel { get; set; }

        public DateTime? DateTaken { get; set; }

        [MaxLength(50)]
        public string ExposureBiasStep { get; set; }

        [MaxLength(200)]
        public string FileName { get; set; }

        [MaxLength(50)]
        public string FocusLen { get; set; }

        public List<double> GpsCoordinates { get; set; }

        public string GpsName { get; set; }

        public int Height { get; set; }

        public Guid Id { get; set; }

        public GpsDataViewModel ImageGpsData { get; set; }

        public string Info { get; set; }

        [MaxLength(50)]
        public string Iso { get; set; }

        [MaxLength(100)]
        public string Lenses { get; set; }

        public int LowHeight { get; set; }

        public string LowImageSource { get; set; }

        public int LowWidth { get; set; }

        public string MiddleImageSource { get; set; }

        public int MidHeight { get; set; }

        public int MidWidth { get; set; }

        public string OriginalDownloadPath { get; set; }

        [MaxLength(100)]
        public string OriginalFileName { get; set; }

        [MaxLength(50)]
        public string ShutterSpeed { get; set; }

        [MaxLength(200)]
        public string Title { get; set; }

        public int Width { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<Image, AllImagesViewModel>()
                .ForMember(m => m.Info, opt => opt.MapFrom(src => MappingFunctions.MapInfo(src)))
                .ForMember(m => m.GpsCoordinates, opt => opt.MapFrom(src => MappingFunctions.MapGpsCoordinates(src)))
                .ForMember(m => m.GpsName, opt => opt.MapFrom(src => MappingFunctions.MapGpsName(src)))
                .ForMember(
                    m => m.OriginalDownloadPath,
                    opt => opt.MapFrom(
                        c => Constants.MainContentFolder + "/" + c.AlbumId + "/" + Constants.ImageFolderOriginal + "/"
                             + c.FileName))
                .ForMember(
                    m => m.MiddleImageSource,
                    opt => opt.MapFrom(
                        c => Constants.MainContentFolder + "/" + c.AlbumId + "/" + Constants.ImageFolderMiddle + "/"
                             + c.FileName)).ForMember(
                    m => m.LowImageSource,
                    opt => opt.MapFrom(
                        c => Constants.MainContentFolder + "/" + c.AlbumId + "/" + Constants.ImageFolderLow + "/"
                             + c.FileName));
        }
    }
}