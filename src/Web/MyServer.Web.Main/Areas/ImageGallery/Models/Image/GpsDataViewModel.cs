namespace MyServer.Web.Main.Areas.ImageGallery.Models.Image
{
    using MyServer.Data.Models;
    using MyServer.Web.Infrastructure.Mappings;

    public class GpsDataViewModel : IMapFrom<ImageGpsData>
    {
        public double? Latitude { get; set; }

        public string LocationName { get; set; }

        public double? Longitude { get; set; }
    }
}