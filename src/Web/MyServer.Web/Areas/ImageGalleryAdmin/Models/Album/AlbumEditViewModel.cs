namespace MyServer.Web.Areas.ImageGalleryAdmin.Models.Album
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Mvc.Rendering;

    using MyServer.Common;
    using MyServer.Data.Models;
    using MyServer.Services.Mappings;
    using MyServer.Web.Areas.ImageGalleryAdmin.Models.Image;

    public class AlbumEditViewModel : IMapFrom<Album>
    {
        public MyServerAccessType Access { get; set; }

        public SelectList AccessTypes { get; set; } =
            new SelectList(
                new List<string>()
                    {
                        MyServerAccessType.Public.ToString(),
                        MyServerAccessType.Registrated.ToString(),
                        MyServerAccessType.Private.ToString()
                    });

        [MaxLength(3000)]
        public string Description { get; set; }

        public Guid Id { get; set; }

        public ICollection<ImageViewModel> Images { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(200)]
        public string Title { get; set; }
    }
}