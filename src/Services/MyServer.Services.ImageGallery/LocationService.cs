namespace MyServer.Services.ImageGallery
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    //using Geocoding;
    //using Geocoding.Google;

    using MyServer.Data.Common;
    using MyServer.Data.Models;

    public class LocationService : ILocationService
    {
        //private IGeocoder geocoder;

        //private IRepository<ImageGpsData, Guid> gpsDbData;

        //public LocationService(IRepository<ImageGpsData, Guid> gpsDbData)
        //{
        //    this.geocoder = new GoogleGeocoder() { ApiKey = "AIzaSyAJOGz_xyAi_2CdRPW4HX-g5E1WcTwQMSY" };
        //    this.gpsDbData = gpsDbData;
        //}

        //public ImageGpsData GetGpsData(string location)
        //{
        //    var result = new ImageGpsData { Id = Guid.NewGuid(), CreatedOn = DateTime.Now };

        //    if (!string.IsNullOrEmpty(location))
        //    {
        //        result.LocationName = location;

        //        if (this.gpsDbData.All().Any(x => x.LocationName == location))
        //        {
        //            return this.gpsDbData.All().First();
        //        }

        //        try
        //        {
        //            IList<Address> addresses = this.geocoder.Geocode(location).ToList();

        //            if (addresses.Any())
        //            {
        //                result.Latitude = addresses.First().Coordinates.Latitude;
        //                result.Longitude = addresses.First().Coordinates.Longitude;
        //            }
        //        }
        //        catch (GoogleGeocodingException)
        //        {
        //            return null;
        //        }
        //    }

        //    return result;
        //}

        //public ImageGpsData GetGpsData(double latitude, double longitude)
        //{
        //    var result = new ImageGpsData { Id = Guid.NewGuid(), CreatedOn = DateTime.Now };

        //    if (this.gpsDbData.All().Any(x => x.Latitude == latitude && x.Longitude == longitude))
        //    {
        //        return this.gpsDbData.All().First();
        //    }

        //    result.Latitude = latitude;
        //    result.Longitude = longitude;

        //    try
        //    {
        //        IList<Address> addresses = this.geocoder.ReverseGeocode(latitude, longitude).ToList();
        //        IList<GoogleAddress> googleAddresses = addresses.Select(addr => addr as GoogleAddress).ToList();

        //        switch (googleAddresses.Count)
        //        {
        //            case 0:
        //                result.LocationName = string.Empty;
        //                break;
        //            case 1:
        //                result.LocationName = addresses[0].FormattedAddress;
        //                break;
        //            default:
        //                if (googleAddresses.Any(x => x.LocationType == GoogleLocationType.Rooftop))
        //                {
        //                    result.LocationName =
        //                        googleAddresses.First(x => x.LocationType == GoogleLocationType.Rooftop)
        //                            .FormattedAddress;
        //                }
        //                else if (googleAddresses.Any(x => x.LocationType == GoogleLocationType.Approximate))
        //                {
        //                    result.LocationName =
        //                        googleAddresses.First(x => x.LocationType == GoogleLocationType.Approximate)
        //                            .FormattedAddress;
        //                }
        //                else if (googleAddresses.Any(x => x.LocationType == GoogleLocationType.GeometricCenter))
        //                {
        //                    result.LocationName =
        //                        googleAddresses.First(x => x.LocationType == GoogleLocationType.GeometricCenter)
        //                            .FormattedAddress;
        //                }
        //                else if (googleAddresses.Any(x => x.LocationType == GoogleLocationType.GeometricCenter))
        //                {
        //                    result.LocationName =
        //                        googleAddresses.First(x => x.LocationType == GoogleLocationType.GeometricCenter)
        //                            .FormattedAddress;
        //                }

        //                break;
        //        }
        //    }
        //    catch (GoogleGeocodingException)
        //    {
        //        return null;
        //    }

        //    return result;
        //}
    }
}