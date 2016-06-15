namespace MyServer.Web.Main.Areas.ImageGalleryAdmin.Models.Album
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using MyServer.Data.Models;
    using MyServer.Web.Infrastructure.Mappings;

    public class AlbumListViewModel : IMapFrom<Album>
    {
        [MaxLength(3000)]
        public string Description { get; set; }

        [Key]
        public Guid Id { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(150)]
        public string Title { get; set; }
    }
}