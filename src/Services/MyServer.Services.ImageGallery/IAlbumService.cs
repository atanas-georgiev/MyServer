namespace MyServer.Services.ImageGallery
{
    using System;
    using System.Linq;

    using MyServer.Common.ImageGallery;
    using MyServer.Data.Models;

    public interface IAlbumService
    {
        void Add(Album album);

        string GenerateZipArchive(Guid id, ImageType type);

        IQueryable<Album> GetAllReqursive(bool cache = true);

        Album GetById(Guid id, bool cache = true);

        void Remove(Guid id);

        void Update(Album album);

        void UpdateCoverImage(Guid album, Guid image);
    }
}