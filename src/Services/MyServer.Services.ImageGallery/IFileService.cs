namespace MyServer.Services.ImageGallery
{
    using System;
    using System.IO;

    using MyServer.Common.ImageGallery;

    public interface IFileService
    {
        void CreateInitialFolders(Guid albumId);

        void EmptyTempFolder();

        string GetImageFolder(Guid albumId, ImageType type);

        string GetImageFolderSize();

        string MakeValidFileName(string name);

        void RemoveAlbum(Guid albumId);

        void RemoveImage(Guid albumId, string fileName);

        void Save(Stream inputStream, ImageType type, string originalFilename, Guid albumId);
    }
}