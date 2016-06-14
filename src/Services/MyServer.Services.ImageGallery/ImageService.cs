namespace MyServer.Services.ImageGallery
{
    using System;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;

    using ImageProcessor;
    using ImageProcessor.Imaging;
    using ImageProcessor.Imaging.Formats;

    using MetadataExtractor;
    using MetadataExtractor.Formats.Exif;

    using MyServer.Common.ImageGallery;
    using MyServer.Data.Common;
    using MyServer.Data.Models.ImageGallery;

    using Directory = System.IO.Directory;
    using Image = MyServer.Data.Models.ImageGallery.Image;

    public class ImageService : IImageService
    {
        private IRepository<Album, Guid> albums;

        private IRepository<Image, Guid> images;

        private ILocationService locationService;

        private int lowheight;

        private int lowwidth;

        private int midheight;

        private int midwidth;

        public ImageService(IRepository<Image, Guid> images, IRepository<Album, Guid> albums, ILocationService locationService)
        {
            this.images = images;
            this.albums = albums;
            this.locationService = locationService;
        }

        public void Add(Guid albumId, HttpPostedFileBase file, HttpServerUtility server)
        {
            if (file == null)
            {
                return;
            }

            if (file.ContentType != "image/jpeg")
            {
                return;
            }

            Image image = this.ExtractExifData(file.InputStream, Path.GetFileName(file.FileName));

            if (image.FileName != null)
            {
                image.FileName += Path.GetExtension(file.FileName);
            }

            image.OriginalFileName = Path.GetFileName(file.FileName);

            // Create initial folders if not available
            FileService.CreateInitialFolders(albumId, server);
            FileService.Save(file.InputStream, ImageType.Original, image.FileName, albumId, server);
            this.Resize(file.InputStream, ImageType.Medium, albumId, image.FileName, server);
            this.Resize(file.InputStream, ImageType.Low, albumId, image.FileName, server);

            GC.Collect();

            image.AlbumId = albumId;
            image.Title = string.Empty;
            image.LowHeight = this.lowheight;
            image.LowWidth = this.lowwidth;
            image.MidHeight = this.midheight;
            image.MidWidth = this.midwidth;

            this.images.Add(image);

            // check if first image, if so, make album cover
            var album = this.albums.GetById(albumId);
            if (album.Images.Count == 1)
            {
                album.CoverId = image.Id;
                this.albums.Update(album);
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

        public void Remove(Guid id)
        {
            this.images.Delete(id);
        }

        public void Update(Image image)
        {
            this.images.Update(image);
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

        private void EmptyTempFolder(HttpServerUtilityBase server)
        {
            var di = new DirectoryInfo(server.MapPath(Constants.TempContentFolder));

            foreach (var dir in di.GetDirectories())
            {
                foreach (var file in dir.GetFiles())
                {
                    file.Delete();
                }

                dir.Delete(true);
            }
        }            

        public void PrepareFileForDownload(Guid id, HttpServerUtilityBase server)
        {
            var image = this.GetById(id);
            var filePathServer = server.MapPath(Constants.MainContentFolder + "\\" + image.AlbumId + "\\" + Constants.ImageFolderOriginal + "\\" + image.FileName);
            var filePathTemp = server.MapPath(Constants.TempContentFolder + "\\" + id + "\\" + image.OriginalFileName);

            this.EmptyTempFolder(server);
            Directory.CreateDirectory(server.MapPath(Constants.TempContentFolder + "\\" + id));
            
            File.Copy(filePathServer, filePathTemp);
        }

        private Image ExtractExifData(Stream inputStream, string originalFileName)
        {
            var newImage = new Image();

            using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
            {
                var imageFactoryStream = ImageMetadataReader.ReadMetadata(inputStream);
                var exifMain = imageFactoryStream.OfType<ExifSubIfdDirectory>().FirstOrDefault();
                var exifExtended = imageFactoryStream.OfType<ExifIfd0Directory>().FirstOrDefault();
                var exifGps = imageFactoryStream.OfType<GpsDirectory>().FirstOrDefault();
                
                var gps = exifGps?.GetGeoLocation();

                if (gps != null && !gps.IsZero)
                {
                    var gpsData = this.locationService.GetGpsData(gps.Latitude, gps.Longitude);

                    if (gpsData != null)
                    {
                        newImage.ImageGpsData = gpsData;
                    }
                }

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
                }

                if (newImage.DateTaken != null)
                {
                    newImage.FileName = newImage.DateTaken.Value.ToString("yyyy-MM-dd-HH-mm-ss-", CultureInfo.CreateSpecificCulture("en-US")) + Guid.NewGuid();
                }
                else
                {
                    newImage.FileName = Path.GetFileNameWithoutExtension(originalFileName) + Guid.NewGuid();
                }
            }

            return newImage;
        }

        private void Resize(
            Stream inputStream, 
            ImageType type, 
            Guid albumId, 
            string originalFilename, 
            HttpServerUtility server)
        {
            inputStream.Seek(0, SeekOrigin.Begin);

            if (type == ImageType.Low)
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
                    {
                        imageFactory.Load(inputStream)
                            .Resize(
                                new ResizeLayer(
                                    new Size(Constants.ImageLowMaxSize, Constants.ImageLowMaxSize),
                                    ResizeMode.Max))
                            .Format(new JpegFormat { Quality = 75 })
                            .Save(outStream);
                        this.lowwidth = imageFactory.Load(outStream).Image.Width;
                        this.lowheight = imageFactory.Load(outStream).Image.Height;
                        FileService.Save(outStream, type, originalFilename, albumId, server);
                    }
                }
                GC.Collect();
            }
            else if (type == ImageType.Medium)
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
                    {
                        imageFactory.Load(inputStream)
                            .Resize(
                                new ResizeLayer(
                                    new Size(Constants.ImageMiddleMaxSize, Constants.ImageMiddleMaxSize),
                                    ResizeMode.Max))
                            .Format(new JpegFormat { Quality = 75 })
                            .Save(outStream);
                        this.midwidth = imageFactory.Load(outStream).Image.Width;
                        this.midheight = imageFactory.Load(outStream).Image.Height;
                        FileService.Save(outStream, type, originalFilename, albumId, server);
                    }
                }
                GC.Collect();
            }
        }
    }
}