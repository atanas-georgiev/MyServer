namespace MyServer.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using MyServer.Data.Common;
    using MyServer.Data.Common.Models;

    public class ImageGpsData : BaseModel<Guid>, IHavePrimaryKey<Guid>
    {
        [Required]
        public double? Latitude { get; set; }

        [Required]
        [MaxLength(200)]
        public string LocationName { get; set; }

        [Required]
        public double? Longitude { get; set; }
    }
}