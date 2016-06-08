namespace ImageGallery.Web.Areas.Admin.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;

    using ImageGallery.Data.Models;
    using ImageGallery.Services.Album;
    using ImageGallery.Services.User;
    using ImageGallery.Web.Areas.Admin.Models.Album;
    using ImageGallery.Web.Infrastructure.Mappings;

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
            return this.View(new AddAlbumViewModel() { Date = DateTime.Today, Title = string.Empty });
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
            return this.View(result);
        }

        public ActionResult Index()
        {
            return this.View();
        }
    }
}