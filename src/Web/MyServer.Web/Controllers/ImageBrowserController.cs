namespace MyServer.Web.Controllers
{
    using System.IO;

    using Kendo.Mvc.UI;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Roles = "Admin")]
    public class ImageBrowserController : EditorImageBrowserController
    {
        private readonly IHostingEnvironment appEnvironment;

        private readonly string VirtualPath = "UserFiles";

        public ImageBrowserController(IHostingEnvironment hostingEnvironment)
            : base(hostingEnvironment)
        {
            this.appEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// Gets the base paths from which content will be served.
        /// </summary>
        public override string ContentPath => this.CreateUserFolder();        

        private string CreateUserFolder()
        {
            

            var path = this.appEnvironment.WebRootPath + "/" + this.VirtualPath;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                //foreach (var sourceFolder in foldersToCopy)
                //{
                //    this.CopyFolder(this.appEnvironment.WebRootPath + sourceFolder, path);
                //}
            }

            var res = this.HostingEnvironment.WebRootPath + "/" + this.VirtualPath;
            return res;
        }

        public override ActionResult Upload(string path, IFormFile file)
        {
            var fullPath = this.HostingEnvironment.WebRootPath + "/" + this.VirtualPath + path;

            if (file.Length > 0)
            {
                var filePath = Path.Combine(fullPath, file.FileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyToAsync(fileStream);
                }

                return this.Content(string.Empty);
            }

            return this.NotFound();
        }
    }
}