namespace MyServer.Web.Main.Areas.ImageGalleryAdmin.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;

    using MyServer.Data.Models.ImageGallery;
    using MyServer.Services.ImageGallery;
    using MyServer.Services.Users;
    using MyServer.Web.Infrastructure.Mappings;
    using MyServer.Web.Main.Areas.ImageGalleryAdmin.Models.Album;

    public class AlbumController : BaseController
    {
        private readonly IAlbumService albumService;

        public AlbumController(IUserService userService, IAlbumService albumService)
            : base(userService)
        {
            this.albumService = albumService;
        }

        public ActionResult Add()
        {
            return this.View(new AddAlbumViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(AddAlbumViewModel model)
        {
            if (this.ModelState.IsValid && model != null)
            {
                var album = new Album()
                                {
                                    Title = model.Title, 
                                    Description = model.Description,
                                    CreatedOn = DateTime.UtcNow
                                };

                this.albumService.Add(album);
                return this.RedirectToAction("Index");
            }

            return this.View(model);
        }

        public ActionResult Details(string id)
        {
            this.Session["AlbumId"] = id;
            var intId = Guid.Parse(id);
            var result = this.albumService.GetAll().Where(x => x.Id == intId).To<AlbumDetailsViewModel>().FirstOrDefault();

            if (result == null)
            {
                return this.HttpNotFound("Album not found");
            }

            return this.View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(AlbumDetailsViewModel model)
        {
            if (this.ModelState.IsValid && model != null)
            {
                var album = this.albumService.GetById(model.Id);

                if (album == null)
                {
                    return this.HttpNotFound();
                }

                album.Title = model.Title;
                album.Description = model.Description;

                this.albumService.Update(album);

                return this.RedirectToAction("Details", album.Id);
            }

            return this.View(model);
        }

        public ActionResult Index()
        {
            return this.View();
        }
    }
}