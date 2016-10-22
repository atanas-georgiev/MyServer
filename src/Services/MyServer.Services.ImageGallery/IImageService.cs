namespace MyServer.Services.ImageGallery
{
    using System;
    using System.IO;
    using System.Linq;

    using MyServer.Common;
    using MyServer.Data.Models;

    public interface IImageService
    {
        void Add(Guid albumId, string userId, Stream fileStream, string fileName);

        void AddGpsDataToImage(Guid id, ImageGpsData gpsData);

        IQueryable<Image> GetAll();

        IQueryable<Image> GetAllReqursive();

        Image GetById(Guid id);

        string GetRandomImagePath();

        void PrepareFileForDownload(Guid id);

        void Remove(Guid id);

        void Update(Image image);

        void Rotate(Guid imageId, MyServerRotateType rotateType);
    }
}