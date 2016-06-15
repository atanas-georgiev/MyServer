namespace MyServer.Web.Main.Areas.ImageGalleryAdmin.Models.Album
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    using AutoMapper;

    using MyServer.Common.ImageGallery;
    using MyServer.Data.Models;
    using MyServer.Web.Infrastructure.Mappings;
    using MyServer.Web.Main.Areas.ImageGalleryAdmin.Models.Image;

    public class AlbumDetailsViewModel : IMapFrom<Album>, IHaveCustomMappings
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

        public List<ImageDetailsViewModel> Images { get; set; }

        public string FullLowFileFolder { get; set; }

        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<Album, AlbumDetailsViewModel>(string.Empty)
                .ForMember(
                    m => m.FullLowFileFolder,
                    opt =>
                    opt.MapFrom(c => Constants.MainContentFolder + "\\" + c.Id + "\\" + Constants.ImageFolderLow + "\\"));
        }
    }
}