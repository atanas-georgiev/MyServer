namespace MyServer.Web.Api.Areas.ImageGallery.Controllers
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;
    using System.Web.Http.OData;

    using Microsoft.AspNet.Identity;

    using MyServer.Common;
    using MyServer.Common.ImageGallery;
    using MyServer.Data.Models;
    using MyServer.Services.ImageGallery;
    using MyServer.Services.Users;
    using MyServer.Web.Api.Areas.ImageGallery.Models;
    using MyServer.Web.Infrastructure.Mappings;

    public class AlbumsController : BaseController
    {
        private readonly IAlbumService albumService;

        private readonly ILocationService locationService;

        private readonly IImageService imageService;

        public AlbumsController(
                IAlbumService albumService,
                ILocationService locationService,
                IImageService imageService)
        {
            this.albumService = albumService;
            this.locationService = locationService;
            this.imageService = imageService;
        }

        [Route("ImageGallery/Albums")]
        [EnableQuery]
        [ResponseType(typeof(IQueryable<AlbumBindingModel>))]
        public IHttpActionResult Get()
        {
            try
            {
                if (!this.User.Identity.IsAuthenticated)
                {
                    return this.Ok(this.albumService.GetAll().Where(x => x.AccessType == AccessType.Public).To<AlbumBindingModel>());
                }

                if (this.User.IsInRole(MyServerRoles.User))
                {
                    return this.Ok(this.albumService.GetAll().Where(x => x.AccessType != AccessType.Private).To<AlbumBindingModel>());
                }

                if (this.User.IsInRole(MyServerRoles.Admin))
                {
                    return this.Ok(this.albumService.GetAll().To<AlbumBindingModel>());
                }

                return this.NotFound();
            }
            catch (Exception ex)
            {
                return this.InternalServerError(ex);
            }
        }

        [Route("ImageGallery/Albums/{id}")]
        [ResponseType(typeof(AlbumBindingModel))]
        public IHttpActionResult Get(string id)
        {
            try
            {
                var albumId = Guid.Parse(id);
                var album =
                    this.albumService.GetAll()
                        .Where(x => x.Id == albumId)
                        .To<AlbumBindingModel>()
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

        [Route("ImageGallery/Albums")]
        [Authorize(Roles = MyServerRoles.Admin)]
        [ResponseType(typeof(AlbumBindingModel))]
        public IHttpActionResult Post([FromBody]AlbumBindingModel model)
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
                    CreatedOn = DateTime.Now,
                    AddedById = this.User.Identity.GetUserId(),
                    AccessType = model.AccessType,
                    Cover = this.imageService.GetAll().First()
                };

                this.albumService.Add(album);
                model = this.albumService.GetAll().Where(x => x.Id == album.Id).To<AlbumBindingModel>().First();

                return this.Created<AlbumBindingModel>(this.Request.RequestUri + model.Id.ToString(), model);
            }
            catch (Exception ex)
            {
                return this.InternalServerError(ex);
            }
        }

        [Route("ImageGallery/Albums/{id}")]
        [Authorize(Roles = MyServerRoles.Admin)]
        [ResponseType(typeof(AlbumBindingModel))]
        public IHttpActionResult Put(string id, [FromBody]AlbumBindingModel model)
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

                var albumId = Guid.Parse(id);
                var album = this.albumService.GetById(albumId);

                if (album == null)
                {
                    return this.NotFound();
                }

                album.Title = model.Title;
                album.Description = model.Description;
                album.AccessType = model.AccessType;

                this.albumService.Update(album);
                model = this.albumService.GetAll().Where(x => x.Id == album.Id).To<AlbumBindingModel>().First();

                return this.Ok(model);
            }
            catch (Exception ex)
            {
                return this.InternalServerError(ex);
            }
        }

        [Route("ImageGallery/Albums/{id}")]
        [Authorize(Roles = MyServerRoles.Admin)]
        public IHttpActionResult Delete(string id)
        {
            try
            {
                var albumId = Guid.Parse(id);
                var album = this.albumService.GetById(albumId);

                if (album == null)
                {
                    return this.NotFound();
                }

                this.albumService.Remove(albumId);
                
                return this.Ok();
            }
            catch (Exception ex)
            {
                return this.InternalServerError(ex);
            }
        }
    }
}
