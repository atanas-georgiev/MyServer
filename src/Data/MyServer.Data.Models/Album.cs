namespace MyServer.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using MyServer.Common;
    using MyServer.Data.Common;
    using MyServer.Data.Common.Models;

    public class Album : BaseModel<Guid>, IHavePrimaryKey<Guid>
    {
        public Album()
        {
            this.Id = Guid.NewGuid();
            this.Images = new List<Image>();
        }

        public Album(string id)
        {
            this.Id = Guid.Parse(id);
            this.Images = new List<Image>();
        }

        public MyServerAccessType Access { get; set; }

        public virtual User AddedBy { get; set; }

        public virtual string AddedById { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        [InverseProperty("Covers")]
        public virtual Image Cover { get; set; }

        [ForeignKey("Cover")]
        public virtual Guid? CoverId { get; set; }

        [MaxLength(3000)]
        public string DescriptionBg { get; set; }

        [MaxLength(3000)]
        public string DescriptionEn { get; set; }

        [InverseProperty("Album")]
        public virtual ICollection<Image> Images { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(200)]
        public string TitleBg { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(200)]
        public string TitleEn { get; set; }
    }
}