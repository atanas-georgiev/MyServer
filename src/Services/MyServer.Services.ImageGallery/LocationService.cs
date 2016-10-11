namespace MyServer.Services.ImageGallery
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    using MyServer.Data.Common;
    using MyServer.Data.Models;

    public class LocationService : ILocationService
    {
        private IRepository<ImageGpsData, Guid> gpsDbData;

        public LocationService(IRepository<ImageGpsData, Guid> gpsDbData)
        {
            this.gpsDbData = gpsDbData;
        }

        public async Task<ImageGpsData> GetGpsData(string location)
        {
            if (this.gpsDbData.All().Any(x => x.LocationName == location))
            {
                return this.gpsDbData.All().First();
            }

            var result = new ImageGpsData { Id = Guid.NewGuid(), CreatedOn = DateTime.Now };

            if (!string.IsNullOrEmpty(location))
            {
                var httpClient = new HttpClient();
                var result1 =
                    await
                        httpClient.GetStringAsync(
                            "https://maps.googleapis.com/maps/api/geocode/xml?address=" + location
                            + "&key=AIzaSyAJOGz_xyAi_2CdRPW4HX-g5E1WcTwQMSY");
                var xmlElm = XElement.Parse(result1);

                var status = (from elm in xmlElm.Descendants() where elm.Name == "status" select elm).FirstOrDefault();

                if (status.Value.ToLower() == "ok")
                {
                    var lat = (from elm in xmlElm.Descendants() where elm.Name == "lat" select elm).FirstOrDefault();

                    var lng = (from elm in xmlElm.Descendants() where elm.Name == "lng" select elm).FirstOrDefault();

                    if (lat != null && lng != null)
                    {
                        result.Latitude = double.Parse(lat.Value, CultureInfo.InvariantCulture);
                        result.Longitude = double.Parse(lng.Value, CultureInfo.InvariantCulture);
                        result.LocationName = location;
                        return result;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }

            return null;
        }

        public async Task<ImageGpsData> GetGpsData(double latitude, double longitude)
        {
            var result = new ImageGpsData { Id = Guid.NewGuid(), CreatedOn = DateTime.Now };

            var dbElement = this.gpsDbData.All().FirstOrDefault(x => x.Latitude == latitude && x.Longitude == longitude);
            if (dbElement != null)
            {
                return dbElement;
            }

            var httpClient = new HttpClient();
            var result1 =
                await
                    httpClient.GetStringAsync(
                        "https://maps.googleapis.com/maps/api/geocode/xml?latlng=" + longitude.ToString(CultureInfo.InvariantCulture) + ","
                        + latitude.ToString(CultureInfo.InvariantCulture) + "&key=AIzaSyAJOGz_xyAi_2CdRPW4HX-g5E1WcTwQMSY");
            var xmlElm = XElement.Parse(result1);

            var status = (from elm in xmlElm.Descendants() where elm.Name == "status" select elm).FirstOrDefault();

            if (status.Value.ToLower() == "ok")
            {
                var res =
                    (from elm in xmlElm.Descendants() where elm.Name == "formatted_address" select elm).FirstOrDefault();
                if (res != null)
                {
                    result.Latitude = latitude;
                    result.Longitude = longitude;
                    result.LocationName = res.Value;
                    return result;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}