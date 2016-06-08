namespace MyServer.Data.Models.ImageGallery
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using MyServer.Data.Common;
    using MyServer.Data.Common.Models;

    public class Album : BaseModel<Guid>, IHavePrimaryKey<Guid>
    {
        private ICollection<Image> images;

        public Album()
        {
            this.images = new HashSet<Image>();
        }

        public AccessType Access { get; set; }

        public Guid? CoverId { get; set; }

        [MaxLength(3000)]
        public string Description { get; set; }

        public virtual ICollection<Image> Images
        {
            get
            {
                return this.images;
            }

            set
            {
                this.images = value;
            }
        }

        [Required]
        [MinLength(3)]
        [MaxLength(150)]
        public string Title { get; set; }
    }
}