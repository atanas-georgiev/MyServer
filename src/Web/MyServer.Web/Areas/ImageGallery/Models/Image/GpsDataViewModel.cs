namespace MyServer.Web.Areas.ImageGallery.Models.Image
{
    using MyServer.Data.Models;
    using MyServer.Services.Mappings;

    public class GpsDataViewModel : IMapFrom<ImageGpsData>
    {
        public double? Latitude { get; set; }

        public string LocationName { get; set; }

        public double? Longitude { get; set; }
    }
}