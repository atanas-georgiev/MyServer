namespace MyServer.Web.Helpers
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;

    using MyServer.Data.Models;

    public class PathHelper
    {
        public static UserManager<User> UserManager = null;

        public static string WwwPath = string.Empty;

        public static string WwwRootPath = string.Empty;

        public PathHelper(IHostingEnvironment appEnvironment, UserManager<User> userManager)
        {
            WwwPath = appEnvironment.WebRootPath;
            UserManager = userManager;
        }
    }
}