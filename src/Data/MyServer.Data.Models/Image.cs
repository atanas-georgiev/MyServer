namespace MyServer.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using MyServer.Data.Common;
    using MyServer.Data.Common.Models;

    public class Image : BaseModel<Guid>, IHavePrimaryKey<Guid>
    {
        public Image()
        {
            this.Id = Guid.NewGuid();
            this.Comments = new List<Comment>();
            this.Covers = new List<Album>();
        }

        public Image(string id)
        {
            this.Id = Guid.Parse(id);
            this.Comments = new List<Comment>();
            this.Covers = new List<Album>();
        }

        public virtual User AddedBy { get; set; }

        public virtual string AddedById { get; set; }

        [InverseProperty("Images")]
        public virtual Album Album { get; set; }

        public virtual Guid? AlbumId { get; set; }

        [MaxLength(50)]
        public string Aperture { get; set; }

        [MaxLength(50)]
        public string CameraMaker { get; set; }

        [MaxLength(50)]
        public string CameraModel { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        [InverseProperty("Cover")]
        public ICollection<Album> Covers { get; set; }

        public DateTime? DateTaken { get; set; }

        [MaxLength(50)]
        public string ExposureBiasStep { get; set; }

        [Required]
        [MaxLength(200)]
        public string FileName { get; set; }

        [MaxLength(50)]
        public string FocusLen { get; set; }

        public int Height { get; set; }

        public virtual ImageGpsData ImageGpsData { get; set; }

        [MaxLength(50)]
        public string Iso { get; set; }

        public int LowHeight { get; set; }

        public int LowWidth { get; set; }

        public int MidHeight { get; set; }

        public int MidWidth { get; set; }

        [Required]
        [MaxLength(200)]
        public string OriginalFileName { get; set; }

        [MaxLength(50)]
        public string ShutterSpeed { get; set; }

        [MaxLength(200)]
        public string Title { get; set; }

        public int Width { get; set; }
    }
}