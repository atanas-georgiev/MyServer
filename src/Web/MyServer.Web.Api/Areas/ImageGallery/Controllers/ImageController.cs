using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace MyServer.Web.Api.Areas.ImageGallery.Controllers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;

    using Microsoft.AspNet.Identity;
    using Microsoft.Owin;

    using MyServer.Services.ImageGallery;

    public class ImageController : BaseController
    {
        private readonly IImageService imageService;

        public ImageController(IImageService imageService)
        {
            this.imageService = imageService;
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.Route("ImageGallery/Image")]
        public async Task<IHttpActionResult> Post(HttpRequestMessage request)
        {
            var provider = new RestrictiveMultipartMemoryStreamProvider();

            await request.Content.ReadAsMultipartAsync(provider);
            var albumId = Guid.Parse(request.Headers.GetValues("Album-Id").First());

            foreach (HttpContent ctnt in provider.Contents)
            {
                var stream = await ctnt.ReadAsStreamAsync();
                
                if (stream.Length != 0)
                {
                    var fileName = ctnt.Headers.ContentDisposition.FileName.Replace('\"', ' ').Trim();
                    this.imageService.Add(albumId, stream, fileName, System.Web.HttpContext.Current.Server, this.User.Identity.GetUserId());
                }
            }

            return this.Ok();
        }
    }
}