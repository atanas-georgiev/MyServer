namespace MyServer.Web.Api.Areas.ImageGallery.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using MyServer.Common.ImageGallery;
    using MyServer.Data.Models;
    using MyServer.Web.Infrastructure.Mappings;

    public class AlbumAddBindingModel : IMapFrom<Album>
    {
        public AccessType AccessType { get; set; }

        [MaxLength(3000)]
        public string Description { get; set; }

        public Guid Id { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(200)]
        public string Title { get; set; }
    }
}