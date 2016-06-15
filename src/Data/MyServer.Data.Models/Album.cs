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
        public virtual User AddedBy { get; set; }

        public virtual string AddedById { get; set; }

        public Guid? CoverId { get; set; }

        [MaxLength(3000)]
        public string Description { get; set; }

        public virtual ICollection<Image> Images { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public virtual ICollection<IdentityUserRole> Roles { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(200)]
        public string Title { get; set; }
    }
}