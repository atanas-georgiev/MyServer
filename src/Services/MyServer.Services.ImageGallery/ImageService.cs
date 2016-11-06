namespace MyServer.Services.ImageGallery
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    using ImageMagick;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Memory;

    using MyServer.Common;
    using MyServer.Common.ImageGallery;
    using MyServer.Data.Common;
    using MyServer.Data.Models;

    public class ImageService : IImageService
    {
        private readonly IRepository<Album, Guid> albums;

        private readonly IHostingEnvironment appEnvironment;

        private readonly IFileService fileService;

        private readonly IRepository<Image, Guid> images;

        private readonly ILocationService locationService;

        private readonly IMemoryCache memoryCache;

        public ImageService(
            IRepository<Image, Guid> images,
            IRepository<Album, Guid> albums,
            ILocationService locationService,
            IFileService fileService,
            IHostingEnvironment appEnvironment,
            IMemoryCache memoryCache)
        {
            this.images = images;
            this.albums = albums;
            this.locationService = locationService;
            this.fileService = fileService;
            this.appEnvironment = appEnvironment;
            this.memoryCache = memoryCache;
        }

        public void Add(Guid albumId, string userId, Stream fileStream, string fileName)
        {
            if (string.IsNullOrEmpty(userId) || fileStream == null || string.IsNullOrEmpty(fileName))
            {
                return;
            }

            var image = new Image
                            {
                                Id = Guid.NewGuid(),
                                AlbumId = albumId,
                                OriginalFileName = Path.GetFileName(fileName),
                                Title = string.Empty,
                                AddedById = userId
                            };

            using (var imageMagick = new MagickImage(fileStream))
            {
                var orientation = this.ExtractExifData(image, imageMagick, fileName);

                if (orientation != null && orientation != 1)
                {
                    switch (orientation)
                    {
                        case 8:
                            this.Rotate(imageMagick, MyServerRotateType.Left);
                            break;
                        case 3:
                            this.Rotate(imageMagick, MyServerRotateType.Flip);
                            break;
                        case 6:
                            this.Rotate(imageMagick, MyServerRotateType.Right);
                            break;
                    }
                }

                // write original quality
                imageMagick.Write(this.fileService.GetImageFolder(albumId, ImageType.Original) + image.FileName);

                // write middle quality
                if (imageMagick.Height > Constants.ImageMiddleMaxSize
                    || imageMagick.Width > Constants.ImageMiddleMaxSize)
                {
                    imageMagick.Resize(Constants.ImageMiddleMaxSize, Constants.ImageMiddleMaxSize);
                    image.MidHeight = imageMagick.Height;
                    image.MidWidth = imageMagick.Width;
                }

                imageMagick.Write(this.fileService.GetImageFolder(albumId, ImageType.Medium) + image.FileName);

                // add low quality
                if (imageMagick.Height > Constants.ImageLowMaxSize || imageMagick.Width > Constants.ImageLowMaxSize)
                {
                    imageMagick.Resize(Constants.ImageLowMaxSize, Constants.ImageLowMaxSize);
                    image.LowHeight = imageMagick.Height;
                    image.LowWidth = imageMagick.Width;
                }

                imageMagick.Quality = 70;
                imageMagick.Interlace = Interlace.Plane;
                imageMagick.Write(this.fileService.GetImageFolder(albumId, ImageType.Low) + image.FileName);
            }

            // Store image in DB
            this.images.Add(image);

            // check if first image, if so, make album cover
            var album = this.albums.All().Where(x => x.Id == albumId).Include(x => x.Images).FirstOrDefault();
            if (album.Images.Count == 1)
            {
                album.CoverId = image.Id;
                this.albums.Update(album);
            }

            // GC.Collect();
            this.memoryCache.Remove(CacheKeys.AlbumsServiceCacheKey);
            this.memoryCache.Remove(CacheKeys.ImageServiceCacheKey);
            this.memoryCache.Remove(CacheKeys.FileServiceCacheKey);
        }

        public void AddGpsDataToImage(Guid imageId, ImageGpsData gpsData)
        {
            var image = this.images.GetById(imageId);
            if (image != null && gpsData?.Latitude != null && gpsData.Longitude.HasValue)
            {
                image.ImageGpsData = gpsData;
                this.Update(image);

                var lowFile = this.appEnvironment.WebRootPath + Constants.MainContentFolder + "/" + image.AlbumId + "/"
                              + Constants.ImageFolderLow + "/" + image.FileName;
                var middleFile = this.appEnvironment.WebRootPath + Constants.MainContentFolder + "/" + image.AlbumId
                                 + "/" + Constants.ImageFolderMiddle + "/" + image.FileName;
                var highFile = this.appEnvironment.WebRootPath + Constants.MainContentFolder + "/" + image.AlbumId + "/"
                               + Constants.ImageFolderOriginal + "/" + image.FileName;

                using (var imageMagick = new MagickImage(lowFile))
                {
                    var exif = imageMagick.GetExifProfile() ?? new ExifProfile();
                    exif.SetValue(ExifTag.GPSLatitude, ExifDoubleToGps(gpsData.Latitude.Value));
                    exif.SetValue(ExifTag.GPSLongitude, ExifDoubleToGps(gpsData.Longitude.Value));
                    imageMagick.AddProfile(exif, true);
                    imageMagick.Write(lowFile);
                }

                using (var imageMagick = new MagickImage(middleFile))
                {
                    var exif = imageMagick.GetExifProfile() ?? new ExifProfile();
                    exif.SetValue(ExifTag.GPSLatitude, ExifDoubleToGps(gpsData.Latitude.Value));
                    exif.SetValue(ExifTag.GPSLongitude, ExifDoubleToGps(gpsData.Longitude.Value));
                    imageMagick.AddProfile(exif, true);
                    imageMagick.Write(middleFile);
                }

                using (var imageMagick = new MagickImage(highFile))
                {
                    var exif = imageMagick.GetExifProfile() ?? new ExifProfile();
                    exif.SetValue(ExifTag.GPSLatitude, ExifDoubleToGps(gpsData.Latitude.Value));
                    exif.SetValue(ExifTag.GPSLongitude, ExifDoubleToGps(gpsData.Longitude.Value));
                    imageMagick.AddProfile(exif, true);
                    imageMagick.Write(highFile);
                }
            }

            this.memoryCache.Remove(CacheKeys.AlbumsServiceCacheKey);
            this.memoryCache.Remove(CacheKeys.ImageServiceCacheKey);
            this.memoryCache.Remove(CacheKeys.FileServiceCacheKey);
        }

        public IQueryable<Image> GetAllReqursive(bool cache = true)
        {
            var firstImageToBeExcludeGuid = Guid.Parse(Constants.NoCoverId);

            if (cache)
            {
                IQueryable<Image> result;

                if (!this.memoryCache.TryGetValue(CacheKeys.ImageServiceCacheKey, out result))
                {
                    // fetch the value from the source
                    result =
                        this.images.All()
                            .Where(x => x.IsDeleted == false && x.Id != firstImageToBeExcludeGuid)
                            .Include(x => x.Album)
                            .Include(x => x.Comments)
                            .Include(x => x.ImageGpsData)
                            .ToList()
                            .AsQueryable();

                    // store in the cache
                    this.memoryCache.Set(
                        CacheKeys.ImageServiceCacheKey,
                        result,
                        new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromDays(365)));
                }

                return result;
            }

            return
                this.images.All()
                    .Where(x => x.IsDeleted == false && x.Id != firstImageToBeExcludeGuid)
                    .Include(x => x.Album)
                    .Include(x => x.Comments)
                    .Include(x => x.ImageGpsData)
                    .ToList()
                    .AsQueryable();
        }

        public Image GetById(Guid id, bool cache = true)
        {
            return this.GetAllReqursive(cache).FirstOrDefault(x => x.Id == id);
        }

        public string GetRandomImagePath()
        {
            var rand = new Random();
            var skip = rand.Next(0, this.GetAllReqursive().Count(x => x.Album.Access == MyServerAccessType.Public));
            var randomImage =
                this.GetAllReqursive()
                    .Where(x => x.Album.Access == MyServerAccessType.Public)
                    .Skip(skip)
                    .FirstOrDefault();
            if (randomImage != null)
            {
                var randomImagePath = Constants.MainContentFolder + "/" + randomImage.AlbumId + "/"
                                      + Constants.ImageFolderMiddle + "/" + randomImage.FileName;
                return randomImagePath;
            }

            return null;
        }

        public void PrepareFileForDownload(Guid id)
        {
            var image = this.GetById(id);
            var filePathServer = this.appEnvironment.WebRootPath + Constants.MainContentFolder + "\\" + image.AlbumId
                                 + "\\" + Constants.ImageFolderOriginal + "\\" + image.FileName;
            var filePathTemp = this.appEnvironment.WebRootPath + Constants.TempContentFolder + "\\" + id + "\\"
                               + image.OriginalFileName;

            this.fileService.EmptyTempFolder();
            Directory.CreateDirectory(this.appEnvironment.WebRootPath + Constants.TempContentFolder + "\\" + id);

            File.Copy(filePathServer, filePathTemp);
        }

        public void Remove(Guid id)
        {
            var image = this.GetById(id, false);

            if (id == image.Album.CoverId)
            {
                var noCoverImageGuid = Guid.Parse(Constants.NoCoverId);
                var noCoverImage = this.images.All().FirstOrDefault(x => x.Id == noCoverImageGuid);
                image.Album.CoverId = noCoverImage.Id;
                this.albums.Update(image.Album);
            }

            this.images.Delete(id);
            if (image.AlbumId.HasValue)
            {
                this.fileService.RemoveImage(image.AlbumId.Value, image.FileName);
            }

            this.memoryCache.Remove(CacheKeys.AlbumsServiceCacheKey);
            this.memoryCache.Remove(CacheKeys.ImageServiceCacheKey);
            this.memoryCache.Remove(CacheKeys.FileServiceCacheKey);
        }

        public void Rotate(Guid imageId, MyServerRotateType rotateType)
        {
            var image = this.images.GetById(imageId);
            if (image != null)
            {
                var lowFile = this.appEnvironment.WebRootPath + Constants.MainContentFolder + "/" + image.AlbumId + "/"
                              + Constants.ImageFolderLow + "/" + image.FileName;
                var middleFile = this.appEnvironment.WebRootPath + Constants.MainContentFolder + "/" + image.AlbumId
                                 + "/" + Constants.ImageFolderMiddle + "/" + image.FileName;
                var highFile = this.appEnvironment.WebRootPath + Constants.MainContentFolder + "/" + image.AlbumId + "/"
                               + Constants.ImageFolderOriginal + "/" + image.FileName;

                using (var imageMagick = new MagickImage(lowFile))
                {
                    this.Rotate(imageMagick, rotateType);
                    imageMagick.Write(lowFile);
                }

                using (var imageMagick = new MagickImage(middleFile))
                {
                    this.Rotate(imageMagick, rotateType);
                    imageMagick.Write(middleFile);
                }

                using (var imageMagick = new MagickImage(highFile))
                {
                    this.Rotate(imageMagick, rotateType);
                    imageMagick.Write(highFile);
                }
            }

            this.memoryCache.Remove(CacheKeys.AlbumsServiceCacheKey);
            this.memoryCache.Remove(CacheKeys.ImageServiceCacheKey);
            this.memoryCache.Remove(CacheKeys.FileServiceCacheKey);
        }

        public void Update(Image image)
        {
            this.images.Update(image);

            this.memoryCache.Remove(CacheKeys.AlbumsServiceCacheKey);
            this.memoryCache.Remove(CacheKeys.ImageServiceCacheKey);
            this.memoryCache.Remove(CacheKeys.FileServiceCacheKey);
        }

        public void UpdateDateTaken(Guid imageId, DateTime date)
        {
            var image = this.images.GetById(imageId);
            if (image != null)
            {
                var lowFileOld = this.appEnvironment.WebRootPath + Constants.MainContentFolder + "/" + image.AlbumId
                                 + "/" + Constants.ImageFolderLow + "/" + image.FileName;
                var middleFileOld = this.appEnvironment.WebRootPath + Constants.MainContentFolder + "/" + image.AlbumId
                                    + "/" + Constants.ImageFolderMiddle + "/" + image.FileName;
                var highFileOld = this.appEnvironment.WebRootPath + Constants.MainContentFolder + "/" + image.AlbumId
                                  + "/" + Constants.ImageFolderOriginal + "/" + image.FileName;

                var oldFilename = image.FileName;
                image.DateTaken = date;
                image.FileName = date.ToString("yyyy-MM-dd-HH-mm-ss-") + Guid.NewGuid() + Path.GetExtension(oldFilename);
                this.Update(image);

                var format = "yyyy:MM:dd HH:mm:ss";

                var lowFile = this.appEnvironment.WebRootPath + Constants.MainContentFolder + "/" + image.AlbumId + "/"
                              + Constants.ImageFolderLow + "/" + image.FileName;
                var middleFile = this.appEnvironment.WebRootPath + Constants.MainContentFolder + "/" + image.AlbumId
                                 + "/" + Constants.ImageFolderMiddle + "/" + image.FileName;
                var highFile = this.appEnvironment.WebRootPath + Constants.MainContentFolder + "/" + image.AlbumId + "/"
                               + Constants.ImageFolderOriginal + "/" + image.FileName;

                using (var imageMagick = new MagickImage(lowFileOld))
                {
                    var exif = imageMagick.GetExifProfile() ?? new ExifProfile();
                    exif.SetValue(ExifTag.DateTimeOriginal, date.ToString(format));
                    imageMagick.AddProfile(exif, true);
                    imageMagick.Write(lowFile);
                }

                using (var imageMagick = new MagickImage(middleFileOld))
                {
                    var exif = imageMagick.GetExifProfile() ?? new ExifProfile();
                    exif.SetValue(ExifTag.DateTimeOriginal, date.ToString(format));
                    imageMagick.AddProfile(exif, true);
                    imageMagick.Write(middleFile);
                }

                using (var imageMagick = new MagickImage(highFileOld))
                {
                    var exif = imageMagick.GetExifProfile() ?? new ExifProfile();
                    exif.SetValue(ExifTag.DateTimeOriginal, date.ToString(format));
                    imageMagick.AddProfile(exif, true);
                    imageMagick.Write(highFile);
                }

                File.Delete(lowFileOld);
                File.Delete(middleFileOld);
                File.Delete(highFileOld);

                this.memoryCache.Remove(CacheKeys.AlbumsServiceCacheKey);
                this.memoryCache.Remove(CacheKeys.ImageServiceCacheKey);
                this.memoryCache.Remove(CacheKeys.FileServiceCacheKey);
            }
        }

        private static Rational[] ExifDoubleToGps(double propItem)
        {
            double temp;
            var result = new Rational[3];

            temp = Math.Abs(propItem);
            var degrees = Math.Truncate(temp);

            temp = (temp - degrees) * 60;
            var minutes = Math.Truncate(temp);

            temp = (temp - minutes) * 60;
            var seconds = Math.Truncate(temp);

            result[0] = new Rational(degrees);
            result[1] = new Rational(minutes);
            result[2] = new Rational(seconds);

            return result;
        }

        private static double ExifGpsToDouble(Rational[] propItem)
        {
            var degreesNumerator = propItem[0].Numerator;
            var degreesDenominator = propItem[0].Denominator;
            var degrees = degreesNumerator / (double)degreesDenominator;

            var minutesNumerator = propItem[1].Numerator;
            var minutesDenominator = propItem[1].Denominator;
            var minutes = minutesNumerator / (double)minutesDenominator;

            var secondsNumerator = propItem[2].Numerator;
            var secondsDenominator = propItem[2].Denominator;
            var seconds = secondsNumerator / (double)secondsDenominator;

            var coorditate = degrees + (minutes / 60f) + (seconds / 3600f);

            return coorditate;
        }

        private ushort? ExtractExifData(Image inputImage, MagickImage inputImageMagick, string originalFileName)
        {
            var exif = inputImageMagick.GetExifProfile();

            var dateTimeTaken = exif?.Values?.FirstOrDefault(x => x.Tag == ExifTag.DateTimeOriginal);
            if (dateTimeTaken != null)
            {
                var format = "yyyy:MM:dd HH:mm:ss";
                inputImage.DateTaken = DateTime.ParseExact(
                    dateTimeTaken.Value.ToString(),
                    format,
                    CultureInfo.InvariantCulture);
            }

            var gpdLong = exif?.Values?.FirstOrDefault(x => x.Tag == ExifTag.GPSLongitude);
            var gpdLat = exif?.Values?.FirstOrDefault(x => x.Tag == ExifTag.GPSLatitude);
            if (gpdLong != null && gpdLat != null)
            {
                inputImage.ImageGpsData =
                    this.locationService.GetGpsDataNormalized(
                        ExifGpsToDouble((Rational[])gpdLong.Value),
                        ExifGpsToDouble((Rational[])gpdLat.Value)).Result;
            }

            var make = exif?.Values?.FirstOrDefault(x => x.Tag == ExifTag.Make);
            if (make != null)
            {
                inputImage.CameraMaker = make.Value.ToString();
            }

            var model = exif?.Values?.FirstOrDefault(x => x.Tag == ExifTag.Model);
            if (model != null)
            {
                inputImage.CameraModel = model.Value.ToString();
            }

            var iso = exif?.Values?.FirstOrDefault(x => x.Tag == ExifTag.ISOSpeedRatings);
            if (iso != null)
            {
                inputImage.Iso = iso.Value.ToString();
            }

            var shutter = exif?.Values?.FirstOrDefault(x => x.Tag == ExifTag.ExposureTime);
            if (shutter != null)
            {
                inputImage.ShutterSpeed = shutter.Value.ToString();
            }

            var aperture = exif?.Values?.FirstOrDefault(x => x.Tag == ExifTag.ApertureValue);
            if (aperture != null)
            {
                Rational<uint> val = new Rational<uint>(
                                         ((Rational)aperture.Value).Numerator,
                                         ((Rational)aperture.Value).Denominator);
                double fstop = Math.Pow(2.0, Convert.ToDouble(val, CultureInfo.InvariantCulture) / 2.0);
                inputImage.Aperture = string.Format(new CultureInfo("en-US"), "f/{0:#0.0}", fstop);
            }

            var focuslen = exif?.Values?.FirstOrDefault(x => x.Tag == ExifTag.FocalLength);
            if (focuslen != null)
            {
                Rational<uint> val = new Rational<uint>(
                                         ((Rational)focuslen.Value).Numerator,
                                         ((Rational)focuslen.Value).Denominator);
                inputImage.FocusLen = string.Format(
                    new CultureInfo("en-US"),
                    "{0:#0.#}",
                    Convert.ToDecimal(val, CultureInfo.InvariantCulture));
            }

            var exposure = exif?.Values?.FirstOrDefault(x => x.Tag == ExifTag.ExposureBiasValue);
            if (exposure != null)
            {
                try
                {
                    var val = new Rational<int>(
                                  ((SignedRational)exposure.Value).Numerator,
                                  ((SignedRational)exposure.Value).Denominator);
                    inputImage.ExposureBiasStep = val.Numerator != 0 ? val.ToString(CultureInfo.InvariantCulture) : "0";
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            inputImage.Width = inputImageMagick.Width;
            inputImage.Height = inputImageMagick.Height;

            if (inputImage.DateTaken != null)
            {
                inputImage.FileName = inputImage.DateTaken.Value.ToString("yyyy-MM-dd-HH-mm-ss-") + Guid.NewGuid();
            }
            else
            {
                inputImage.FileName = Path.GetFileNameWithoutExtension(originalFileName) + Guid.NewGuid();
            }

            inputImage.FileName += Path.GetExtension(originalFileName);

            var orientation = exif?.Values?.FirstOrDefault(x => x.Tag == ExifTag.Orientation);

            if (orientation == null)
            {
                return null;
            }

            return (ushort)orientation.Value;
        }

        private void Rotate(MagickImage image, MyServerRotateType rotateType)
        {
            switch (rotateType)
            {
                case MyServerRotateType.Left:
                    image.Rotate(270);
                    break;
                case MyServerRotateType.Right:
                    image.Rotate(90);
                    break;
                case MyServerRotateType.Flip:
                    image.Rotate(180);
                    break;
            }

            image.Orientation = OrientationType.Undefined;
        }
    }
}