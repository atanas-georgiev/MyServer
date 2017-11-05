namespace MyServer.Web.Controllers
{
    using System.IO;

    using Kendo.Mvc.UI;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;

    [Authorize(Roles = "Admin")]
    public class ImageBrowserController : EditorImageBrowserController
    {
        private readonly IHostingEnvironment appEnvironment;

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
            var virtualPath = "/UserFiles";

            var path = this.appEnvironment.WebRootPath + virtualPath;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                //foreach (var sourceFolder in foldersToCopy)
                //{
                //    this.CopyFolder(this.appEnvironment.WebRootPath + sourceFolder, path);
                //}
            }

            return virtualPath;
        }
    }
}