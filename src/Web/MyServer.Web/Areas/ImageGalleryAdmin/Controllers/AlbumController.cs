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

    [Authorize(Roles = "Admin")]
    [Area("ImageGalleryAdmin")]
    public class AlbumController : BaseController
    {
        private static int statusDataCurrentIndex;

        private static bool statusDataError = false;

        private static int statusDataLength;

        private static bool statusIsStared = false;

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
            MyServerDbContext dbcontext,
            IFileService fileService)
            : base(userService, userManager, signInManager, dbcontext)
        {
            this.albumService = albumService;
            this.locationService = locationService;
            this.imageService = imageService;
            this.fileService = fileService;
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

                var result =
                    this.albumService.GetAllReqursive().Where(a => a.Id == album.Id).To<AlbumEditViewModel>().First();

                return this.PartialView("_AlbumDataPartial", result);
            }

            return this.PartialView("_AlbumDataPartial", model);
        }

        public IActionResult Create()
        {
            return this.View(new AddAlbumViewModel() { Access = MyServerAccessType.Private });
        }

        [HttpPost]
        public IActionResult Create(AddAlbumViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var album = new Album()
                                {
                                    TitleBg = model.TitleBg,
                                    TitleEn = model.TitleEn,
                                    DescriptionBg = model.DescriptionBg,
                                    DescriptionEn = model.DescriptionEn,
                                    CreatedOn = DateTime.UtcNow,
                                    AddedBy = this.UserProfile,
                                    Access = model.Access
                                };

                this.albumService.Add(album);
                return this.RedirectToAction(nameof(this.Edit), new { id = album.Id });
            }

            return this.View(model);
        }

        public IActionResult Delete(Guid id)
        {
            this.albumService.Remove(id);
            return this.RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteImages(ImageUpdateViewModel model)
        {
            try
            {
                if (model != null)
                {
                    statusIsStared = true;
                    statusDataError = false;
                    statusDataLength = 0;
                    statusDataCurrentIndex = 0;

                    var ids = model.Items.Split(',');

                    for (var i = 0; i < ids.Length; i++)
                    {
                        statusDataLength = ids.Length;
                        statusDataCurrentIndex = i;
                        this.imageService.Remove(Guid.Parse(ids[i]));
                    }

                    statusIsStared = false;
                    var imageId = Guid.Parse(ids.First());
                    var albumId = this.imageService.GetById(imageId).AlbumId;
                    var album =
                        this.albumService.GetAllReqursive().Where(x => x.Id == albumId).To<AlbumEditViewModel>().First();

                    return this.PartialView("_ImageListPartial", album);
                }

                statusDataError = true;
                return this.NoContent();
            }
            catch (Exception)
            {
                statusDataError = true;
                return this.NoContent();
            }
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

        public IActionResult GetOperationStatus()
        {
            var percentage = 0;
            if (statusDataLength != 0)
            {
                percentage = ((statusDataCurrentIndex + 1) * 100) / statusDataLength;
            }

            return this.Json(new { started = statusIsStared, status = percentage, error = statusDataError });
        }

        public IActionResult Index()
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult RotateImagesLeft(ImageUpdateViewModel model)
        {
            try
            {
                if (model != null)
                {
                    statusIsStared = true;
                    statusDataError = false;
                    statusDataLength = 0;
                    statusDataCurrentIndex = 0;

                    var ids = model.Items.Split(',');

                    for (var i = 0; i < ids.Length; i++)
                    {
                        statusDataLength = ids.Length;
                        statusDataCurrentIndex = i;
                        this.imageService.Rotate(Guid.Parse(ids[i]), MyServerRotateType.Left);
                    }

                    statusIsStared = false;
                    var imageId = Guid.Parse(ids.First());
                    var albumId = this.imageService.GetById(imageId).AlbumId;
                    var album =
                        this.albumService.GetAllReqursive().Where(x => x.Id == albumId).To<AlbumEditViewModel>().First();

                    return this.PartialView("_ImageListPartial", album);
                }

                statusDataError = true;
                return this.NoContent();
            }
            catch (Exception)
            {
                statusDataError = true;
                return this.NoContent();
            }
        }

        [HttpPost]
        public IActionResult RotateImagesRight(ImageUpdateViewModel model)
        {
            try
            {
                if (model != null)
                {
                    statusIsStared = true;
                    statusDataError = false;
                    statusDataLength = 0;
                    statusDataCurrentIndex = 0;

                    var ids = model.Items.Split(',');

                    for (var i = 0; i < ids.Length; i++)
                    {
                        statusDataLength = ids.Length;
                        statusDataCurrentIndex = i;
                        this.imageService.Rotate(Guid.Parse(ids[i]), MyServerRotateType.Right);
                    }

                    statusIsStared = false;
                    var imageId = Guid.Parse(ids.First());
                    var albumId = this.imageService.GetById(imageId).AlbumId;
                    var album =
                        this.albumService.GetAllReqursive().Where(x => x.Id == albumId).To<AlbumEditViewModel>().First();

                    return this.PartialView("_ImageListPartial", album);
                }

                statusDataError = true;
                return this.NoContent();
            }
            catch (Exception)
            {
                statusDataError = true;
                return this.NoContent();
            }
        }

        [HttpPost]
        public IActionResult UpdateAlbumCover(ImageUpdateViewModel model)
        {
            if (model != null)
            {
                var albumId = Guid.Parse(this.Request.Cookies["AlbumId"]);
                var coverId = Guid.Parse(model.Items);
                this.albumService.UpdateCoverImage(albumId, coverId);

                var album =
                    this.albumService.GetAllReqursive().Where(x => x.Id == albumId).To<AlbumEditViewModel>().First();

                return this.PartialView("_ImageListPartial", album);
            }

            return this.NoContent();
        }

        [HttpPost]
        public IActionResult UpdateImageDate(ImageUpdateViewModel model)
        {
            try
            {
                if (model != null)
                {
                    statusIsStared = true;
                    statusDataError = false;
                    statusDataLength = 0;
                    statusDataCurrentIndex = 0;

                    var ids = model.Items.Split(',');

                    for (var i = 0; i < ids.Length; i++)
                    {
                        statusDataLength = ids.Length;
                        statusDataCurrentIndex = i;
                        this.imageService.UpdateDateTaken(Guid.Parse(ids[i]), DateTime.Parse(model.Data));
                    }

                    statusIsStared = false;
                    var imageId = Guid.Parse(ids.First());
                    var albumId = this.imageService.GetById(imageId).AlbumId;
                    var album =
                        this.albumService.GetAllReqursive().Where(x => x.Id == albumId).To<AlbumEditViewModel>().First();

                    return this.PartialView("_ImageListPartial", album);
                }

                statusDataError = true;
                return this.NoContent();
            }
            catch (Exception)
            {
                statusDataError = true;
                return this.NoContent();
            }
        }

        [HttpPost]
        public IActionResult UpdateImageLocation(ImageUpdateViewModel model)
        {
            try
            {
                if (model != null)
                {
                    statusIsStared = true;
                    statusDataError = false;
                    statusDataLength = 0;
                    statusDataCurrentIndex = 0;

                    var ids = model.Items.Split(',');
                    var gpsData = this.locationService.GetGpsData(model.Data).Result;

                    for (var i = 0; i < ids.Length; i++)
                    {
                        statusDataLength = ids.Length;
                        statusDataCurrentIndex = i;
                        this.imageService.AddGpsDataToImage(Guid.Parse(ids[i]), gpsData);
                    }

                    statusIsStared = false;
                    var imageId = Guid.Parse(ids.First());
                    var albumId = this.imageService.GetById(imageId).AlbumId;
                    var album =
                        this.albumService.GetAllReqursive().Where(x => x.Id == albumId).To<AlbumEditViewModel>().First();

                    return this.PartialView("_ImageListPartial", album);
                }

                statusDataError = true;
                return this.NoContent();
            }
            catch (Exception)
            {
                statusDataError = true;
                return this.NoContent();
            }
        }

        [HttpPost]
        public PartialViewResult UpdateImages()
        {
            var albumId = Guid.Parse(this.Request.Cookies["AlbumId"]);
            var album = this.albumService.GetAllReqursive().Where(x => x.Id == albumId).To<AlbumEditViewModel>().First();
            return this.PartialView("_ImageListPartial", album);
        }

        [HttpPost]
        public IActionResult UpdateImageTitle(ImageUpdateViewModel model)
        {
            try
            {
                if (model != null)
                {
                    statusIsStared = true;
                    statusDataError = false;
                    statusDataLength = 0;
                    statusDataCurrentIndex = 0;

                    var ids = model.Items.Split(',');

                    for (var i = 0; i < ids.Length; i++)
                    {
                        statusDataLength = ids.Length;
                        statusDataCurrentIndex = i;

                        var image = this.imageService.GetById(Guid.Parse(ids[i]));
                        image.Title = model.Data;
                        this.imageService.Update(image);
                    }

                    statusIsStared = false;
                    var imageId = Guid.Parse(ids.First());
                    var albumId = this.imageService.GetById(imageId).AlbumId;
                    var album =
                        this.albumService.GetAllReqursive().Where(x => x.Id == albumId).To<AlbumEditViewModel>().First();

                    return this.PartialView("_ImageListPartial", album);
                }

                statusDataError = true;
                return this.NoContent();
            }
            catch (Exception)
            {
                statusDataError = true;
                return this.NoContent();
            }
        }
    }
}