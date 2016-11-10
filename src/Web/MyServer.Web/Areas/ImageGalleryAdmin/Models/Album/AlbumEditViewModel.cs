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
    using MyServer.Web.Areas.ImageGalleryAdmin.Models.Image;
    using MyServer.Web.Resources;

    public class AlbumEditViewModel : IMapFrom<Album>
    {
        [Display(Name = "Access", ResourceType = typeof(Helpers_SharedResource))]
        [UIHint("EnumAccess")]
        public MyServerAccessType Access { get; set; }

        [StringLength(3000, ErrorMessageResourceName = "ErrorLength",
             ErrorMessageResourceType = typeof(Helpers_SharedResource), MinimumLength = 0)]
        [Display(Name = "DescriptionBg", ResourceType = typeof(Helpers_SharedResource))]
        public string DescriptionBg { get; set; }

        [StringLength(3000, ErrorMessageResourceName = "ErrorLength",
             ErrorMessageResourceType = typeof(Helpers_SharedResource), MinimumLength = 0)]
        [Display(Name = "DescriptionEn", ResourceType = typeof(Helpers_SharedResource))]
        public string DescriptionEn { get; set; }

        [Key]
        [HiddenInput(DisplayValue = false)]
        public Guid Id { get; set; }

        public ICollection<ImageViewModel> Images { get; set; }

        [Required(ErrorMessageResourceName = "ErrorRequired", ErrorMessageResourceType = typeof(Helpers_SharedResource))
        ]
        [StringLength(150, ErrorMessageResourceName = "ErrorLength",
             ErrorMessageResourceType = typeof(Helpers_SharedResource), MinimumLength = 3)]
        [Display(Name = "TitleBg", ResourceType = typeof(Helpers_SharedResource))]
        public string TitleBg { get; set; }

        [Required(ErrorMessageResourceName = "ErrorRequired", ErrorMessageResourceType = typeof(Helpers_SharedResource))
        ]
        [StringLength(150, ErrorMessageResourceName = "ErrorLength",
             ErrorMessageResourceType = typeof(Helpers_SharedResource), MinimumLength = 3)]
        [Display(Name = "TitleEn", ResourceType = typeof(Helpers_SharedResource))]
        public string TitleEn { get; set; }
    }
}