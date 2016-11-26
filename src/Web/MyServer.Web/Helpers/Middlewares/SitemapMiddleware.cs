using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyServer.Web.Helpers.Middlewares
{
    public static class SitemapMiddleware
    { 
        public static void HandleSitemap(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                context.Response.Redirect("/sitemap");
            });
        }
    }
}
