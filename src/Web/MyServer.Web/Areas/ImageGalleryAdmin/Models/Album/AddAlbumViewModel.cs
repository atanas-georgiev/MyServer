 namespace MyServer.Web.Areas.ImageGalleryAdmin.Models.Album
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using MyServer.Data.Models;
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using Services.Mappings;

    public class AddAlbumViewModel : IMapFrom<Album>, IHaveCustomMappings
    {
        [MaxLength(3000)]
        public string Description { get; set; }

        [Key]
        [HiddenInput(DisplayValue = false)]
        public Guid Id { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(150)]
        public string Title { get; set; }

        public bool IsPublic { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<Album, AddAlbumViewModel>()                
                .ForMember(m => m.Title, opt => opt.MapFrom(c => "Titleeee"));
        }
    }
}