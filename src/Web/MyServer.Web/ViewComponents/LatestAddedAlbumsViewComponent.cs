using Microsoft.AspNetCore.Mvc;
using MyServer.Services.ImageGallery;
using MyServer.Services.Mappings;
using MyServer.Web.Models.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyServer.Web.ViewComponents
{
    public class LatestAddedAlbumsViewComponent : ViewComponent
    {
        private readonly IAlbumService albumService;

        public LatestAddedAlbumsViewComponent(IAlbumService albumService)
        {
            this.albumService = albumService;
        }

        public IViewComponentResult Invoke()
        {
            IQueryable<HomeAlbumViewModel> albums = null;

            if (!this.User.Identity.IsAuthenticated)
            {
                albums =
                    this.albumService.GetAllReqursive()
                        .Where(x => x.Access == Common.MyServerAccessType.Public)
                        .To<HomeAlbumViewModel>();
            }
            else if (this.User.IsInRole(Common.MyServerRoles.User.ToString()))
            {
                albums =
                    this.albumService.GetAllReqursive()
                        .Where(x => x.Access != Common.MyServerAccessType.Private)
                        .To<HomeAlbumViewModel>();
            }
            else if (this.User.IsInRole(Common.MyServerRoles.Admin.ToString()))
            {
                albums = this.albumService.GetAllReqursive().To<HomeAlbumViewModel>();
            }

            return View(albums);
        }
    }
}
