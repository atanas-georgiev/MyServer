namespace MyServer.Services.ImageGallery
{
    using System;
    //using System.Drawing;
    //using System.Drawing.Imaging;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    using ImageProcessorCore;

    using MyServer.Common.ImageGallery;
    using MyServer.Data.Common;
    using MyServer.Data.Models;

    using Directory = System.IO.Directory;
    using Image = MyServer.Data.Models.Image;
    using Microsoft.AspNetCore.Hosting;

    public class ImageService : IImageService
    {
        private readonly IRepository<Album, Guid> albums;

        private readonly IRepository<Image, Guid> images;

        private readonly ILocationService locationService;

        private readonly IFileService fileService;

        private readonly IHostingEnvironment appEnvironment;

        private int lowheight;

        private int lowwidth;

        private int midheight;

        private int midwidth;

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
            if (albumId == null || string.IsNullOrEmpty(userId) || fileStream == null || string.IsNullOrEmpty(fileName))
            {
                return;
            }

            var image = new Image();
            image.Id = Guid.NewGuid();
            image.AlbumId = albumId;
            image.OriginalFileName = Path.GetFileName(fileName);
            image.Title = string.Empty;
            image.LowHeight = this.lowheight;
            image.LowWidth = this.lowwidth;
            image.MidHeight = this.midheight;
            image.MidWidth = this.midwidth;
            image.AddedById = userId;

            // Add exif data
            this.ExtractExifData(image, fileStream, fileName);

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

            GC.Collect();

            var cover = this.albums.GetById(Guid.Parse(Constants.NoCoverId));                       

            // check if first image, if so, make album cover
            var album = this.albums.GetById(albumId);
            if (album.Images.Count == 1)
            {
                album.CoverId = image.Id;
                this.albums.Update(album);
            }
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
            return this.images.All();
        }

        public Image GetById(Guid id)
        {
            return this.GetAll().FirstOrDefault(x => x.Id == id);
        }

        public void PrepareFileForDownload(Guid id)
        {
            var image = this.GetById(id);
            var filePathServer = appEnvironment.WebRootPath + Constants.MainContentFolder + "\\" + image.AlbumId + "\\" + Constants.ImageFolderOriginal + "\\" + image.FileName;
            var filePathTemp = appEnvironment.WebRootPath + Constants.TempContentFolder + "\\" + id + "\\" + image.OriginalFileName;

            this.fileService.EmptyTempFolder();
            Directory.CreateDirectory(appEnvironment.WebRootPath + Constants.TempContentFolder + "\\" + id);

            File.Copy(filePathServer, filePathTemp);
        }

        public void Remove(Guid id)
        {
            var coverId = this.GetById(id).Album.Cover.Id;

            //if (id == coverId)
            //{
            //    var firstImage = this.images.All().Where(x => x.Id)      
            //}

            this.images.Delete(id);
        }

        public void Update(Image image)
        {
            this.images.Update(image);
        }

        private void ExtractExifData(Image inputImage, Stream inputStream, string originalFileName)
        {
            var image = new ImageProcessorCore.Image(inputStream);
            var exif = image.ExifProfile;

            var dateTimeTaken = exif.Values.Where(x => x.Tag == ExifTag.DateTimeOriginal).FirstOrDefault();
            if (dateTimeTaken != null)
            {
                var format = "yyyy:MM:dd HH:mm:ss";
                inputImage.DateTaken = DateTime.ParseExact(dateTimeTaken.Value.ToString(), format, CultureInfo.InvariantCulture);
            }

            //if (gps != null && !gps.IsZero)
            //{
            //    var gpsData = this.locationService.GetGpsData(gps.Latitude, gps.Longitude);

            //    if (gpsData != null)
            //    {
            //        newImage.ImageGpsData = gpsData;
            //    }
            //}

            var make = exif.Values.Where(x => x.Tag == ExifTag.Make).FirstOrDefault();
            if (make != null)
            {
                inputImage.CameraMaker = make.Value.ToString();
            }

            var model = exif.Values.Where(x => x.Tag == ExifTag.Model).FirstOrDefault();
            if (model != null)
            {
                inputImage.CameraModel = model.Value.ToString();
            }

            var iso = exif.Values.Where(x => x.Tag == ExifTag.ISOSpeedRatings).FirstOrDefault();
            if (iso != null)
            {
                inputImage.Iso = iso.Value.ToString();
            }

            var shutter = exif.Values.Where(x => x.Tag == ExifTag.ExposureTime).FirstOrDefault();
            if (shutter != null)
            {
                inputImage.ShutterSpeed = shutter.Value.ToString();
            }

            var aperture = exif.Values.Where(x => x.Tag == ExifTag.ApertureValue).FirstOrDefault();
            if (aperture != null)
            {
                inputImage.Aperture = aperture.Value.ToString();
            }

            var focuslen = exif.Values.Where(x => x.Tag == ExifTag.FocalLength).FirstOrDefault();
            if (focuslen != null)
            {
                inputImage.FocusLen = focuslen.Value.ToString();
            }

            var exposure = exif.Values.Where(x => x.Tag == ExifTag.ExposureBiasValue).FirstOrDefault();
            if (exposure != null)
            {
                inputImage.ExposureBiasStep = exposure.Value.ToString();
            }

            //var lens = exif.Values.Where(x => x.Tag == ExifTag.le).FirstOrDefault();
            //if (lens != null)
            //{
            //    inputImage.ExposureBiasStep = lens.Value.ToString();
            //}

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
        }

        private ImageProcessorCore.Image<ImageProcessorCore.Color, uint>Resize(Stream inputStream, ImageType type)
        {
            var image = new ImageProcessorCore.Image(inputStream);

            if (type == ImageType.Low)
            {
                var resizedImage = image.Resize(new ResizeOptions()
                {
                    Mode = ResizeMode.Max,
                    Size = new ImageProcessorCore.Size(Constants.ImageLowMaxSize, Constants.ImageLowMaxSize)
                });

                return resizedImage;
            }
            else if (type == ImageType.Medium)
            {
                var resizedImage = image.Resize(new ResizeOptions()
                {
                    Mode = ResizeMode.Max,
                    Size = new ImageProcessorCore.Size(Constants.ImageMiddleMaxSize, Constants.ImageMiddleMaxSize)
                });

                return resizedImage;
            }

            return null;
        }
    }
}