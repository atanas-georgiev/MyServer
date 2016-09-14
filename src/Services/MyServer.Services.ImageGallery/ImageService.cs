namespace MyServer.Services.ImageGallery
{
    using System;
    //using System.Drawing;
    //using System.Drawing.Imaging;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    using ImageProcessorCore;
    
    using MetadataExtractor;
    using MetadataExtractor.Formats.Exif;

    using MyServer.Common.ImageGallery;
    using MyServer.Data.Common;
    using MyServer.Data.Models;

    using Directory = System.IO.Directory;
    using Image = MyServer.Data.Models.Image;
    using Microsoft.AspNetCore.Hosting;
    using System.Drawing;

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
            if (fileStream == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }

            Image image = this.ExtractExifData(fileStream, fileName);

            if (image.FileName != null)
            {
                image.FileName += Path.GetExtension(fileName);
            }

            image.OriginalFileName = Path.GetFileName(fileName);

            // Create initial folders if not available
            this.fileService.CreateInitialFolders(albumId);
            this.fileService.Save(fileStream, ImageType.Original, image.FileName, albumId);
            this.Resize(fileStream, ImageType.Medium, albumId, image.FileName);
            this.Resize(fileStream, ImageType.Low, albumId, image.FileName);

            GC.Collect();

            var albumDb = this.albums.GetById(albumId);
            var cover = this.albums.GetById(Guid.Parse(Constants.NoCoverId));


            image.Id = Guid.NewGuid();
            image.Album = albumDb;
            image.Title = string.Empty;
            image.LowHeight = this.lowheight;
            image.LowWidth = this.lowwidth;
            image.MidHeight = this.midheight;
            image.MidWidth = this.midwidth;
            image.AddedById = userId;
        //    image.Cover = cover;
          
            this.images.Add(image);

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

        private Image ExtractExifData(Stream inputStream, string originalFileName)
        {
            var newImage = new Image();

            var image = new ImageProcessorCore.Image(inputStream);

            var exif = image.ExifProfile;
            var a = exif.Values;

            var dateTimeTaken = exif.Values.Where(x => x.Tag == ExifTag.DateTimeOriginal).FirstOrDefault();

            if (dateTimeTaken != null)
            {
                var format = "yyyy:MM:dd HH:mm:ss";
                newImage.DateTaken = DateTime.ParseExact(dateTimeTaken.Value.ToString(), format, CultureInfo.InvariantCulture);
            }


            /*
            using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
            {
                var imageFactoryStream = ImageMetadataReader.ReadMetadata(inputStream);
                var exifMain = imageFactoryStream.OfType<ExifSubIfdDirectory>().FirstOrDefault();
                var exifExtended = imageFactoryStream.OfType<ExifIfd0Directory>().FirstOrDefault();
                var exifGps = imageFactoryStream.OfType<GpsDirectory>().FirstOrDefault();

                var gps = exifGps?.GetGeoLocation();

                //if (gps != null && !gps.IsZero)
                //{
                //    var gpsData = this.locationService.GetGpsData(gps.Latitude, gps.Longitude);

                //    if (gpsData != null)
                //    {
                //        newImage.ImageGpsData = gpsData;
                //    }
                //}

                if (exifExtended != null)
                {
                    if (exifExtended.ContainsTag(ExifDirectoryBase.TagMake))
                    {
                        newImage.CameraMaker = exifExtended.GetDescription(ExifDirectoryBase.TagMake);
                    }

                    if (exifExtended.ContainsTag(ExifDirectoryBase.TagModel))
                    {
                        newImage.CameraModel = exifExtended.GetDescription(ExifDirectoryBase.TagModel);
                    }
                }

                if (exifMain != null)
                {
                    if (exifMain.ContainsTag(ExifDirectoryBase.TagDateTimeOriginal))
                    {
                        newImage.DateTaken = exifMain.GetDateTime(ExifDirectoryBase.TagDateTimeOriginal);
                    }

                    if (exifMain.ContainsTag(ExifDirectoryBase.TagIsoEquivalent))
                    {
                        newImage.Iso = exifMain.GetDescription(ExifDirectoryBase.TagIsoEquivalent);
                    }

                    if (exifMain.ContainsTag(ExifDirectoryBase.TagExposureTime))
                    {
                        newImage.ShutterSpeed = exifMain.GetDescription(ExifDirectoryBase.TagExposureTime);
                    }

                    if (exifMain.ContainsTag(ExifDirectoryBase.TagFNumber))
                    {
                        newImage.Aperture = exifMain.GetDescription(ExifDirectoryBase.TagFNumber);
                    }

                    if (exifMain.ContainsTag(ExifDirectoryBase.TagFocalLength))
                    {
                        newImage.FocusLen = exifMain.GetDescription(ExifDirectoryBase.TagFocalLength);
                    }

                    if (exifMain.ContainsTag(ExifDirectoryBase.TagExposureBias))
                    {
                        newImage.ExposureBiasStep = exifMain.GetDescription(ExifDirectoryBase.TagExposureBias);
                    }

                    if (exifMain.ContainsTag(ExifDirectoryBase.TagLensModel))
                    {
                        newImage.Lenses = exifMain.GetDescription(ExifDirectoryBase.TagLensModel);
                    }

                    if (exifMain.ContainsTag(ExifDirectoryBase.TagExifImageWidth))
                    {
                        newImage.Width = exifMain.GetInt32(ExifDirectoryBase.TagExifImageWidth);
                    }

                    if (exifMain.ContainsTag(ExifDirectoryBase.TagExifImageHeight))
                    {
                        newImage.Height = exifMain.GetInt32(ExifDirectoryBase.TagExifImageHeight);
                    }
                }*/

                if (newImage.DateTaken != null)
                {
                    newImage.FileName = newImage.DateTaken.Value.ToString("yyyy-MM-dd-HH-mm-ss-") + Guid.NewGuid();
                }
                else
                {
                    newImage.FileName = Path.GetFileNameWithoutExtension(originalFileName) + Guid.NewGuid();
                }
            //}

            return newImage;
        }

        private void Resize(
            Stream inputStream, 
            ImageType type, 
            Guid albumId, 
            string originalFilename)
        {
            inputStream.Seek(0, SeekOrigin.Begin);

            if (type == ImageType.Low)
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    //using (ImageFactory imageFactory = new ImageFactory(preserveExifData: false))
                    //{
                    //    imageFactory.Load(inputStream)
                    //        //.Resize(
                    //        //    new ResizeLayer(
                    //        //        new Size(Constants.ImageLowMaxSize, Constants.ImageLowMaxSize), 
                    //        //        ResizeMode.Max))
                    //        .Format(new JpegFormat { Quality = 70 })
                    //        .Resolution(96, 96)
                    //        .Save(outStream);
                    //    //this.lowwidth = imageFactory.Load(outStream).Image.Width;
                    //    //this.lowheight = imageFactory.Load(outStream).Image.Height;
                    //    this.fileService.Save(outStream, type, originalFilename, albumId);
                    //}
                }

                GC.Collect();
            }
            else if (type == ImageType.Medium)
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    //using (ImageFactory imageFactory = new ImageFactory(preserveExifData: false))
                    //{
                    //    imageFactory.Load(inputStream)
                    //        //.Resize(
                    //        //    new ResizeLayer(
                    //        //        new Size(Constants.ImageMiddleMaxSize, Constants.ImageMiddleMaxSize), 
                    //        //        ResizeMode.Max))
                    //        .Format(new JpegFormat { Quality = 85 })
                    //        .Resolution(96, 96)
                    //        .Save(outStream);
                    //    // this.midwidth = imageFactory.Load(outStream).Image.Width;
                    //    // this.midheight = imageFactory.Load(outStream).Image.Height;
                    //    this.fileService.Save(outStream, type, originalFilename, albumId);
                    //}
                }

                GC.Collect();
            }
        }
    }
}