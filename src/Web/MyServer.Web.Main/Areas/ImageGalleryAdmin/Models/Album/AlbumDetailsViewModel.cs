namespace MyServer.Web.Main.Areas.ImageGalleryAdmin.Models.Album
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using AutoMapper;

    using MyServer.Data.Models.ImageGallery;
    using MyServer.Web.Infrastructure.Mappings;
    using MyServer.Web.Main.Areas.ImageGalleryAdmin.Models.Image;

    public class AlbumDetailsViewModel : IMapFrom<Album>, IHaveCustomMappings
    {
        [Required]
        public DateTime Date { get; set; }

        [MaxLength(3000)]
        public string Description { get; set; }

        [Key]
        public Guid Id { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(150)]
        public string Title { get; set; }

        public List<ImageDetailsViewModel> Images { get; set; }

        public void CreateMappings(IConfiguration configuration)
        {
        }
    }
}