﻿namespace MyServer.Services.ImageGallery
{
    using System;
    using System.Linq;

    using MyServer.Data.Models;

    public interface IAlbumService
    {
        void Add(Album album);

        IQueryable<Album> GetAll();

        IQueryable<Album> GetAllReqursive();

        Album GetById(Guid id);

        void Update(Album album);

        void Remove(Guid id);

        void UpdateCoverImage(Guid album, Guid image);

        string GenerateZipArchive(Guid id);
    }
}