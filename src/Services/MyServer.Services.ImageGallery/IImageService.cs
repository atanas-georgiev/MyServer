namespace MyServer.Services.ImageGallery
{
    using System;
    using System.Linq;
    using System.Web;

    using MyServer.Data.Models.ImageGallery;

    public interface IImageService
    {
        void Add(Guid albumId, HttpPostedFileBase file, HttpServerUtility server);

        IQueryable<Image> GetAll();

        Image GetById(Guid id);

        void Update(Image image);

        void Remove(Guid id);
    }
}