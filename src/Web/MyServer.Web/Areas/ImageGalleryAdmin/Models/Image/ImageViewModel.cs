namespace MyServer.Web.Main.Areas.ImageGalleryAdmin.Models.Image
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using AutoMapper;
    using MyServer.Common.ImageGallery;
    using MyServer.Data.Models;
    using Services.Mappings;
    using Microsoft.AspNetCore.Hosting;
    using System.Text;
    using System.Globalization;

    public class ImageViewModel : IMapFrom<Image>//, IHaveCustomMappings
    {
        public Guid Id { get; set; }

        //public Guid? AlbumId { get; set; }

        //[MaxLength(50)]
        //public string Aperture { get; set; }

        //[MaxLength(50)]
        //public string CameraMaker { get; set; }

        //[MaxLength(50)]
        //public string CameraModel { get; set; }

        //public DateTime? DateTaken { get; set; }

        //[MaxLength(50)]
        //public string ExposureBiasStep { get; set; }

        //[MaxLength(200)]
        //public string FileName { get; set; }

        //[MaxLength(50)]
        //public string FocusLen { get; set; }

        //public int Height { get; set; }
        
        //public string Info { get; set; }

        //public virtual GpsDataViewModel ImageGpsData { get; set; }

        //[Computed]
        //public string GpsName => this.ImageGpsData?.LocationName;

        //[Computed]
        //public List<double> GpsCoordinates => this.ImageGpsData != null ? new List<double>() { this.ImageGpsData.Latitude.Value, this.ImageGpsData.Longitude.Value } : null;

        //[MaxLength(50)]
        //public string Iso { get; set; }

        //[MaxLength(100)]
        //public string Lenses { get; set; }

        //public int LowHeight { get; set; }
        
        //public string LowImageSource { get; set; }

        //public int LowWidth { get; set; }
        
        //public string MiddleImageSource { get; set; }

        //public int MidHeight { get; set; }

        //public int MidWidth { get; set; }

        //public string OriginalDownloadPath { get; set; }

        //[MaxLength(100)]
        //public string OriginalFileName { get; set; }

        //[MaxLength(50)]
        //public string ShutterSpeed { get; set; }

        //[MaxLength(200)]
        //public string Title { get; set; }

        //public int Width { get; set; }

        //public void CreateMappings(IMapperConfigurationExpression configuration)
        //{
        //   // configuration.CreateMap<Image, ImageViewModel>()
        //     //   .ForMember(m => m.Info, opt => opt.MapFrom(src => MapStatus(src)));
        //        //.ForMember(m => m.OriginalDownloadPath, opt => opt.MapFrom(c => this.appEnvironment + Constants.TempContentFolder + "\\" + this.Id + "\\" + this.OriginalFileName))
        //        //.ForMember(m => m.MiddleImageSource, opt => opt.MapFrom(c => this.appEnvironment + Constants.MainContentFolder + "\\" + this.AlbumId + "\\" + Constants.ImageFolderMiddle + "\\" + this.FileName))
        //        //.ForMember(m => m.LowImageSource, opt => opt.MapFrom(c => this.appEnvironment + Constants.MainContentFolder + "\\" + this.AlbumId + "\\" + Constants.ImageFolderLow + "\\" + this.FileName));
        //}

        //private string MapStatus(Image source)
        //{
        //    var result = new StringBuilder();

        //    if (!string.IsNullOrEmpty(source.Title))
        //    {
        //        result.Append(source.Title + "<br/>");
        //    }

        //    result.Append("<small>");

        //    if (!string.IsNullOrEmpty(source.CameraMaker))
        //    {
        //        result.Append(source.CameraMaker + " ");
        //    }

        //    if (!string.IsNullOrEmpty(source.CameraModel))
        //    {
        //        result.Append(source.CameraModel + " ");
        //    }

        //    if (!string.IsNullOrEmpty(source.Lenses))
        //    {
        //        result.Append(source.Lenses + " ");
        //    }

        //    result.Append("<br/>");

        //    if (!string.IsNullOrEmpty(source.Aperture))
        //    {
        //        result.Append(source.Aperture + " ");
        //    }

        //    if (!string.IsNullOrEmpty(source.ShutterSpeed))
        //    {
        //        result.Append(source.ShutterSpeed + " ");
        //    }

        //    if (source.FocusLen != null)
        //    {
        //        result.Append(source.FocusLen + " ");
        //    }

        //    if (source.Iso != null)
        //    {
        //        result.Append("ISO " + source.Iso + " ");
        //    }

        //    if (source.ExposureBiasStep != null)
        //    {
        //        result.Append(source.ExposureBiasStep);
        //    }

        //    if (source.DateTaken != null)
        //    {
        //        result.Append(
        //            "<br/>" + source.DateTaken.Value.ToString("dd-MMM-yy"));
        //    }

        //    result.Append("</small>");

        //    return result.ToString();
        //}
    }
}