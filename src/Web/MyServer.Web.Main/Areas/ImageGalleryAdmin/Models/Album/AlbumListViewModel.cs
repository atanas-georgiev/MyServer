namespace MyServer.Web.Main.Areas.ImageGalleryAdmin.Models.Album
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    using DelegateDecompiler;

    using MyServer.Common.ImageGallery;
    using MyServer.Data.Models;
    using MyServer.Web.Infrastructure.Mappings;
    using MyServer.Web.Main.Areas.ImageGallery.Models.Album;

    public class AlbumListViewModel : AlbumViewModel
    {
        [Computed]
        public int Height =>
            this.CoverId == null
                    ? Convert.ToInt32(Convert.ToDouble(Constants.ImageLowMaxSize / 2) / 1.5)
                    : this.Cover.LowHeight / 2;

        [Computed]
        public int Width => 
            this.CoverId == null
                    ? Constants.ImageLowMaxSize / 2
                    : this.Cover.LowWidth / 2;
    }
}