namespace MyServer.Web.Main.Areas.ImageGallery.Models.Album
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AutoMapper;

    using MyServer.Common.ImageGallery;
    using MyServer.Data.Models.ImageGallery;
    using MyServer.Web.Infrastructure.Mappings;
    using MyServer.Web.Main.Areas.ImageGallery.Models.Image;

    public class AlbumViewModel : IMapFrom<Album>, IHaveCustomMappings
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int ItemsCount { get; set; }

        public string FullLowFileFolder { get; set; }

        public string CoverImage { get; set; }

        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<Album, AlbumViewModel>(string.Empty)
                .ForMember(m => m.StartDate, opt => opt.MapFrom(c => c.Images.OrderBy(x => x.DateTaken).FirstOrDefault().DateTaken))
                .ForMember(m => m.EndDate, opt => opt.MapFrom(c => c.Images.OrderByDescending(x => x.DateTaken).FirstOrDefault().DateTaken))
                .ForMember(m => m.ItemsCount, opt => opt.MapFrom(c => c.Images.Count))
                .ForMember(m => m.CoverImage, opt => opt.MapFrom(c => c.Images.FirstOrDefault(i => i.Id == c.CoverId).FileName))
                .ForMember(m => m.FullLowFileFolder, opt => opt.MapFrom(c => Constants.MainContentFolder + "\\" + c.Id + "\\" + Constants.ImageFolderLow + "\\"));
        }
    }
}