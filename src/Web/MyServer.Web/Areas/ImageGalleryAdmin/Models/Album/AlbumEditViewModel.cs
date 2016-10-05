namespace MyServer.Web.Main.Areas.ImageGalleryAdmin.Models.Album
{
    using AutoMapper;
    using Image;
    using Services.Mappings;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using MyServer.Data.Models;
    using System;
    using Common;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class AlbumEditViewModel : IMapFrom<Album>
    {
        public Guid Id { get; set; }

        [MaxLength(3000)]
        public string Description { get; set; }

        public ICollection<ImageViewModel> Images { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(200)]
        public string Title { get; set; }

        public MyServerAccessType Access { get; set; }

        public SelectList AccessTypes { get; set; } = new SelectList(new List<string>() { MyServerAccessType.Public.ToString(), MyServerAccessType.Registrated.ToString(), MyServerAccessType.Private.ToString() });
    }
}