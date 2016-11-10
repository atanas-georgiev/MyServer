namespace MyServer.Web.Areas.ImageGallery.Models.Album
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

    public class SortFilterAlbumViewModel
    {
        [UIHint("EnumSort")]
        public MyServerSortType SortType { get; set; }
        
        public string SearchString { get; set; }

        public bool IsAscending { get; set; }
    }
}