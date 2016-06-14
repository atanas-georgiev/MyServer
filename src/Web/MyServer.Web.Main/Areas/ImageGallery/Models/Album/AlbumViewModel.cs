namespace MyServer.Web.Main.Areas.ImageGallery.Models.Album
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web;

    using DelegateDecompiler;

    using MyServer.Common.ImageGallery;
    using MyServer.Data.Models.ImageGallery;
    using MyServer.Web.Infrastructure.Mappings;
    using MyServer.Web.Main.Areas.ImageGallery.Models.Image;

    public class AlbumViewModel : IMapFrom<Album>
    {
        public Guid? CoverId { get; set; }

        [Computed]
        public string CoverImage
        {
            get
            {
                return this.CoverId == null
                           ? string.Empty
                           : VirtualPathUtility.ToAbsolute(
                               Constants.MainContentFolder + "\\" + this.Id + "\\" + Constants.ImageFolderLow + "\\"
                               + this.Images.FirstOrDefault(i => i.Id == this.CoverId.Value).FileName);
            }
        }

        [Computed]
        public string Date
        {
            get
            {
                var dates = this.Images.Where(x => x.DateTaken != null).Select(x => x.DateTaken).ToList();

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
                        return firstDate.ToString("dd MMM yyyy", CultureInfo.CreateSpecificCulture("en-US"));
                    }
                    else if (firstDate.Year == lastDate.Year && firstDate.Month == lastDate.Month)
                    {
                        return firstDate.Day + "-" + lastDate.Day + " " + firstDate.ToString("MMM yyyy", CultureInfo.CreateSpecificCulture("en-US"));
                    }
                    else if (firstDate.Year == lastDate.Year)
                    {
                        return firstDate.ToString("dd MMM", CultureInfo.CreateSpecificCulture("en-US")) + "-" +
                               lastDate.ToString("dd MMM", CultureInfo.CreateSpecificCulture("en-US")) + " " +
                               lastDate.ToString("yyyy", CultureInfo.CreateSpecificCulture("en-US"));
                    }
                    else
                    {
                        return firstDate.ToString("dd MMM yyyy", CultureInfo.CreateSpecificCulture("en-US")) + "-"
                               + lastDate.ToString("dd MMM yyyy", CultureInfo.CreateSpecificCulture("en-US"));
                    }
                }
            }
        }

        public Guid Id { get; set; }

        public virtual ICollection<ImageViewModel> Images { get; set; }

        [Computed]
        public string ImagesCountCover
        {
            get
            {
                switch (this.Images.Count)
                {
                    case 0:
                        return "No items";
                    case 1:
                        return "1 item";
                    default:
                        return this.Images.Count + " items";
                }
            }
        }

        public string Title { get; set; }
    }
}