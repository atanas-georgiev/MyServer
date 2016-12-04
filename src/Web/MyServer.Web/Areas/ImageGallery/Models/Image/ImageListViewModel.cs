using MyServer.Common;
using MyServer.Web.Resources;
using System.ComponentModel.DataAnnotations;

namespace MyServer.Web.Areas.ImageGallery.Models.Image
{
    public class ImageListViewModel
    {
        [UIHint("EnumImageList")]
        [Display(Name = "Group", ResourceType = typeof(Helpers_SharedResource))]
        public ImageListType Type { get; set; }

        public string Caption { get; set; }

        public object Data { get; set; }
    }
}
