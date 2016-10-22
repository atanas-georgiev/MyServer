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
            var image = this.GetById(id);

            if (image != null)
            {
                image.ImageGpsData = gpsData;
                this.Update(image);
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
            var data = this.images.All()
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
            this.GetAllReqursive();
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
            var album = this.GetById(id).Album;

            if (id == album.CoverId)
            {
                var noCoverImageGuid = Guid.Parse(Constants.NoCoverId);
                var noCoverImage = this.images.All().FirstOrDefault(x => x.Id == noCoverImageGuid);
                album.CoverId = noCoverImage.Id;
                this.albums.Update(album);
            }

            this.images.Delete(id);
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

        private static double ExifGpsToDouble(Rational[] propItem)
        {
            uint degreesNumerator = propItem[0].Numerator;
            uint degreesDenominator = propItem[0].Denominator;
            double degrees = degreesNumerator / (double)degreesDenominator;

            uint minutesNumerator = propItem[1].Numerator;
            uint minutesDenominator = propItem[1].Denominator;
            double minutes = minutesNumerator / (double)minutesDenominator;

            uint secondsNumerator = propItem[2].Numerator;
            uint secondsDenominator = propItem[2].Denominator;
            double seconds = secondsNumerator / (double)secondsDenominator;

            double coorditate = degrees + (minutes / 60f) + (seconds / 3600f);

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
                    this.locationService.GetGpsData(
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