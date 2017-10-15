namespace MyServer.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using MyServer.Data.Common;
    using MyServer.Data.Common.Models;

    public class StaticContent : BaseModel<Guid>, IHavePrimaryKey<Guid>
    {
        [Required]
        [MaxLength(50)]
        public string ContentKey { get; set; }

        [Required]
        [MaxLength(30000)]
        public string ContentValueBg { get; set; }

        [Required]
        [MaxLength(30000)]
        public string ContentValueEn { get; set; }
    }
}
