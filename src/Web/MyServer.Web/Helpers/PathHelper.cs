
using Microsoft.AspNetCore.Hosting;

namespace MyServer.Web.Helpers
{
    public class PathHelper
    {
        public static string WwwPath = string.Empty;
        
        public PathHelper(IHostingEnvironment appEnvironment)
        {
            WwwPath = appEnvironment.WebRootPath;
        }
    }
}
