namespace MyServer.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using MyServer.Data.Common;
    using MyServer.Data.Common.Models;

    public class Comment : BaseModel<Guid>, IHavePrimaryKey<Guid>
    {
        public Comment()
        {
            this.Id = Guid.NewGuid();
        }

        [Required]
        [MinLength(10)]
        [MaxLength(1000)]
        public string Data { get; set; }

        [ForeignKey("UserId")]
        [Required]
        public virtual User User { get; set; }

        [Required]
        public virtual string UserId { get; set; }
    }
}