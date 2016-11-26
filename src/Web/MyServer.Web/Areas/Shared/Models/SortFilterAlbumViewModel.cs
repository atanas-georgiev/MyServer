namespace MyServer.Web.Areas.Shared.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using AutoMapper;

    using MyServer.Common;
    using MyServer.Data.Models;
    using MyServer.Services.Mappings;
    using MyServer.Web.Areas.ImageGallery.Models.Image;
    using MyServer.Web.Areas.Shared.Models;
    using Resources;

    public class SortFilterAlbumViewModel
    {
        [UIHint("EnumSort")]
        [Display(Name = "Sort", ResourceType = typeof(Helpers_SharedResource))]
        public MyServerSortType SortType { get; set; }

        [Display(Name = "Search", ResourceType = typeof(Helpers_SharedResource))]
        public string SearchString { get; set; }        
    }
}