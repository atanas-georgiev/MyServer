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
    using MyServer.Services.ImageGallery;
    using MyServer.Web.Api.Areas.ImageGallery.Models;
    using MyServer.Web.Infrastructure.Mappings;

    public class ImagesController : BaseController
    {
        private readonly IAlbumService albumService;

        private readonly IImageService imageService;

        private readonly ILocationService locationService;

        public ImagesController(
            IAlbumService albumService,
            ILocationService locationService,
            IImageService imageService)
        {
            this.albumService = albumService;
            this.locationService = locationService;
            this.imageService = imageService;
        }

        [Route("ImageGallery/Albums/{albumId}/Images")]
        [EnableQuery]
        [ResponseType(typeof(IQueryable<ImageBindingModel>))]
        public IHttpActionResult Get(string albumId)
        {
            try
            {
                var albumIdGuid = Guid.Parse(albumId);
                var album = this.albumService.GetById(albumIdGuid);

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

                return this.Ok(album.Images.AsQueryable().To<ImageBindingModel>());
            }
            catch (Exception ex)
            {
                return this.InternalServerError(ex);
            }
        }

        [Authorize(Roles = MyServerRoles.Admin)]
        [Route("ImageGallery/Albums/{albumId}/Images")]
        public async Task<IHttpActionResult> Post(string albumId, HttpRequestMessage request)
        {
            try
            {
                var albumIdGuid = Guid.Parse(albumId);
                var album = this.albumService.GetById(albumIdGuid);

                if (album == null)
                {
                    return this.NotFound();
                }

                var provider = new RestrictiveMultipartMemoryStreamProvider();
                await request.Content.ReadAsMultipartAsync(provider);

                foreach (var ctnt in provider.Contents)
                {
                    var stream = await ctnt.ReadAsStreamAsync();

                    if (stream.Length != 0)
                    {
                        var fileName = ctnt.Headers.ContentDisposition.FileName.Replace('\"', ' ').Trim();
                        this.imageService.Add(
                            albumIdGuid,
                            stream,
                            fileName,
                            System.Web.HttpContext.Current.Server,
                            this.User.Identity.GetUserId());
                    }
                }

                return this.Ok();
            }
            catch (Exception ex)
            {
                return this.InternalServerError(ex);
            }
        }
    }
}