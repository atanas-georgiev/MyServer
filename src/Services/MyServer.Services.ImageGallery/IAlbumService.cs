namespace MyServer.Services.ImageGallery
{
    using System;
    using System.Linq;

    using MyServer.Data.Models;

    public interface IAlbumService
    {
        void Add(Album album);

        string GenerateZipArchive(Guid id);

        IQueryable<Album> GetAll();

        IQueryable<Album> GetAllReqursive();

        Album GetById(Guid id);

        void Remove(Guid id);

        void Update(Album album);

        void UpdateCoverImage(Guid album, Guid image);
    }
}