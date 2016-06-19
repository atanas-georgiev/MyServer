 namespace MyServer.Web.Main.Areas.ImageGalleryAdmin.Models.Album
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    using MyServer.Data.Models;
    using MyServer.Web.Infrastructure.Mappings;

    public class AddAlbumViewModel : IMapFrom<Album>
    {
        [MaxLength(3000)]
        [UIHint("Editor")]
        [AllowHtml]
        public string Description { get; set; }

        [Key]
        [HiddenInput(DisplayValue = false)]
        public Guid Id { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(150)]
        [UIHint("String")]
        public string Title { get; set; }

        public bool IsPublic { get; set; }
    }
}