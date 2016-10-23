namespace MyServer.Web.Areas.Shared.Models
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    using MyServer.Common;
    using MyServer.Common.ImageGallery;
    using MyServer.Data.Models;
    using MyServer.Web.Helpers;

    public static class MappingFunctions
    {
        public static string MapCoverImage(Album source)
        {
            return Constants.MainContentFolder + "/" + source.Cover.AlbumId + "/" + Constants.ImageFolderLow + "/"
                   + source.Cover.FileName;
        }

        public static string MapDate(Album source)
        {
            if (source.Images == null)
            {
                return string.Empty;
            }

            var dates = source.Images.Where(x => x.DateTaken != null).Select(x => x.DateTaken).ToList();

            if (dates.Count == 0)
            {
                return string.Empty;
            }
            else
            {
                var firstDate = dates.OrderBy(x => x.Value).Select(x => x.Value).First();
                var lastDate = dates.OrderBy(x => x.Value).Select(x => x.Value).Last();

                if (firstDate.Date == lastDate.Date)
                {
                    return firstDate.ToString("dd MMMM yyyy");
                }
                else if (firstDate.Year == lastDate.Year && firstDate.Month == lastDate.Month)
                {
                    return firstDate.Day + "-" + lastDate.Day + " " + firstDate.ToString("MMMM yyyy");
                }
                else if (firstDate.Year == lastDate.Year)
                {
                    return firstDate.ToString("dd MMMM") + "-" + lastDate.ToString("dd MMMM") + " "
                           + lastDate.ToString("yyyy");
                }
                else
                {
                    return firstDate.ToString("dd MMMM yyyy") + "-" + lastDate.ToString("dd MMMM yyyy");
                }
            }
        }

        public static string MapDescription(Album source)
        {
            var culture = CultureInfo.CurrentCulture.ToString();

            if (culture == "bg-BG")
            {
                return source.DescriptionBg;
            }
            else if (culture == "en-US")
            {
                return source.DescriptionEn;
            }

            return null;
        }

        public static string MapFbImage(Album source)
        {
            return Constants.MainContentFolder + "/" + source.Cover.AlbumId + "/" + Constants.ImageFolderMiddle + "/"
                   + source.Cover.FileName;
        }

        public static List<double> MapGpsCoordinates(Image source)
        {
            return source.ImageGpsData != null
                       ? new List<double>() { source.ImageGpsData.Latitude.Value, source.ImageGpsData.Longitude.Value }
                       : null;
        }

        public static string MapGpsName(Image source)
        {
            return source.ImageGpsData?.LocationName;
        }

        public static int MapHeight(Album source)
        {
            return source.CoverId == null
                       ? Convert.ToInt32(Convert.ToDouble(Constants.ImageLowMaxSize) / 1.5)
                       : source.Cover.LowHeight;
        }

        public static IEnumerable<ImageGpsData> MapImageCoordinates(Album source)
        {
            return source.Images?.Where(x => x.ImageGpsData != null).Select(x => x.ImageGpsData).Distinct();
        }

        public static string MapImagesCountCover(Album source)
        {
            switch (source.Images.Count)
            {
                case 0:
                    return Startup.SharedLocalizer["NoItems"];
                case 1:
                    return "1 " + Startup.SharedLocalizer["Item"];
                default:
                    return source.Images.Count + " " + Startup.SharedLocalizer["Items"];
            }
        }

        public static string MapInfo(Image source)
        {
            var result = new StringBuilder();

            if (!string.IsNullOrEmpty(source.Title))
            {
                result.Append(source.Title + "<br/>");
            }

            result.Append("<small>");

            if (!string.IsNullOrEmpty(source.CameraMaker))
            {
                result.Append(source.CameraMaker + " ");
            }

            if (!string.IsNullOrEmpty(source.CameraModel))
            {
                result.Append(source.CameraModel + " ");
            }

            if (!string.IsNullOrEmpty(source.Aperture))
            {
                result.Append(source.Aperture + " ");
            }

            if (!string.IsNullOrEmpty(source.ShutterSpeed))
            {
                result.Append(source.ShutterSpeed + " sec ");
            }

            if (source.FocusLen != null)
            {
                result.Append(source.FocusLen + " mm ");
            }

            if (source.Iso != null)
            {
                result.Append(source.Iso + " ISO");
            }

            if (source.DateTaken != null)
            {
                result.Append("<br/>" + source.DateTaken.Value.ToString("dd-MMMM-yy"));
            }

            result.Append("</small>");

            return result.ToString();
        }

        public static string MapTitle(Album source)
        {
            var culture = CultureInfo.CurrentCulture.ToString();

            if (culture == "bg-BG")
            {
                return source.TitleBg;
            }
            else if (culture == "en-US")
            {
                return source.TitleEn;
            }

            return null;
        }

        public static MyServerRoles MapUserRole(User user)
        {
            var role = PathHelper.UserManager.GetRolesAsync(user).Result;
            return (MyServerRoles)Enum.Parse(typeof(MyServerRoles), role.First(), true);
        }

        public static int MapWidth(Album source)
        {
            return source.CoverId == null ? Constants.ImageLowMaxSize : source.Cover.LowWidth;
        }
    }
}