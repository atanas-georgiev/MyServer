using MyServer.Common.ImageGallery;

namespace MyServer.Web.Areas.ImageGallery.Models.Image
{
    public class ImageListViewModel
    {
        public ImageListType Type { get; set; }

        public string Caption { get; set; }

        public object Data { get; set; }
    }
}
