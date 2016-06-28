namespace MyServer.Web.Api.Models.ImageGallery
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;
    using System.Web;

    using DelegateDecompiler;

    using MyServer.Common.ImageGallery;
    using MyServer.Data.Models;
    using MyServer.Web.Infrastructure.Mappings;

    using Newtonsoft.Json;

    public class ImageModel : IMapFrom<Image>
    {
        [JsonIgnore]
        public Guid? AlbumId { get; set; }

        [JsonIgnore]
        [MaxLength(50)]
        public string Aperture { get; set; }

        [JsonIgnore]
        [MaxLength(50)]
        public string CameraMaker { get; set; }

        [JsonIgnore]
        [MaxLength(50)]
        public string CameraModel { get; set; }

        public DateTime? DateTaken { get; set; }

        [JsonIgnore]
        [MaxLength(50)]
        public string ExposureBiasStep { get; set; }

        [JsonIgnore]
        [MaxLength(200)]
        public string FileName { get; set; }

        [JsonIgnore]
        [MaxLength(50)]
        public string FocusLen { get; set; }

        [JsonIgnore]
        [Computed]
        public List<double> GpsCoordinates
            =>
                this.ImageGpsData != null
                    ? new List<double>() { this.ImageGpsData.Latitude.Value, this.ImageGpsData.Longitude.Value }
                    : null;

        [Computed]
        public string GpsName => this.ImageGpsData?.LocationName;

        public int Height { get; set; }

        public Guid Id { get; set; }

        [JsonIgnore]
        public virtual GpsDataModel ImageGpsData { get; set; }

        [Computed]
        public string Info
        {
            get
            {
                var result = new StringBuilder();

                if (!string.IsNullOrEmpty(this.Title))
                {
                    result.Append(this.Title + "<br/>");
                }

                result.Append("<small>");

                if (!string.IsNullOrEmpty(this.CameraMaker))
                {
                    result.Append(this.CameraMaker + " ");
                }

                if (!string.IsNullOrEmpty(this.CameraModel))
                {
                    result.Append(this.CameraModel + " ");
                }

                if (!string.IsNullOrEmpty(this.Lenses))
                {
                    result.Append(this.Lenses + " ");
                }

                result.Append("<br/>");

                if (!string.IsNullOrEmpty(this.Aperture))
                {
                    result.Append(this.Aperture + " ");
                }

                if (!string.IsNullOrEmpty(this.ShutterSpeed))
                {
                    result.Append(this.ShutterSpeed + " ");
                }

                if (this.FocusLen != null)
                {
                    result.Append(this.FocusLen + " ");
                }

                if (this.Iso != null)
                {
                    result.Append("ISO " + this.Iso + " ");
                }

                if (this.ExposureBiasStep != null)
                {
                    result.Append(this.ExposureBiasStep);
                }

                if (this.DateTaken != null)
                {
                    result.Append(
                        "<br/>" + this.DateTaken.Value.ToString("dd-MMM-yy", CultureInfo.CreateSpecificCulture("en-US")));
                }

                result.Append("</small>");

                return result.ToString();
            }
        }

        [JsonIgnore]
        [MaxLength(50)]
        public string Iso { get; set; }

        [JsonIgnore]
        [MaxLength(100)]
        public string Lenses { get; set; }

        public int LowHeight { get; set; }

        [Computed]
        public string LowImageSource
            =>
                VirtualPathUtility.ToAbsolute(
                    Constants.MainContentFolder + "\\" + this.AlbumId + "\\" + Constants.ImageFolderLow + "\\"
                    + this.FileName);

        public int LowWidth { get; set; }

        [Computed]
        public string MiddleImageSource
            =>
                VirtualPathUtility.ToAbsolute(
                    Constants.MainContentFolder + "\\" + this.AlbumId + "\\" + Constants.ImageFolderMiddle + "\\"
                    + this.FileName);

        public int MidHeight { get; set; }

        public int MidWidth { get; set; }

        [Computed]
        public string OriginalDownloadPath
            =>
                VirtualPathUtility.ToAbsolute(
                    Constants.TempContentFolder + "\\" + this.Id + "\\" + this.OriginalFileName);

        [MaxLength(100)]
        public string OriginalFileName { get; set; }

        [JsonIgnore]
        [MaxLength(50)]
        public string ShutterSpeed { get; set; }

        [MaxLength(200)]
        public string Title { get; set; }

        public int Width { get; set; }
    }
}