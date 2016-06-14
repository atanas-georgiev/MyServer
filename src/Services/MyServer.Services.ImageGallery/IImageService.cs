namespace MyServer.Services.ImageGallery
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;

    using MyServer.Data.Models.ImageGallery;

    public interface IImageService
    {
        void Add(Guid albumId, HttpPostedFileBase file, HttpServerUtility server);

        IQueryable<Image> GetAll();

        Image GetById(Guid id);

        void Remove(Guid id);

        void Update(Image image);

        void AddGpsDataToImage(Guid id, ImageGpsData gpsData);

        void PrepareFileForDownload(Guid id, HttpServerUtilityBase server);
    }
}