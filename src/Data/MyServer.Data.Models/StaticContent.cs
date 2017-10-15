using MyServer.Data.Common;
using MyServer.Data.Common.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace MyServer.Data.Models
{
    public class StaticContent : BaseModel<Guid>, IHavePrimaryKey<Guid>
    {
        [Required]
        [MaxLength(50)]
        public string ContentKey { get; set; }

        [Required]
        [MaxLength(30000)]
        public string ContentValue { get; set; }
    }
}
