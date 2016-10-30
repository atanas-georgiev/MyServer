namespace MyServer.Services.ImageGallery
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Memory;

    using MyServer.Common.ImageGallery;
    using MyServer.Data.Common;
    using MyServer.Data.Models;

    public class AlbumService : IAlbumService
    {
        private readonly IRepository<Album, Guid> albums;

        private readonly IHostingEnvironment appEnvironment;

        private readonly IFileService fileService;

        private readonly IRepository<Image, Guid> images;

        private readonly IMemoryCache memoryCache;

        public AlbumService(
            IRepository<Album, Guid> albums,
            IRepository<Image, Guid> images,
            IHostingEnvironment appEnvironment,
            IFileService fileService,
            IMemoryCache memoryCache)
        {
            this.albums = albums;
            this.images = images;
            this.fileService = fileService;
            this.appEnvironment = appEnvironment;
            this.memoryCache = memoryCache;
        }

        public void Add(Album album)
        {
            var noCoverImageGuid = Guid.Parse(Constants.NoCoverId);
            album.Cover = this.images.GetById(noCoverImageGuid);
            this.albums.Add(album);
            this.fileService.CreateInitialFolders(album.Id);
            this.memoryCache.Remove(CacheKeys.AlbumsServiceCacheKey);
            this.memoryCache.Remove(CacheKeys.ImageServiceCacheKey);
            this.memoryCache.Remove(CacheKeys.FileServiceCacheKey);
        }

        public string GenerateZipArchive(Guid id, ImageType type)
        {
            var number = 0;
            var album = this.GetById(id);
            var files =
                album.Images.OrderBy(x => x.OriginalFileName)
                    .Select(x => new { id = number++, FileName = x.FileName, OriginalFileName = x.OriginalFileName })
                    .ToList();
            var duplicates = files.GroupBy(s => s.OriginalFileName).SelectMany(grp => grp.Skip(1)).ToList();
            string albumPath = string.Empty;

            switch (type)
            {
                case ImageType.Low:
                    albumPath = this.appEnvironment.WebRootPath + Constants.MainContentFolder + "\\" + id + "\\"
                                + Constants.ImageFolderLow + "\\";
                    break;
                case ImageType.Medium:
                    albumPath = this.appEnvironment.WebRootPath + Constants.MainContentFolder + "\\" + id + "\\"
                                + Constants.ImageFolderMiddle + "\\";
                    break;
                case ImageType.Original:
                    albumPath = this.appEnvironment.WebRootPath + Constants.MainContentFolder + "\\" + id + "\\"
                                + Constants.ImageFolderOriginal + "\\";
                    break;
            }

            var albumPathTemp = this.appEnvironment.WebRootPath + Constants.TempContentFolder + "\\" + id + "\\";

            this.fileService.EmptyTempFolder();
            Directory.CreateDirectory(this.appEnvironment.WebRootPath + Constants.TempContentFolder + "\\" + id);

            foreach (var file in files)
            {
                if (!duplicates.Exists(x => x.OriginalFileName == file.OriginalFileName))
                {
                    File.Copy(albumPath + file.FileName, albumPathTemp + file.OriginalFileName);
                }
                else
                {
                    var newFileName = Path.GetFileNameWithoutExtension(file.OriginalFileName) + "_" + file.id
                                      + Path.GetExtension(file.OriginalFileName);
                    File.Copy(albumPath + file.FileName, albumPathTemp + newFileName);
                }
            }

            // TODO: change en/bg
            var archiveName = this.appEnvironment.WebRootPath + Constants.TempContentFolder + "\\"
                              + this.fileService.MakeValidFileName(album.TitleEn) + ".zip";
            File.Delete(archiveName);
            ZipFile.CreateFromDirectory(albumPathTemp, archiveName);

            return archiveName.Replace(
                this.appEnvironment.WebRootPath + Constants.TempContentFolder,
                Constants.TempContentFolder);
        }

        public IQueryable<Album> GetAllReqursive()
        {
            IQueryable<Album> result = null;

            if (!this.memoryCache.TryGetValue(CacheKeys.AlbumsServiceCacheKey, out result))
            {
                // fetch the value from the source
                var firstAlbumToExcludeGuid = Guid.Parse(Constants.NoCoverId);
                result =
                    this.albums.All()
                        .Include(x => x.Cover)
                        .Include(x => x.Images)
                        .ThenInclude(x => x.ImageGpsData)
                        .Where(x => x.IsDeleted == false && x.Id != firstAlbumToExcludeGuid)
                        .ToList()
                        .AsQueryable();

                // store in the cache
                this.memoryCache.Set(
                    CacheKeys.AlbumsServiceCacheKey,
                    result,
                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromDays(365)));
            }

            return result;
        }

        public Album GetById(Guid id)
        {
            return this.GetAllReqursive().FirstOrDefault(x => x.Id == id);
        }

        public void Remove(Guid id)
        {
            var album = this.GetById(id);

            if (album != null)
            {
                if (album.Images != null)
                {
                    foreach (var image in album.Images.ToList())
                    {
                        this.images.Delete(image.Id);
                    }
                }

                this.albums.Delete(id);
                this.fileService.RemoveAlbum(id);

                this.memoryCache.Remove(CacheKeys.AlbumsServiceCacheKey);
                this.memoryCache.Remove(CacheKeys.ImageServiceCacheKey);
                this.memoryCache.Remove(CacheKeys.FileServiceCacheKey);
            }
        }

        public void Update(Album album)
        {
            this.albums.Update(album);
            this.memoryCache.Remove(CacheKeys.AlbumsServiceCacheKey);
            this.memoryCache.Remove(CacheKeys.ImageServiceCacheKey);
            this.memoryCache.Remove(CacheKeys.FileServiceCacheKey);
        }

        public void UpdateCoverImage(Guid album, Guid image)
        {
            var albumDb = this.GetById(album);
            albumDb.CoverId = image;
            this.Update(albumDb);
            this.memoryCache.Remove(CacheKeys.AlbumsServiceCacheKey);
            this.memoryCache.Remove(CacheKeys.ImageServiceCacheKey);
            this.memoryCache.Remove(CacheKeys.FileServiceCacheKey);
        }
    }
}