namespace MyServer.Data.Models.ImageGallery
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using MyServer.Data.Common;
    using MyServer.Data.Common.Models;

    public class Image : BaseModel<Guid>, IHavePrimaryKey<Guid>
    {
        [ForeignKey("AlbumId")]
        public virtual Album Album { get; set; }

        public virtual Guid? AlbumId { get; set; }

        [MaxLength(50)]
        public string Aperture { get; set; }

        [MaxLength(50)]
        public string CameraMaker { get; set; }

        [MaxLength(50)]
        public string CameraModel { get; set; }

        public DateTime? DateTaken { get; set; }

        [MaxLength(3000)]
        public string Description { get; set; }

        public double? ExposureBiasStep { get; set; }

        [MaxLength(100)]
        public string FileName { get; set; }

        public double? FocusLen { get; set; }

        public int Height { get; set; }

        [ForeignKey("ImageGpsDataId")]
        public virtual ImageGpsData ImageGpsData { get; set; }

        public virtual Guid? ImageGpsDataId { get; set; }

        public int? Iso { get; set; }

        [MaxLength(50)]
        public string Lenses { get; set; }

        public int LowHeight { get; set; }

        public int LowWidth { get; set; }

        public int MidHeight { get; set; }

        public int MidWidth { get; set; }

        [MaxLength(100)]
        public string OriginalFileName { get; set; }

        [MaxLength(50)]
        public string ShutterSpeed { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(150)]
        public string Title { get; set; }

        public int Width { get; set; }
    }
}