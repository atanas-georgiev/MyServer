namespace MyServer.Services.ImageGallery
{
    using System;
    using System.IO;
    using System.Web;

    using MyServer.Common.ImageGallery;
    using MyServer.Data.Models.ImageGallery;

    public static class FileService
    {
        public static string GetImageFolder(Guid albumId, ImageType type, HttpServerUtility server)
        {
            switch (type)
            {
                case ImageType.Low:
                    return
                        server.MapPath(
                            Constants.MainContentFolder + "\\" + albumId + "\\" + Constants.ImageFolderLow + "\\");
                case ImageType.Medium:
                    return
                        server.MapPath(
                            Constants.MainContentFolder + "\\" + albumId + "\\" + Constants.ImageFolderMiddle + "\\");
                case ImageType.Original:
                    return
                        server.MapPath(
                            Constants.MainContentFolder + "\\" + albumId + "\\" + Constants.ImageFolderOriginal + "\\");
            }

            return string.Empty;
        }

        internal static void CreateInitialFolders(Guid albumId, HttpServerUtility server)
        {
            if (!Directory.Exists(server.MapPath(Constants.MainContentFolder)))
            {
                Directory.CreateDirectory(server.MapPath(Constants.MainContentFolder));
            }

            if (!Directory.Exists(server.MapPath(Constants.MainContentFolder + "\\" + albumId)))
            {
                Directory.CreateDirectory(server.MapPath(Constants.MainContentFolder + "\\" + albumId));
            }

            if (
                !Directory.Exists(
                    server.MapPath(Constants.MainContentFolder + "\\" + albumId + "\\" + Constants.ImageFolderOriginal)))
            {
                Directory.CreateDirectory(
                    server.MapPath(Constants.MainContentFolder + "\\" + albumId + "\\" + Constants.ImageFolderOriginal));
            }

            if (
                !Directory.Exists(
                    server.MapPath(Constants.MainContentFolder + "\\" + albumId + "\\" + Constants.ImageFolderMiddle)))
            {
                Directory.CreateDirectory(
                    server.MapPath(Constants.MainContentFolder + "\\" + albumId + "\\" + Constants.ImageFolderMiddle));
            }

            if (
                !Directory.Exists(
                    server.MapPath(Constants.MainContentFolder + "\\" + albumId + "\\" + Constants.ImageFolderLow)))
            {
                Directory.CreateDirectory(
                    server.MapPath(Constants.MainContentFolder + "\\" + albumId + "\\" + Constants.ImageFolderLow));
            }
        }

        internal static void Save(
            Stream inputStream, 
            ImageType type, 
            string originalFilename, 
            Guid albumId, 
            HttpServerUtility server)
        {
            using (var fileStream = File.Create(GetImageFolder(albumId, type, server) + originalFilename))
            {
                inputStream.Seek(0, SeekOrigin.Begin);
                inputStream.CopyTo(fileStream);
                fileStream.Close();
            }
        }
    }
}