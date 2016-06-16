namespace MyServer.Web.Main.Areas.ImageGalleryAdmin.Models.Album
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    using DelegateDecompiler;

    using MyServer.Data.Models;
    using MyServer.Web.Infrastructure.Mappings;
    using MyServer.Web.Main.Areas.ImageGallery.Models.Album;

    public class AlbumListViewModel : AlbumViewModel
    {
        [Computed]
        public int Height =>
            this.CoverId == null
                    ? 0
                    : this.Images.First(x => x.Id == this.CoverId).LowHeight;

        [Computed]
        public int Width =>
            this.CoverId == null
                    ? 0
                    : this.Images.First(x => x.Id == this.CoverId).LowWidth;
    }
}