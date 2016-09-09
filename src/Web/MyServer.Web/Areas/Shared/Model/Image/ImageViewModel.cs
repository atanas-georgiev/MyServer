namespace MyServer.Web.Main.Areas.Shared.Models.Image
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using AutoMapper;
    using MyServer.Common.ImageGallery;
    using MyServer.Data.Models;
    using Services.Mappings;
    using Microsoft.AspNetCore.Hosting;

    public class ImageViewModel : IMapFrom<Image>//, IHaveCustomMappings
    {
        //private readonly IHostingEnvironment appEnvironment;

        //public ImageViewModel(IHostingEnvironment appEnvironment)
        //{
        //    this.appEnvironment = appEnvironment;
        //}

        public Guid Id { get; set; }

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

        public int Height { get; set; }

        //[Computed]
        //public string Info
        //{
        //    get
        //    {
        //        var result = new StringBuilder();

        //        if (!string.IsNullOrEmpty(this.Title))
        //        {
        //            result.Append(this.Title + "<br/>");
        //        }

        //        result.Append("<small>");

        //        if (!string.IsNullOrEmpty(this.CameraMaker))
        //        {
        //            result.Append(this.CameraMaker + " ");
        //        }

        //        if (!string.IsNullOrEmpty(this.CameraModel))
        //        {
        //            result.Append(this.CameraModel + " ");
        //        }

        //        if (!string.IsNullOrEmpty(this.Lenses))
        //        {
        //            result.Append(this.Lenses + " ");
        //        }

        //        result.Append("<br/>");

        //        if (!string.IsNullOrEmpty(this.Aperture))
        //        {
        //            result.Append(this.Aperture + " ");
        //        }

        //        if (!string.IsNullOrEmpty(this.ShutterSpeed))
        //        {
        //            result.Append(this.ShutterSpeed + " ");
        //        }

        //        if (this.FocusLen != null)
        //        {
        //            result.Append(this.FocusLen + " ");
        //        }

        //        if (this.Iso != null)
        //        {
        //            result.Append("ISO " + this.Iso + " ");
        //        }

        //        if (this.ExposureBiasStep != null)
        //        {
        //            result.Append(this.ExposureBiasStep);
        //        }

        //        if (this.DateTaken != null)
        //        {
        //            result.Append(
        //                "<br/>" + this.DateTaken.Value.ToString("dd-MMM-yy", CultureInfo.CreateSpecificCulture("en-US")));
        //        }

        //        result.Append("</small>");

        //        return result.ToString();
        //    }
        //}

        //public virtual GpsDataViewModel ImageGpsData { get; set; }

        //[Computed]
        //public string GpsName => this.ImageGpsData?.LocationName;

        //[Computed]
        //public List<double> GpsCoordinates => this.ImageGpsData != null ? new List<double>() { this.ImageGpsData.Latitude.Value, this.ImageGpsData.Longitude.Value } : null;

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

        //public void CreateMappings(IMapperConfigurationExpression configuration)
        //{
        //    configuration.CreateMap<Image, ImageViewModel>()
        //        .ForMember(m => m.OriginalDownloadPath, opt => opt.MapFrom(c => this.appEnvironment + Constants.TempContentFolder + "\\" + this.Id + "\\" + this.OriginalFileName))
        //        .ForMember(m => m.MiddleImageSource, opt => opt.MapFrom(c => this.appEnvironment + Constants.MainContentFolder + "\\" + this.AlbumId + "\\" + Constants.ImageFolderMiddle + "\\" + this.FileName))
        //        .ForMember(m => m.LowImageSource, opt => opt.MapFrom(c => this.appEnvironment + Constants.MainContentFolder + "\\" + this.AlbumId + "\\" + Constants.ImageFolderLow + "\\" + this.FileName));
        //}
    }
}