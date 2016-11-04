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

        void AddGpsDataToImage(Guid imageId, ImageGpsData gpsData);

        IQueryable<Image> GetAllReqursive(bool cache = true);

        Image GetById(Guid id, bool cache = true);

        string GetRandomImagePath();

        void PrepareFileForDownload(Guid id);

        void Remove(Guid id);

        void Rotate(Guid imageId, MyServerRotateType rotateType);

        void Update(Image image);

        void UpdateDateTaken(Guid imageId, DateTime date);
    }
}