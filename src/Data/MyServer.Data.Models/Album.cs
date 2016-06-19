namespace MyServer.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Microsoft.AspNet.Identity.EntityFramework;

    using MyServer.Data.Common;
    using MyServer.Data.Common.Models;

    public class Album : BaseModel<Guid>, IHavePrimaryKey<Guid>
    {
        public Album()
        {
            this.Id = Guid.NewGuid();
        }

        public Album(string id)
        {
            this.Id = Guid.Parse(id);
        }

        public virtual User AddedBy { get; set; }

        public virtual string AddedById { get; set; }

        [InverseProperty("Covers")]
        public virtual Image Cover { get; set; }

        public virtual Guid? CoverId { get; set; }

        public bool IsPublic { get; set; }

        [MaxLength(3000)]
        public string Description { get; set; }

        [InverseProperty("Album")]
        public virtual ICollection<Image> Images { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(200)]
        public string Title { get; set; }
    }
}