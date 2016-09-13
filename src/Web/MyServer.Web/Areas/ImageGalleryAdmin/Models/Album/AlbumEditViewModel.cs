namespace MyServer.Web.Main.Areas.ImageGalleryAdmin.Models.Album
{
    using AutoMapper;
    using Image;
    using Services.Mappings;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using MyServer.Data.Models;
    using System;

    public class AlbumEditViewModel : IMapFrom<Album>, IHaveCustomMappings
    {
        public Guid Id { get; set; }

        [MaxLength(3000)]
        public string Description { get; set; }

        public ICollection<ImageViewModel> Images { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(200)]
        public string Title { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
        }
    }
}