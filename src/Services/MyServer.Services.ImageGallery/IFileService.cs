using MyServer.Common.ImageGallery;
using System;
using System.IO;

namespace MyServer.Services.ImageGallery
{
    public interface IFileService
    {
        string MakeValidFileName(string name);

        void EmptyTempFolder();

        void CreateInitialFolders(Guid albumId);

        string GetImageFolder(Guid albumId, ImageType type);

        void Save(
            Stream inputStream,
            ImageType type,
            string originalFilename,
            Guid albumId);
    }
}