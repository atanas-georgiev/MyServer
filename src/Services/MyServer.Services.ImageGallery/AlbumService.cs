namespace MyServer.Services.ImageGallery
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Web;

    using MyServer.Common.ImageGallery;
    using MyServer.Data.Common;
    using MyServer.Data.Models.ImageGallery;

    using System.IO;
    using System.IO.Compression;

    public class AlbumService : IAlbumService
    {
        private IRepository<Album, Guid> albums;

        public AlbumService(IRepository<Album, Guid> albums)
        {
            this.albums = albums;
        }

        public void Add(Album album)
        {
            this.albums.Add(album);
        }

        public IQueryable<Album> GetAll()
        {
            return this.albums.All();
        }

        public Album GetById(Guid id)
        {
            return this.GetAll().FirstOrDefault(x => x.Id == id);
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

        public string GenerateZipArchive(Guid id, HttpServerUtilityBase server)
        {
            var number = 0;
            var album = this.GetById(id);
            var files = album.Images.OrderBy(x => x.OriginalFileName).Select(x => new { id = number++, FileName = x.FileName, OriginalFileName = x.OriginalFileName }).ToList();
            var duplicates = files.GroupBy(s => s.OriginalFileName).SelectMany(grp => grp.Skip(1)).ToList();
            var albumPathOriginal = server.MapPath(Constants.MainContentFolder + "\\" + id + "\\" + Constants.ImageFolderOriginal + "\\");
            var albumPathTemp = server.MapPath(Constants.TempContentFolder + "\\" + id + "\\");

            FileService.EmptyTempFolder(server);
            Directory.CreateDirectory(server.MapPath(Constants.TempContentFolder + "\\" + id));

            foreach (var file in files)
            {
                if (!duplicates.Exists(x => x.OriginalFileName == file.OriginalFileName))
                {
                    File.Copy(albumPathOriginal + file.FileName, albumPathTemp + file.OriginalFileName);
                }
                else
                {
                    var newFileName = Path.GetFileNameWithoutExtension(file.OriginalFileName) + "_" + file.id
                                      + Path.GetExtension(file.OriginalFileName);
                    File.Copy(albumPathOriginal + file.FileName, albumPathTemp + newFileName);
                }
            }

            var archiveName = server.MapPath(Constants.TempContentFolder + "\\" + FileService.MakeValidFileName(album.Title) + ".zip");
            File.Delete(archiveName);
            ZipFile.CreateFromDirectory(albumPathTemp, archiveName);

            return archiveName.Replace(server.MapPath(Constants.TempContentFolder), Constants.TempContentFolder);
        }
    }
}