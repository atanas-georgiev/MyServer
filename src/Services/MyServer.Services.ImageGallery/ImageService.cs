namespace MyServer.Services.ImageGallery
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    using ImageProcessorCore;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;

    using MyServer.Common;
    using MyServer.Common.ImageGallery;
    using MyServer.Data.Common;
    using MyServer.Data.Models;

    using Image = MyServer.Data.Models.Image;

    public class ImageService : IImageService
    {
        private readonly IRepository<Album, Guid> albums;

        private readonly IHostingEnvironment appEnvironment;

        private readonly IFileService fileService;

        private readonly IRepository<Image, Guid> images;

        private readonly ILocationService locationService;

        public ImageService(
            IRepository<Image, Guid> images,
            IRepository<Album, Guid> albums,
            ILocationService locationService,
            IFileService fileService,
            IHostingEnvironment appEnvironment)
        {
            this.images = images;
            this.albums = albums;
            this.locationService = locationService;
            this.fileService = fileService;
            this.appEnvironment = appEnvironment;
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

            // Add exif data
            var orientation = this.ExtractExifData(image, fileStream, fileName);

            // save files
            this.fileService.Save(fileStream, ImageType.Original, image.FileName, albumId);

            var midStream = new MemoryStream();
            var midImage = this.Resize(fileStream, ImageType.Medium).Save(midStream);
            this.fileService.Save(midStream, ImageType.Medium, image.FileName, albumId);

            var lowStream = new MemoryStream();
            var lowImage = this.Resize(fileStream, ImageType.Low).Save(lowStream);
            this.fileService.Save(lowStream, ImageType.Low, image.FileName, albumId);

            image.LowHeight = lowImage.Height;
            image.LowWidth = lowImage.Width;
            image.MidHeight = midImage.Height;
            image.MidWidth = midImage.Width;

            // Store image in DB
            this.images.Add(image);

            // check if first image, if so, make album cover
            var album = this.albums.All().Where(x => x.Id == albumId).Include(x => x.Images).FirstOrDefault();
            if (album.Images.Count == 1)
            {
                album.CoverId = image.Id;
                this.albums.Update(album);
            }

            if (orientation != null && orientation != 1)
            {
                switch (orientation)
                {
                    case 8:
                        this.Rotate(image.Id, MyServerRotateType.Left);
                        break;
                    case 3:
                        this.Rotate(image.Id, MyServerRotateType.Flip);
                        break;
                    case 6:
                        this.Rotate(image.Id, MyServerRotateType.Right);
                        break;
                }
            }

            GC.Collect();
        }

        public void AddGpsDataToImage(Guid id, ImageGpsData gpsData)
        {
            var image = this.images.GetById(id);

            if (image != null && gpsData?.Latitude != null && gpsData.Longitude.HasValue)
            {
                image.ImageGpsData = gpsData;
                this.Update(image);

                var lowFileFolder = this.appEnvironment.WebRootPath + Constants.MainContentFolder + "/" + image.AlbumId
                                    + "/" + Constants.ImageFolderLow + "/";
                var middleFileFolder = this.appEnvironment.WebRootPath + Constants.MainContentFolder + "/"
                                       + image.AlbumId + "/" + Constants.ImageFolderMiddle + "/";
                var highFileFolder = this.appEnvironment.WebRootPath + Constants.MainContentFolder + "/" + image.AlbumId
                                     + "/" + Constants.ImageFolderOriginal + "/";

                File.Move(lowFileFolder + image.FileName, lowFileFolder + "_" + image.FileName);
                File.Move(middleFileFolder + image.FileName, middleFileFolder + "_" + image.FileName);
                File.Move(highFileFolder + image.FileName, highFileFolder + "_" + image.FileName);

                var lowStream = new MemoryStream(File.ReadAllBytes(lowFileFolder + "_" + image.FileName));
                var middleStream = new MemoryStream(File.ReadAllBytes(middleFileFolder + "_" + image.FileName));
                var highStream = new MemoryStream(File.ReadAllBytes(highFileFolder + "_" + image.FileName));

                var imageCoreLow = new ImageProcessorCore.Image(lowStream);
                var imageCoreMiddle = new ImageProcessorCore.Image(middleStream);
                var imageCoreHigh = new ImageProcessorCore.Image(highStream);

                imageCoreLow.ExifProfile.SetValue(ExifTag.GPSLatitude, ExifDoubleToGps(gpsData.Latitude.Value));
                imageCoreLow.ExifProfile.SetValue(ExifTag.GPSLongitude, ExifDoubleToGps(gpsData.Longitude.Value));
                imageCoreMiddle.ExifProfile.SetValue(ExifTag.GPSLatitude, ExifDoubleToGps(gpsData.Latitude.Value));
                imageCoreMiddle.ExifProfile.SetValue(ExifTag.GPSLongitude, ExifDoubleToGps(gpsData.Longitude.Value));
                imageCoreHigh.ExifProfile.SetValue(ExifTag.GPSLatitude, ExifDoubleToGps(gpsData.Latitude.Value));
                imageCoreHigh.ExifProfile.SetValue(ExifTag.GPSLongitude, ExifDoubleToGps(gpsData.Longitude.Value));

                var imageStreamLowModified = new MemoryStream();
                var imageStreamMiddleModified = new MemoryStream();
                var imageStreamHighModified = new MemoryStream();

                imageCoreLow.Save(imageStreamLowModified);
                imageCoreMiddle.Save(imageStreamMiddleModified);
                imageCoreHigh.Save(imageStreamHighModified);

                using (var fileStream = File.Create(lowFileFolder + image.FileName))
                {
                    imageStreamLowModified.Seek(0, SeekOrigin.Begin);
                    imageStreamLowModified.CopyTo(fileStream);
                }

                using (var fileStream = File.Create(middleFileFolder + image.FileName))
                {
                    imageStreamMiddleModified.Seek(0, SeekOrigin.Begin);
                    imageStreamMiddleModified.CopyTo(fileStream);
                }

                using (var fileStream = File.Create(highFileFolder + image.FileName))
                {
                    imageStreamHighModified.Seek(0, SeekOrigin.Begin);
                    imageStreamHighModified.CopyTo(fileStream);
                }

                File.Delete(lowFileFolder + "_" + image.FileName);
                File.Delete(middleFileFolder + "_" + image.FileName);
                File.Delete(highFileFolder + "_" + image.FileName);
            }
        }

        public IQueryable<Image> GetAll()
        {
            var firstImageToBeExcludeGuid = Guid.Parse(Constants.NoCoverId);
            var data = this.images.All().Where(x => x.IsDeleted == false && x.Id != firstImageToBeExcludeGuid);
            return data;
        }

        public IQueryable<Image> GetAllReqursive()
        {
            var firstImageToBeExcludeGuid = Guid.Parse(Constants.NoCoverId);
            var data =
                this.images.All()
                    .Where(x => x.IsDeleted == false && x.Id != firstImageToBeExcludeGuid)
                    .Include(x => x.Album)
                    .Include(x => x.Comments)
                    .Include(x => x.ImageGpsData);
            return data;
        }

        public Image GetById(Guid id)
        {
            return this.GetAllReqursive().FirstOrDefault(x => x.Id == id);
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
            var image = this.GetById(id);

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
        }

        public void Rotate(Guid imageId, MyServerRotateType rotateType)
        {
            var image = this.images.GetById(imageId);
            if (image != null)
            {
                var lowFileFolder = this.appEnvironment.WebRootPath + Constants.MainContentFolder + "/" + image.AlbumId
                                    + "/" + Constants.ImageFolderLow + "/";
                var middleFileFolder = this.appEnvironment.WebRootPath + Constants.MainContentFolder + "/"
                                       + image.AlbumId + "/" + Constants.ImageFolderMiddle + "/";
                var highFileFolder = this.appEnvironment.WebRootPath + Constants.MainContentFolder + "/" + image.AlbumId
                                     + "/" + Constants.ImageFolderOriginal + "/";

                File.Move(lowFileFolder + image.FileName, lowFileFolder + "_" + image.FileName);
                File.Move(middleFileFolder + image.FileName, middleFileFolder + "_" + image.FileName);
                File.Move(highFileFolder + image.FileName, highFileFolder + "_" + image.FileName);

                var lowStream = new MemoryStream(File.ReadAllBytes(lowFileFolder + "_" + image.FileName));
                var middleStream = new MemoryStream(File.ReadAllBytes(middleFileFolder + "_" + image.FileName));
                var highStream = new MemoryStream(File.ReadAllBytes(highFileFolder + "_" + image.FileName));

                var imageCoreLow = new ImageProcessorCore.Image(lowStream);
                var imageCoreMiddle = new ImageProcessorCore.Image(middleStream);
                var imageCoreHigh = new ImageProcessorCore.Image(highStream);

                var orientation = imageCoreLow.ExifProfile.Values.FirstOrDefault(x => x.Tag == ExifTag.Orientation);

                var imageStreamLowRotated = new MemoryStream();
                var imageStreamMiddleRotated = new MemoryStream();
                var imageStreamHighRotated = new MemoryStream();

                Image<Color, uint> imageCoreLowRotated = new Image<Color, uint>();
                Image<Color, uint> imageCoreMiddleRotated = new Image<Color, uint>();
                Image<Color, uint> imageCoreHighRotated = new Image<Color, uint>();

                switch (rotateType)
                {
                    case MyServerRotateType.Left:
                        imageCoreLowRotated = imageCoreLow.Rotate(RotateType.Rotate270);
                        imageCoreMiddleRotated = imageCoreMiddle.Rotate(RotateType.Rotate270);
                        imageCoreHighRotated = imageCoreHigh.Rotate(RotateType.Rotate270);
                        if (orientation != null)
                        {
                            var orientationInt = int.Parse(orientation.Value.ToString());

                            switch (orientationInt)
                            {
                                case 1:
                                    imageCoreLowRotated.ExifProfile.SetValue(ExifTag.Orientation, (ushort)6);
                                    imageCoreMiddleRotated.ExifProfile.SetValue(ExifTag.Orientation, (ushort)6);
                                    imageCoreHighRotated.ExifProfile.SetValue(ExifTag.Orientation, (ushort)6);
                                    break;
                                case 8:
                                    imageCoreLowRotated.ExifProfile.SetValue(ExifTag.Orientation, (ushort)1);
                                    imageCoreMiddleRotated.ExifProfile.SetValue(ExifTag.Orientation, (ushort)1);
                                    imageCoreHighRotated.ExifProfile.SetValue(ExifTag.Orientation, (ushort)1);
                                    break;
                                case 3:
                                    imageCoreLowRotated.ExifProfile.SetValue(ExifTag.Orientation, (ushort)8);
                                    imageCoreMiddleRotated.ExifProfile.SetValue(ExifTag.Orientation, (ushort)8);
                                    imageCoreHighRotated.ExifProfile.SetValue(ExifTag.Orientation, (ushort)8);
                                    break;
                                case 6:
                                    imageCoreLowRotated.ExifProfile.SetValue(ExifTag.Orientation, (ushort)3);
                                    imageCoreMiddleRotated.ExifProfile.SetValue(ExifTag.Orientation, (ushort)3);
                                    imageCoreHighRotated.ExifProfile.SetValue(ExifTag.Orientation, (ushort)3);
                                    break;
                            }
                        }

                        break;
                    case MyServerRotateType.Right:
                        imageCoreLowRotated = imageCoreLow.Rotate(RotateType.Rotate90);
                        imageCoreMiddleRotated = imageCoreMiddle.Rotate(RotateType.Rotate90);
                        imageCoreHighRotated = imageCoreHigh.Rotate(RotateType.Rotate90);

                        if (orientation != null)
                        {
                            var orientationInt = int.Parse(orientation.Value.ToString());

                            switch (orientationInt)
                            {
                                case 1:
                                    imageCoreLowRotated.ExifProfile.SetValue(ExifTag.Orientation, (ushort)8);
                                    imageCoreMiddleRotated.ExifProfile.SetValue(ExifTag.Orientation, (ushort)8);
                                    imageCoreHighRotated.ExifProfile.SetValue(ExifTag.Orientation, (ushort)8);
                                    break;
                                case 8:
                                    imageCoreLowRotated.ExifProfile.SetValue(ExifTag.Orientation, (ushort)3);
                                    imageCoreMiddleRotated.ExifProfile.SetValue(ExifTag.Orientation, (ushort)3);
                                    imageCoreHighRotated.ExifProfile.SetValue(ExifTag.Orientation, (ushort)3);
                                    break;
                                case 3:
                                    imageCoreLowRotated.ExifProfile.SetValue(ExifTag.Orientation, (ushort)6);
                                    imageCoreMiddleRotated.ExifProfile.SetValue(ExifTag.Orientation, (ushort)6);
                                    imageCoreHighRotated.ExifProfile.SetValue(ExifTag.Orientation, (ushort)6);
                                    break;
                                case 6:
                                    imageCoreLowRotated.ExifProfile.SetValue(ExifTag.Orientation, (ushort)1);
                                    imageCoreMiddleRotated.ExifProfile.SetValue(ExifTag.Orientation, (ushort)1);
                                    imageCoreHighRotated.ExifProfile.SetValue(ExifTag.Orientation, (ushort)1);
                                    break;
                            }
                        }

                        break;
                    case MyServerRotateType.Flip:
                        imageCoreLowRotated = imageCoreLow.Rotate(RotateType.Rotate180);
                        imageCoreMiddleRotated = imageCoreMiddle.Rotate(RotateType.Rotate180);
                        imageCoreHighRotated = imageCoreHigh.Rotate(RotateType.Rotate180);

                        if (orientation != null)
                        {
                            var orientationInt = int.Parse(orientation.Value.ToString());

                            switch (orientationInt)
                            {
                                case 1:
                                    imageCoreLowRotated.ExifProfile.SetValue(ExifTag.Orientation, (ushort)3);
                                    imageCoreMiddleRotated.ExifProfile.SetValue(ExifTag.Orientation, (ushort)3);
                                    imageCoreHighRotated.ExifProfile.SetValue(ExifTag.Orientation, (ushort)3);
                                    break;
                                case 8:
                                    imageCoreLowRotated.ExifProfile.SetValue(ExifTag.Orientation, (ushort)6);
                                    imageCoreMiddleRotated.ExifProfile.SetValue(ExifTag.Orientation, (ushort)6);
                                    imageCoreHighRotated.ExifProfile.SetValue(ExifTag.Orientation, (ushort)6);
                                    break;
                                case 3:
                                    imageCoreLowRotated.ExifProfile.SetValue(ExifTag.Orientation, (ushort)1);
                                    imageCoreMiddleRotated.ExifProfile.SetValue(ExifTag.Orientation, (ushort)1);
                                    imageCoreHighRotated.ExifProfile.SetValue(ExifTag.Orientation, (ushort)1);
                                    break;
                                case 6:
                                    imageCoreLowRotated.ExifProfile.SetValue(ExifTag.Orientation, (ushort)8);
                                    imageCoreMiddleRotated.ExifProfile.SetValue(ExifTag.Orientation, (ushort)8);
                                    imageCoreHighRotated.ExifProfile.SetValue(ExifTag.Orientation, (ushort)8);
                                    break;
                            }
                        }

                        break;
                }

                image.LowHeight = imageCoreLowRotated.Height;
                image.LowWidth = imageCoreLowRotated.Width;
                image.MidHeight = imageCoreMiddleRotated.Height;
                image.MidWidth = imageCoreMiddleRotated.Width;
                image.Height = imageCoreHighRotated.Height;
                image.Width = imageCoreHighRotated.Width;
                this.images.Update(image);

                imageCoreLowRotated.Save(imageStreamLowRotated);
                imageCoreMiddleRotated.Save(imageStreamMiddleRotated);
                imageCoreHighRotated.Save(imageStreamHighRotated);

                using (var fileStream = File.Create(lowFileFolder + image.FileName))
                {
                    imageStreamLowRotated.Seek(0, SeekOrigin.Begin);
                    imageStreamLowRotated.CopyTo(fileStream);
                }

                using (var fileStream = File.Create(middleFileFolder + image.FileName))
                {
                    imageStreamMiddleRotated.Seek(0, SeekOrigin.Begin);
                    imageStreamMiddleRotated.CopyTo(fileStream);
                }

                using (var fileStream = File.Create(highFileFolder + image.FileName))
                {
                    imageStreamHighRotated.Seek(0, SeekOrigin.Begin);
                    imageStreamHighRotated.CopyTo(fileStream);
                }

                File.Delete(lowFileFolder + "_" + image.FileName);
                File.Delete(middleFileFolder + "_" + image.FileName);
                File.Delete(highFileFolder + "_" + image.FileName);
            }
        }

        public void Update(Image image)
        {
            this.images.Update(image);
        }

        public void UpdateDateTaken(Guid id, DateTime date)
        {
            var image = this.images.GetById(id);

            if (image != null)
            {
                var oldFilename = image.FileName;
                image.DateTaken = date;
                image.FileName = date.ToString("yyyy-MM-dd-HH-mm-ss-") + Guid.NewGuid() + Path.GetExtension(oldFilename);
                this.Update(image);

                var lowFileFolder = this.appEnvironment.WebRootPath + Constants.MainContentFolder + "/" + image.AlbumId
                                    + "/" + Constants.ImageFolderLow + "/";
                var middleFileFolder = this.appEnvironment.WebRootPath + Constants.MainContentFolder + "/"
                                       + image.AlbumId + "/" + Constants.ImageFolderMiddle + "/";
                var highFileFolder = this.appEnvironment.WebRootPath + Constants.MainContentFolder + "/" + image.AlbumId
                                     + "/" + Constants.ImageFolderOriginal + "/";

                File.Move(lowFileFolder + oldFilename, lowFileFolder + "_" + oldFilename);
                File.Move(middleFileFolder + oldFilename, middleFileFolder + "_" + oldFilename);
                File.Move(highFileFolder + oldFilename, highFileFolder + "_" + oldFilename);

                var lowStream = new MemoryStream(File.ReadAllBytes(lowFileFolder + "_" + oldFilename));
                var middleStream = new MemoryStream(File.ReadAllBytes(middleFileFolder + "_" + oldFilename));
                var highStream = new MemoryStream(File.ReadAllBytes(highFileFolder + "_" + oldFilename));

                var imageCoreLow = new ImageProcessorCore.Image(lowStream);
                var imageCoreMiddle = new ImageProcessorCore.Image(middleStream);
                var imageCoreHigh = new ImageProcessorCore.Image(highStream);

                var format = "yyyy:MM:dd HH:mm:ss";

                imageCoreLow.ExifProfile.SetValue(ExifTag.DateTimeOriginal, date.ToString(format));
                imageCoreMiddle.ExifProfile.SetValue(ExifTag.DateTimeOriginal, date.ToString(format));
                imageCoreHigh.ExifProfile.SetValue(ExifTag.DateTimeOriginal, date.ToString(format));

                var imageStreamLowModified = new MemoryStream();
                var imageStreamMiddleModified = new MemoryStream();
                var imageStreamHighModified = new MemoryStream();

                imageCoreLow.Save(imageStreamLowModified);
                imageCoreMiddle.Save(imageStreamMiddleModified);
                imageCoreHigh.Save(imageStreamHighModified);

                using (var fileStream = File.Create(lowFileFolder + image.FileName))
                {
                    imageStreamLowModified.Seek(0, SeekOrigin.Begin);
                    imageStreamLowModified.CopyTo(fileStream);
                }

                using (var fileStream = File.Create(middleFileFolder + image.FileName))
                {
                    imageStreamMiddleModified.Seek(0, SeekOrigin.Begin);
                    imageStreamMiddleModified.CopyTo(fileStream);
                }

                using (var fileStream = File.Create(highFileFolder + image.FileName))
                {
                    imageStreamHighModified.Seek(0, SeekOrigin.Begin);
                    imageStreamHighModified.CopyTo(fileStream);
                }

                File.Delete(lowFileFolder + "_" + oldFilename);
                File.Delete(middleFileFolder + "_" + oldFilename);
                File.Delete(highFileFolder + "_" + oldFilename);
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

        private ushort? ExtractExifData(Image inputImage, Stream inputStream, string originalFileName)
        {
            var image = new ImageProcessorCore.Image(inputStream);
            var exif = image.ExifProfile;

            var dateTimeTaken = exif.Values.FirstOrDefault(x => x.Tag == ExifTag.DateTimeOriginal);
            if (dateTimeTaken != null)
            {
                var format = "yyyy:MM:dd HH:mm:ss";
                inputImage.DateTaken = DateTime.ParseExact(
                    dateTimeTaken.Value.ToString(),
                    format,
                    CultureInfo.InvariantCulture);
            }

            var gpdLong = exif.Values.FirstOrDefault(x => x.Tag == ExifTag.GPSLongitude);
            var gpdLat = exif.Values.FirstOrDefault(x => x.Tag == ExifTag.GPSLatitude);
            if (gpdLong != null && gpdLat != null)
            {
                inputImage.ImageGpsData =
                    this.locationService.GetGpsDataNormalized(
                        ExifGpsToDouble((Rational[])gpdLong.Value),
                        ExifGpsToDouble((Rational[])gpdLat.Value)).Result;
            }

            var make = exif.Values.FirstOrDefault(x => x.Tag == ExifTag.Make);
            if (make != null)
            {
                inputImage.CameraMaker = make.Value.ToString();
            }

            var model = exif.Values.FirstOrDefault(x => x.Tag == ExifTag.Model);
            if (model != null)
            {
                inputImage.CameraModel = model.Value.ToString();
            }

            var iso = exif.Values.FirstOrDefault(x => x.Tag == ExifTag.ISOSpeedRatings);
            if (iso != null)
            {
                inputImage.Iso = iso.Value.ToString();
            }

            var shutter = exif.Values.FirstOrDefault(x => x.Tag == ExifTag.ExposureTime);
            if (shutter != null)
            {
                inputImage.ShutterSpeed = shutter.Value.ToString();
            }

            var aperture = exif.Values.FirstOrDefault(x => x.Tag == ExifTag.ApertureValue);
            if (aperture != null)
            {
                Rational<uint> val = new Rational<uint>(
                                         ((Rational)aperture.Value).Numerator,
                                         ((Rational)aperture.Value).Denominator);
                double fstop = Math.Pow(2.0, Convert.ToDouble(val, CultureInfo.InvariantCulture) / 2.0);
                inputImage.Aperture = string.Format(new CultureInfo("en-US"), "f/{0:#0.0}", fstop);
            }

            var focuslen = exif.Values.FirstOrDefault(x => x.Tag == ExifTag.FocalLength);
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

            var exposure = exif.Values.FirstOrDefault(x => x.Tag == ExifTag.ExposureBiasValue);
            if (exposure != null)
            {
                var val = new Rational<int>(
                              ((SignedRational)exposure.Value).Numerator,
                              ((SignedRational)exposure.Value).Denominator);
                inputImage.ExposureBiasStep = val.Numerator != 0 ? val.ToString(CultureInfo.InvariantCulture) : "0";
            }

            inputImage.Width = image.Width;
            inputImage.Height = image.Height;

            if (inputImage.DateTaken != null)
            {
                inputImage.FileName = inputImage.DateTaken.Value.ToString("yyyy-MM-dd-HH-mm-ss-") + Guid.NewGuid();
            }
            else
            {
                inputImage.FileName = Path.GetFileNameWithoutExtension(originalFileName) + Guid.NewGuid();
            }

            inputImage.FileName += Path.GetExtension(originalFileName);

            var orientation = exif.Values.FirstOrDefault(x => x.Tag == ExifTag.Orientation);

            if (orientation == null)
            {
                return null;
            }

            return (ushort)orientation.Value;
        }

        private Image<Color, uint> Resize(Stream inputStream, ImageType type)
        {
            var image = new ImageProcessorCore.Image(inputStream);

            if (type == ImageType.Low)
            {
                var resizedImage =
                    image.Resize(
                        new ResizeOptions()
                            {
                                Mode = ResizeMode.Max,
                                Size = new Size(Constants.ImageLowMaxSize, Constants.ImageLowMaxSize),
                            });
                resizedImage.Quality = 70;
                return resizedImage;
            }
            else if (type == ImageType.Medium)
            {
                var resizedImage =
                    image.Resize(
                        new ResizeOptions()
                            {
                                Mode = ResizeMode.Max,
                                Size = new Size(Constants.ImageMiddleMaxSize, Constants.ImageMiddleMaxSize)
                            });
                return resizedImage;
            }

            return null;
        }
    }
}