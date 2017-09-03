using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Razor;

namespace MyServer.Web.Helpers
{
    public class ViewLocationExpander : IViewLocationExpander
    {
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            string[] locations = new string[]
            {
                //"~/Pages/{0}.cshtml",
            };
            return locations.Concat(viewLocations);
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            context.Values["customviewlocation"] = nameof(ViewLocationExpander);
        }
    }
}
