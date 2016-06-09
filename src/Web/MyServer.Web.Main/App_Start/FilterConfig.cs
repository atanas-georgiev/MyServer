using System.Web;
using System.Web.Mvc;

namespace MyServer.Web.Main
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            //filters.Add(new CustomHandleErrorAttribute());
            filters.Add(new AuthorizeAttribute());
        }
    }
}
