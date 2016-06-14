namespace MyServer.Services.ImageGallery
{
    using System;
    using System.Linq;
    using System.Web;

    using MyServer.Data.Models.ImageGallery;

    public interface IAlbumService
    {
        void Add(Album album);

        IQueryable<Album> GetAll();

        Album GetById(Guid id);

        void Update(Album album);

        void UpdateCoverImage(Guid album, Guid image);

        string GenerateZipArchive(Guid id, HttpServerUtilityBase server);
    }
}