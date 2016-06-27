using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MyServer.Web.Api.Areas.ImageGallery.Controllers
{
    using System.Web.Http.Description;

    using Microsoft.AspNet.Identity;

    using MyServer.Common;
    using MyServer.Common.ImageGallery;
    using MyServer.Data.Models;
    using MyServer.Services.ImageGallery;
    using MyServer.Services.Users;
    using MyServer.Web.Api.Areas.ImageGallery.Models;
    using MyServer.Web.Infrastructure.Mappings;

    public class AlbumController : BaseController
    {
        private readonly IAlbumService albumService;

        private readonly ILocationService locationService;

        private readonly IImageService imageService;

        public AlbumController(
                IAlbumService albumService,
                ILocationService locationService,
                IImageService imageService)
        {
            this.albumService = albumService;
            this.locationService = locationService;
            this.imageService = imageService;
        }

        [Route("ImageGallery/Album")]
        [ResponseType(typeof(IEnumerable<AlbumListBindingModel>))]
        public IHttpActionResult Get()
        {
            try
            {
                if (!this.User.Identity.IsAuthenticated)
                {
                    return this.Ok(this.albumService.GetAll().Where(x => x.AccessType == AccessType.Public).To<AlbumListBindingModel>());
                }

                if (this.User.IsInRole(MyServerRoles.Admin))
                {
                    return this.Ok(this.albumService.GetAll().To<AlbumListBindingModel>());
                }

                if (this.User.IsInRole(MyServerRoles.User))
                {
                    return this.Ok(this.albumService.GetAll().Where(x => x.AccessType == AccessType.Shared).To<AlbumListBindingModel>());
                }

                return this.NotFound();
            }
            catch (Exception ex)
            {
                return this.InternalServerError(ex);
            }
        }

        [Route("ImageGallery/Album/{id}")]
        [ResponseType(typeof(AlbumDetailsBindingModel))]
        public IHttpActionResult Get(string id)
        {
            try
            {
                var albumId = Guid.Parse(id);
                var album =
                    this.albumService.GetAll()
                        .Where(x => x.Id == albumId)
                        .To<AlbumDetailsBindingModel>()
                        .FirstOrDefault();

                if (album == null)
                {
                    return this.NotFound();
                }

                if (!this.User.Identity.IsAuthenticated && album.AccessType != AccessType.Public)
                {
                    return this.Unauthorized();
                }

                if (this.User.IsInRole(MyServerRoles.User) && album.AccessType == AccessType.Private)
                {
                    return this.Unauthorized();
                }

                return this.Ok(album);
            }
            catch (Exception ex)
            {
                return this.InternalServerError(ex);
            }
        }

        [Authorize(Roles = MyServerRoles.Admin)]
        [Route("ImageGallery/Album")]
        [ResponseType(typeof(AlbumAddBindingModel))]
        public IHttpActionResult Post([FromBody]AlbumAddBindingModel model)
        {
            try
            {
                if (model == null)
                {
                    return this.BadRequest("Album cannot be null");
                }

                if (!this.ModelState.IsValid)
                {
                    return this.BadRequest(this.ModelState);
                }

                var album = new Album()
                {
                    Title = model.Title,
                    Description = model.Description,
                    CreatedOn = DateTime.UtcNow,
                    AddedById = this.User.Identity.GetUserId(),
                    AccessType = model.AccessType,
                    Cover = this.imageService.GetAll().First()
                };

                this.albumService.Add(album);
                model = this.albumService.GetAll().Where(x => x.Id == album.Id).To<AlbumAddBindingModel>().First();

                return this.Created<AlbumAddBindingModel>(this.Request.RequestUri + model.Id.ToString(), model);
            }
            catch (Exception ex)
            {
                return this.InternalServerError(ex);
            }
        }

        // PUT: api/Album/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Album/5
        public void Delete(int id)
        {
        }
    }
}
