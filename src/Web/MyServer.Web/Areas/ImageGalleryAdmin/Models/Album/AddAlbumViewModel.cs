namespace MyServer.Web.Areas.ImageGalleryAdmin.Models.Album
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;

    using MyServer.Common;
    using MyServer.Data.Models;
    using MyServer.Services.Mappings;

    public class AddAlbumViewModel : IMapFrom<Album>
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
        public string DescriptionBg { get; set; }

        [MaxLength(3000)]
        public string DescriptionEn { get; set; }

        [Key]
        [HiddenInput(DisplayValue = false)]
        public Guid Id { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(150)]
        public string TitleBg { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(150)]
        public string TitleEn { get; set; }
    }
}