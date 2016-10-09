namespace MyServer.Services.ImageGallery
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;

    using MyServer.Common.ImageGallery;
    using MyServer.Data.Common;
    using MyServer.Data.Models;

    public class AlbumService : IAlbumService
    {
        private readonly IRepository<Album, Guid> albums;

        private readonly IHostingEnvironment appEnvironment;

        private readonly IFileService fileService;

        private readonly IRepository<Image, Guid> images;

        public AlbumService(
            IRepository<Album, Guid> albums,
            IRepository<Image, Guid> images,
            IHostingEnvironment appEnvironment,
            IFileService fileService)
        {
            this.albums = albums;
            this.images = images;
            this.fileService = fileService;
            this.appEnvironment = appEnvironment;
        }

        public void Add(Album album)
        {
            this.albums.Add(album);
            this.fileService.CreateInitialFolders(album.Id);
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

            var archiveName = this.appEnvironment.WebRootPath + Constants.TempContentFolder + "\\"
                              + this.fileService.MakeValidFileName(album.Title) + ".zip";
            File.Delete(archiveName);
            ZipFile.CreateFromDirectory(albumPathTemp, archiveName);

            return archiveName.Replace(
                this.appEnvironment.WebRootPath + Constants.TempContentFolder,
                Constants.TempContentFolder);
        }

        public IQueryable<Album> GetAll()
        {
            var firstAlbumToExcludeGuid = Guid.Parse(Constants.NoCoverId);
            return this.albums.All().Where(x => x.Id != firstAlbumToExcludeGuid);
        }

        public IQueryable<Album> GetAllReqursive()
        {
            var firstAlbumToExcludeGuid = Guid.Parse(Constants.NoCoverId);
            return
                this.albums.All()
                    .Include(x => x.Cover)
                    .Include(x => x.Images).ThenInclude(x => x.ImageGpsData)
                    .Where(x => x.Id != firstAlbumToExcludeGuid);
        }

        public Album GetById(Guid id)
        {
            return this.GetAllReqursive().FirstOrDefault(x => x.Id == id);
        }

        public void Remove(Guid id)
        {
            var album = this.GetById(id);
            this.albums.Delete(id);
        }

        public void Update(Album album)
        {
            this.albums.Update(album);
        }

        public void UpdateCoverImage(Guid album, Guid image)
        {
            var albumDb = this.GetById(album);
            albumDb.CoverId = image;
            this.Update(albumDb);
        }
    }
}