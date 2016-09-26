﻿namespace MyServer.Services.ImageGallery
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Geocoding;
    using Geocoding.Google;

    using System.Net.Http;

    using MyServer.Data.Common;
    using MyServer.Data.Models;
    using System.Xml;
    using System.Xml.Linq;
    using System.Threading.Tasks;

    public class LocationService : ILocationService
    {
        private IGeocoder geocoder;

        private IRepository<ImageGpsData, Guid> gpsDbData;

        public LocationService(IRepository<ImageGpsData, Guid> gpsDbData)
        {
            this.geocoder = new GoogleGeocoder() { ApiKey = "AIzaSyAJOGz_xyAi_2CdRPW4HX-g5E1WcTwQMSY" };
            this.gpsDbData = gpsDbData;
        }

        public ImageGpsData GetGpsData(string location)
        {
            var result = new ImageGpsData { Id = Guid.NewGuid(), CreatedOn = DateTime.Now };

            if (!string.IsNullOrEmpty(location))
            {
                result.LocationName = location;

                if (this.gpsDbData.All().Any(x => x.LocationName == location))
                {
                    return this.gpsDbData.All().First();
                }

                try
                {
                    IList<Address> addresses = this.geocoder.Geocode(location).ToList();

                    if (addresses.Any())
                    {
                        result.Latitude = addresses.First().Coordinates.Latitude;
                        result.Longitude = addresses.First().Coordinates.Longitude;
                    }
                }
                catch (GoogleGeocodingException)
                {
                    return null;
                }
            }

            return result;
        }

        static string baseUri = "http://maps.googleapis.com/maps/api/geocode/xml?latlng={0},{1}&sensor=false";

        
    


        public async Task<ImageGpsData> GetGpsData(double latitude, double longitude)
        {
            var result = new ImageGpsData { Id = Guid.NewGuid(), CreatedOn = DateTime.Now };

            if (this.gpsDbData.All().Any(x => x.Latitude == latitude && x.Longitude == longitude))
            {
                return this.gpsDbData.All().First();
            }

            result.Latitude = latitude;
            result.Longitude = longitude;

            var httpClient = new HttpClient();
            var result1 = await httpClient.GetStringAsync("https://maps.googleapis.com/maps/api/geocode/xml?latlng=" + longitude.ToString() + "," + latitude.ToString() +"&key=AIzaSyAJOGz_xyAi_2CdRPW4HX-g5E1WcTwQMSY");
            var xmlElm = XElement.Parse(result1);

            var status = (from elm in xmlElm.Descendants()
                          where elm.Name == "status"
                          select elm).FirstOrDefault();
            if (status.Value.ToLower() == "ok")
            {
                var res = (from elm in xmlElm.Descendants()
                           where elm.Name == "formatted_address"
                           select elm).FirstOrDefault();
                if (res != null)
                {
                    result.LocationName = res.Value;
                }
            }
            else
            {
                //Console.WriteLine("No Address Found");
            }

            //try
            //{
            //    var addresses = this.geocoder.ReverseGeocode(latitude, longitude);
            //    var googleAddresses = addresses.Select(addr => addr as GoogleAddress);

            //switch (googleAddresses.Count)
            //{
            //    case 0:
            //        result.LocationName = string.Empty;
            //        break;
            //    case 1:
            //        result.LocationName = addresses[0].FormattedAddress;
            //        break;
            //    default:
            //        if (googleAddresses.Any(x => x.LocationType == GoogleLocationType.Rooftop))
            //        {
            //            result.LocationName =
            //                googleAddresses.First(x => x.LocationType == GoogleLocationType.Rooftop)
            //                    .FormattedAddress;
            //        }
            //        else if (googleAddresses.Any(x => x.LocationType == GoogleLocationType.Approximate))
            //        {
            //            result.LocationName =
            //                googleAddresses.First(x => x.LocationType == GoogleLocationType.Approximate)
            //                    .FormattedAddress;
            //        }
            //        else if (googleAddresses.Any(x => x.LocationType == GoogleLocationType.GeometricCenter))
            //        {
            //            result.LocationName =
            //                googleAddresses.First(x => x.LocationType == GoogleLocationType.GeometricCenter)
            //                    .FormattedAddress;
            //        }
            //        else if (googleAddresses.Any(x => x.LocationType == GoogleLocationType.GeometricCenter))
            //        {
            //            result.LocationName =
            //                googleAddresses.First(x => x.LocationType == GoogleLocationType.GeometricCenter)
            //                    .FormattedAddress;
            //        }

            //        break;
            //}
            //}
            //catch (GoogleGeocodingException)
            //{
            //    return null;
            //}

            return result;
        }
    }
}