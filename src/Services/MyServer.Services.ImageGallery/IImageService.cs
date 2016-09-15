namespace MyServer.Services.ImageGallery
{
    using System;
    using System.Linq;

    using MyServer.Data.Models;
    using System.IO;

    public interface IImageService
    {
        void Add(Guid albumId, string userId, Stream fileStream, string fileName);

        IQueryable<Image> GetAll();

        IQueryable<Image> GetAllReqursive();

        Image GetById(Guid id);

        void Remove(Guid id);

        void Update(Image image);

        void AddGpsDataToImage(Guid id, ImageGpsData gpsData);

        void PrepareFileForDownload(Guid id);
    }
}