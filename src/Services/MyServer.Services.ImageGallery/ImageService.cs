namespace MyServer.Services.ImageGallery
{
    using System;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Web;

    using ImageProcessor;
    using ImageProcessor.Imaging;
    using ImageProcessor.Imaging.Formats;

    using MetadataExtractor;
    using MetadataExtractor.Formats.Exif;

    using MyServer.Common.ImageGallery;
    using MyServer.Data.Common;
    using MyServer.Data.Models.ImageGallery;

    using Image = MyServer.Data.Models.ImageGallery.Image;

    public class ImageService : IImageService
    {
        private IRepository<Album, Guid> albums;

        private IRepository<Image, Guid> images;
        
        private int midwidth;
        private int midheight;

        private int lowwidth;
        private int lowheight;

        private DateTime dateTaken;

        public ImageService(IRepository<Image, Guid> images, IRepository<Album, Guid> albums)
        {
            this.images = images;
            this.albums = albums;
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

            Image image = this.ExtractExifData(file.InputStream);

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
            image.Title = "";
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

        private Image ExtractExifData(Stream inputStream)
        {
            var newImage = new Image();

            using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
            {
                var imageFactoryStream = ImageMetadataReader.ReadMetadata(inputStream);
                var subIfdDirectory = imageFactoryStream.OfType<ExifSubIfdDirectory>().FirstOrDefault();
                var subIfdDirectory2 = imageFactoryStream.OfType<ExifIfd0Directory>().FirstOrDefault();

                try
                {
                    newImage.DateTaken = subIfdDirectory?.GetDateTime(ExifDirectoryBase.TagDateTimeOriginal);
                }
                catch
                {
                    newImage.DateTaken = subIfdDirectory?.GetDateTime(ExifDirectoryBase.TagDateTime);
                }

                try
                {
                    newImage.FileName =
                        subIfdDirectory?.GetDateTime(ExifDirectoryBase.TagDateTimeOriginal)
                            .ToString("yyyy-MM-dd-HH-mm-ss-", CultureInfo.CreateSpecificCulture("en-US")) +
                        Guid.NewGuid();
                }
                catch
                {
                    // ignored
                }

                try
                {
                    newImage.Iso = subIfdDirectory?.GetInt32(ExifDirectoryBase.TagIsoEquivalent);
                }
                catch
                {
                    // ignored
                }

                try
                {
                    newImage.ShutterSpeed = subIfdDirectory?.GetDescription(ExifDirectoryBase.TagShutterSpeed);
                }
                catch
                {
                    // ignored
                }

                try
                {
                    newImage.Aperture = subIfdDirectory?.GetDescription(ExifDirectoryBase.TagAperture);
                }
                catch
                {
                    // ignored
                }

                try
                {
                    newImage.FocusLen = subIfdDirectory?.GetDouble(ExifDirectoryBase.TagFocalLength);
                }
                catch
                {
                    // ignored
                }

                try
                {
                    newImage.CameraMaker = subIfdDirectory2?.GetString(ExifDirectoryBase.TagMake);
                }
                catch
                {
                    // ignored
                }

                try
                {
                    newImage.CameraModel = subIfdDirectory2?.GetString(ExifDirectoryBase.TagModel);
                }
                catch
                {
                    // ignored
                }

                try
                {
                    newImage.ExposureBiasStep = subIfdDirectory?.GetDouble(ExifDirectoryBase.TagExposureBias);
                }
                catch
                {
                    // ignored
                }

                try
                {
                    newImage.Width = subIfdDirectory.GetInt32(ExifDirectoryBase.TagExifImageWidth);
                }
                catch
                {
                    // ignored
                }

                try
                {
                    newImage.Height = subIfdDirectory.GetInt32(ExifDirectoryBase.TagExifImageHeight);
                }
                catch
                {
                    // ignored
                }

                try
                {
                    newImage.Lenses = subIfdDirectory.GetString(ExifDirectoryBase.TagLensModel);
                }
                catch
                {
                    // ignored
                }
            }

            return newImage;
        }

        private void Resize(Stream inputStream, ImageType type, Guid albumId, string originalFilename, HttpServerUtility server)
        {
            inputStream.Seek(0, SeekOrigin.Begin);

            using (MemoryStream outStream = new MemoryStream())
            {                
                using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
                {
                    if (type == ImageType.Low)
                    {
                        imageFactory.Load(inputStream)
                            .Resize(new ResizeLayer(new Size(Constants.ImageLowMaxSize, Constants.ImageLowMaxSize), ResizeMode.Max))
                            .Format(new JpegFormat { Quality = 90 })
                            .Save(outStream);
                        this.lowwidth = imageFactory.Load(outStream).Image.Width;
                        this.lowheight = imageFactory.Load(outStream).Image.Height;
                    }
                    else if (type == ImageType.Medium)
                    {
                        imageFactory.Load(inputStream)
                            .Resize(new ResizeLayer(new Size(Constants.ImageMiddleMaxSize, Constants.ImageMiddleMaxSize), ResizeMode.Max))
                            .Format(new JpegFormat { Quality = 90 })
                            .Save(outStream);
                        this.midwidth = imageFactory.Load(outStream).Image.Width;
                        this.midheight = imageFactory.Load(outStream).Image.Height;
                    }
                    
                    FileService.Save(outStream, type, originalFilename, albumId, server);
                }
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

        public void Update(Image image)
        {
            this.images.Update(image);
        }

        public void Remove(Guid id)
        {
            this.images.Delete(id);
        }
    }
}