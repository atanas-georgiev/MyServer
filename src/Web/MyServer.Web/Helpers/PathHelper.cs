
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using MyServer.Data.Models;

namespace MyServer.Web.Helpers
{
    public class PathHelper
    {
        public static string WwwPath = string.Empty;

        public static string WwwRootPath = string.Empty;

        public static UserManager<User> UserManager = null;

        public PathHelper(IHostingEnvironment appEnvironment, UserManager<User> userManager)
        {
            WwwPath = appEnvironment.WebRootPath;
            UserManager = userManager;
        }
    }
}
