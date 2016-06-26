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
    using MyServer.Data.Models;
    using MyServer.Services.ImageGallery;
    using MyServer.Services.Users;
    using MyServer.Web.Api.Areas.ImageGallery.Models;
    using MyServer.Web.Infrastructure.Mappings;

    public class AlbumController : ApiController
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


        //[HttpPost]
        //[AllowAnonymous]
        //[ResponseType(typeof(User))]
        //[Route("Account/Register")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Album/5
        public string Get(int id)
        {
            return "value";
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

                return this.Created<AlbumAddBindingModel>(Request.RequestUri + model.Id.ToString(), model);
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
