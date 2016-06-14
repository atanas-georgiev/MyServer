namespace MyServer.Web.Main.Areas.ImageGallery.Models.Album
{
    using System;
    using System.Collections.Generic;
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
        public string Date => "12 Mar 2015";

        public Guid Id { get; set; }

        public virtual ICollection<ImageViewModel> Images { get; set; }

        [Computed]
        public string ImagesCountCover => "1 image";

        public string Title { get; set; }
    }
}