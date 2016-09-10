namespace MyServer.Web.Main.Areas.Shared.Models.Album
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using MyServer.Common.ImageGallery;
    using MyServer.Data.Models;
    using Services.Mappings;
    using Image;
    using AutoMapper;
    using Helpers;
    using System.Linq;
    using AutoMapper.QueryableExtensions;

    public class AlbumViewModel : IMapFrom<Album>, IHaveCustomMappings
    {
        public Guid? CoverId { get; set; }

        public ImageViewModel CoverModel { get; set; }

        public string CoverImage { get; set; }

        public string Date { get; set; }

        [MaxLength(3000)]
        [UIHint("Editor")]
        public string Description { get; set; }

        public Guid Id { get; set; }

        //[Computed]
        //public IEnumerable<GpsDataViewModel> ImageCoordinates
        //{
        //    get
        //    {
        //        return this.Images?.Where(x => x.ImageGpsData != null).Select(x => x.ImageGpsData).Distinct();
        //    }
        //}

       // public ICollection<ImageViewModel> Images { get; set; }

        public string ImagesCountCover { get; set; }

        // public virtual ICollection<IdentityUserRole> Roles { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(200)]
        public string Title { get; set; }

        public bool IsPublic { get; set; }

        public virtual void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<Album, AlbumViewModel>()
                //  .ForMember(m => m.Date, opt => opt.ResolveUsing(c => MapDate(c)))
                .ForMember(m => m.ImagesCountCover, opt => opt.MapFrom(c => MapImagesCountCover(c)))
                .ForMember(m => m.CoverImage, opt => opt.MapFrom(c => MapCoverImage(c)))
                .ForMember(m => m.CoverModel, opt => opt.MapFrom(c => MapCoverModel(c)));
        }

        static ImageViewModel MapCoverModel(Album source)
        {
            if (source.Cover == null)
            {
                return null;
            }

            var list = new List<Image>();
            list.Add(source.Cover);
            return list.AsQueryable().To<ImageViewModel>().FirstOrDefault();// (AutoMapperConfig.Configuration, membersToExpand);
        }

        static string MapCoverImage(Album source)
        {
            if (source.Cover == null)
            {
                return string.Empty;
            }

            return PathHelper.WwwPath + Constants.MainContentFolder + "/" + source.Cover.AlbumId + "/" + Constants.ImageFolderLow + "/" + source.Cover.FileName;
        }

        static string MapImagesCountCover(Album source)
        {
            if (source.Images == null)
            {
                return "No items";
            }

            switch (source.Images.Count)
            {
                case 0:
                    return "No items";
                case 1:
                    return "1 item";
                default:
                    return source.Images.Count + " items";
            }
        }

        static string MapDate(Album source)
        {
            if (source.Images == null)
            {
                return string.Empty;
            }

            var dates = source.Images.Where(x => x.DateTaken != null).Select(x => x.DateTaken).ToList();

            if (dates.Count == 0)
            {
                return string.Empty;
            }
            else
            {
                var firstDate = dates.OrderBy(x => x.Value).Select(x => x.Value).First();
                var lastDate = dates.OrderBy(x => x.Value).Select(x => x.Value).Last();

                if (firstDate.Date == lastDate.Date)
                {
                    return firstDate.ToString("dd MMM yyyy");
                }
                else if (firstDate.Year == lastDate.Year && firstDate.Month == lastDate.Month)
                {
                    return firstDate.Day + "-" + lastDate.Day + " "
                           + firstDate.ToString("MMM yyyy");
                }
                else if (firstDate.Year == lastDate.Year)
                {
                    return firstDate.ToString("dd MMM") + "-"
                           + lastDate.ToString("dd MMM") + " "
                           + lastDate.ToString("yyyy");
                }
                else
                {
                    return firstDate.ToString("dd MMM yyyy") + "-"
                           + lastDate.ToString("dd MMM yyyy");
                }
            }
        }
    }
}