namespace MyServer.Web.Api.Models.ImageGallery
{
    using MyServer.Data.Models;
    using MyServer.Web.Infrastructure.Mappings;

    public class GpsDataModel : IMapFrom<ImageGpsData>
    {
        public double? Latitude { get; set; }

        public string LocationName { get; set; }

        public double? Longitude { get; set; }
    }
}