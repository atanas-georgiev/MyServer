﻿ namespace MyServer.Web.Areas.ImageGalleryAdmin.Models.Album
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using MyServer.Data.Models;
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using Services.Mappings;
    using Common;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class AddAlbumViewModel : IMapFrom<Album>
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

        public MyServerAccessType Access { get; set; }

        public SelectList AccessTypes { get; set; } = new SelectList(new List<string>() { MyServerAccessType.Public.ToString(), MyServerAccessType.Registrated.ToString(), MyServerAccessType.Private.ToString() });
    }
}