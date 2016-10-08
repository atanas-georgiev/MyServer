namespace MyServer.Services.ImageGallery
{
    using System;
    using System.IO;

    using Microsoft.AspNetCore.Hosting;

    using MyServer.Common.ImageGallery;

    public class FileService : IFileService
    {
        private readonly IHostingEnvironment appEnvironment;

        public FileService(IHostingEnvironment appEnvironment)
        {
            this.appEnvironment = appEnvironment;
        }

        public void CreateInitialFolders(Guid albumId)
        {
            var contentFolder = this.appEnvironment.WebRootPath + Constants.MainContentFolder;

            if (!Directory.Exists(contentFolder))
            {
                Directory.CreateDirectory(contentFolder);
            }

            if (!Directory.Exists(this.appEnvironment.WebRootPath + Constants.TempContentFolder))
            {
                Directory.CreateDirectory(this.appEnvironment.WebRootPath + Constants.TempContentFolder);
            }

            if (!Directory.Exists(this.appEnvironment.WebRootPath + Constants.MainContentFolder + "\\" + albumId))
            {
                Directory.CreateDirectory(
                    this.appEnvironment.WebRootPath + Constants.MainContentFolder + "\\" + albumId);
            }

            if (!Directory.Exists(contentFolder + "\\" + albumId + "\\" + Constants.ImageFolderOriginal))
            {
                Directory.CreateDirectory(contentFolder + "\\" + albumId + "\\" + Constants.ImageFolderOriginal);
            }

            if (!Directory.Exists(contentFolder + "\\" + albumId + "\\" + Constants.ImageFolderMiddle))
            {
                Directory.CreateDirectory(contentFolder + "\\" + albumId + "\\" + Constants.ImageFolderMiddle);
            }

            if (!Directory.Exists(contentFolder + "\\" + albumId + "\\" + Constants.ImageFolderLow))
            {
                Directory.CreateDirectory(contentFolder + "\\" + albumId + "\\" + Constants.ImageFolderLow);
            }
        }

        public void EmptyTempFolder()
        {
            var di = new DirectoryInfo(this.appEnvironment.WebRootPath + Constants.TempContentFolder);

            foreach (var dir in di.GetDirectories())
            {
                foreach (var file in dir.GetFiles())
                {
                    file.Delete();
                }

                dir.Delete(true);
            }
        }

        public string GetImageFolder(Guid albumId, ImageType type)
        {
            var contentFolder = this.appEnvironment.WebRootPath + Constants.MainContentFolder;

            switch (type)
            {
                case ImageType.Low:
                    return contentFolder + "\\" + albumId + "\\" + Constants.ImageFolderLow + "\\";
                case ImageType.Medium:
                    return contentFolder + "\\" + albumId + "\\" + Constants.ImageFolderMiddle + "\\";
                case ImageType.Original:
                    return contentFolder + "\\" + albumId + "\\" + Constants.ImageFolderOriginal + "\\";
            }

            return string.Empty;
        }

        public string MakeValidFileName(string name)
        {
            var invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            var invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
        }

        public void Save(Stream inputStream, ImageType type, string originalFilename, Guid albumId)
        {
            using (var fileStream = File.Create(this.GetImageFolder(albumId, type) + originalFilename))
            {
                inputStream.Seek(0, SeekOrigin.Begin);
                inputStream.CopyTo(fileStream);
            }
        }
    }
}