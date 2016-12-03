using MyServer.Data.Models;
using MyServer.Services.Mappings;

namespace MyServer.ViewComponents.ImageGallery.Components.ImageList.Models
{
    public class GpsDataViewModel : IMapFrom<ImageGpsData>
    {
        public double? Latitude { get; set; }

        public string LocationName { get; set; }

        public double? Longitude { get; set; }
    }
}
