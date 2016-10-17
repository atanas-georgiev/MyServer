namespace MyServer.Web.Areas.ImageGalleryAdmin.Controllers
{
    using System;
    using System.Linq;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using MyServer.Common;
    using MyServer.Data;
    using MyServer.Data.Models;
    using MyServer.Services.ImageGallery;
    using MyServer.Services.Mappings;
    using MyServer.Services.Users;
    using MyServer.Web.Areas.ImageGalleryAdmin.Models.Album;
    using MyServer.Web.Areas.ImageGalleryAdmin.Models.Image;
    using MyServer.Web.Areas.Shared.Controllers;

    [Authorize(Roles = MyServerRoles.Admin)]
    [Area("ImageGalleryAdmin")]
    public class AlbumController : BaseController
    {
        private readonly IAlbumService albumService;

        private readonly IFileService fileService;

        private readonly IImageService imageService;

        private readonly ILocationService locationService;

        public AlbumController(
            IAlbumService albumService,
            ILocationService locationService,
            IImageService imageService,
            IUserService userService,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            MyServerDbContext dbContext,
            IFileService fileService)
            : base(userService, userManager, signInManager, dbContext)
        {
            this.albumService = albumService;
            this.locationService = locationService;
            this.imageService = imageService;
            this.fileService = fileService;
        }

        public IActionResult Delete(Guid id)
        {
            var album = this.albumService.GetById(id);

            if (album != null)
            {
                if (album.Images != null)
                {
                    foreach (var image in album.Images)
                    {
                        this.imageService.Remove(image.Id);
                    }
                }

                this.albumService.Remove(album.Id);
            }

            return this.RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public PartialViewResult AlbumDataPartial(AlbumEditViewModel model)
        {
            if (this.ModelState.IsValid && model != null)
            {
                var album = this.albumService.GetById(model.Id);

                album.TitleBg = model.TitleBg;
                album.TitleEn = model.TitleEn;
                album.DescriptionBg = model.DescriptionBg;
                album.DescriptionEn = model.DescriptionEn;
                album.Access = model.Access;

                this.albumService.Update(album);

                var result = this.albumService.GetAll().Where(a => a.Id == album.Id).To<AlbumEditViewModel>().First();

                return this.PartialView("_AlbumDataPartial", result);
            }

            return this.PartialView("_AlbumDataPartial", model);
        }

        public IActionResult Create()
        {
            return this.View(new AddAlbumViewModel() { Access = MyServerAccessType.Private });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AddAlbumViewModel model)
        {
            if (this.ModelState.IsValid && model != null)
            {
                var album = new Album()
                                {
                                    TitleBg = model.TitleBg,
                                    TitleEn = model.TitleEn,
                                    DescriptionBg = model.DescriptionBg,
                                    DescriptionEn = model.DescriptionEn,
                                    CreatedOn = DateTime.UtcNow,
                                    AddedBy = this.UserProfile,
                                    Access = model.Access,
                                    Cover = this.imageService.GetAll().OrderBy(x => x.CreatedOn).First()
                                };

                this.albumService.Add(album);
                return this.RedirectToAction("Edit", new { id = album.Id });
            }

            return this.View(model);
        }

        [HttpPost]
        public IActionResult DeleteImages(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var ids = id.Split(',');
                var imageId = Guid.Parse(ids.First());
                var albumId = this.imageService.GetById(imageId).AlbumId;

                foreach (var item in ids)
                {
                    this.imageService.Remove(Guid.Parse(item));
                }

                var album =
                    this.albumService.GetAllReqursive().Where(x => x.Id == albumId).To<AlbumEditViewModel>().First();
                return this.PartialView("_ImageListPartial", album);
            }

            return this.Content(string.Empty);
        }

        [HttpPost]
        public IActionResult RotateImagesLeft(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var ids = id.Split(',');
                var imageId = Guid.Parse(ids.First());
                var albumId = this.imageService.GetById(imageId).AlbumId;

                foreach (var item in ids)
                {
                    this.imageService.Rotate(Guid.Parse(item), MyServerRotateType.Left);
                }

                var album =
                    this.albumService.GetAllReqursive().Where(x => x.Id == albumId).To<AlbumEditViewModel>().First();
                return this.PartialView("_ImageListPartial", album);
            }

            return this.Content(string.Empty);
        }

        [HttpPost]
        public IActionResult RotateImagesRight(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var ids = id.Split(',');
                var imageId = Guid.Parse(ids.First());
                var albumId = this.imageService.GetById(imageId).AlbumId;

                foreach (var item in ids)
                {
                    this.imageService.Rotate(Guid.Parse(item), MyServerRotateType.Right);
                }

                var album =
                    this.albumService.GetAllReqursive().Where(x => x.Id == albumId).To<AlbumEditViewModel>().First();
                return this.PartialView("_ImageListPartial", album);
            }

            return this.Content(string.Empty);
        }

        public IActionResult Edit(string id)
        {
            this.Response.Cookies.Append("AlbumId", id);
            var intId = Guid.Parse(id);
            var result =
                this.albumService.GetAllReqursive().Where(x => x.Id == intId).To<AlbumEditViewModel>().FirstOrDefault();

            if (result == null)
            {
                return this.NotFound("Album not found");
            }

            return this.View(result);
        }

        public IActionResult Index()
        {
            var albums = this.albumService.GetAllReqursive().To<AlbumListViewModel>().ToList();
            return this.View(albums);
        }

        [HttpPost]
        public IActionResult UpdateAlbumCover(string id)
        {
            var albumId = Guid.Parse(this.Request.Cookies["AlbumId"]);
            var coverId = Guid.Parse(id);

            this.albumService.UpdateCoverImage(albumId, coverId);
            return this.Content(string.Empty);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateImageDate(ImageUpdateViewModel model)
        {
            if (model != null && !string.IsNullOrEmpty(model.Items))
            {
                var ids = model.Items.Split(',');

                foreach (var id in ids)
                {
                    var image = this.imageService.GetById(Guid.Parse(id));
                    var date = DateTime.Parse(model.Data);
                    image.DateTaken = date;
                    this.imageService.Update(image);
                }

                var albumId = Guid.Parse(this.Request.Cookies["AlbumId"]);
                var album =
                    this.albumService.GetAllReqursive().Where(x => x.Id == albumId).To<AlbumEditViewModel>().First();
                return this.PartialView("_ImageListPartial", album);
            }

            return this.Content(string.Empty);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateImageLocation(ImageUpdateViewModel model)
        {
            if (model != null && !string.IsNullOrEmpty(model.Items))
            {
                var ids = model.Items.Split(',');
                var gpsData = this.locationService.GetGpsData(model.Data).Result;

                foreach (var id in ids)
                {
                    this.imageService.AddGpsDataToImage(Guid.Parse(id), gpsData);
                }

                var imageId = Guid.Parse(ids.First());
                var albumId = this.imageService.GetById(imageId).AlbumId;
                var album = this.albumService.GetAll().Where(x => x.Id == albumId).To<AlbumEditViewModel>().First();

                return this.PartialView("_ImageListPartial", album);
            }

            return this.Content(string.Empty);
        }

        [HttpPost]
        public PartialViewResult UpdateImages()
        {
            var albumId = Guid.Parse(this.Request.Cookies["AlbumId"]);
            var album = this.albumService.GetAllReqursive().Where(x => x.Id == albumId).To<AlbumEditViewModel>().First();
            return this.PartialView("_ImageListPartial", album);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateImageTitle(ImageUpdateViewModel model)
        {
            if (model != null && !string.IsNullOrEmpty(model.Items))
            {
                var ids = model.Items.Split(',');

                foreach (var id in ids)
                {
                    var image = this.imageService.GetById(Guid.Parse(id));
                    image.Title = model.Data;
                    this.imageService.Update(image);
                }

                var albumId = Guid.Parse(this.Request.Cookies["AlbumId"]);
                var album =
                    this.albumService.GetAllReqursive().Where(x => x.Id == albumId).To<AlbumEditViewModel>().First();
                return this.PartialView("_ImageListPartial", album);
            }

            return this.Content(string.Empty);
        }
    }
}