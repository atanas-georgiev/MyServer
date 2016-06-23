using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MyServer.Web.Api
{
    using System.Reflection;

    using MyServer.Web.Infrastructure;

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            DatabaseConfig.Initialize();

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            AutofacConfig.RegisterAutofac();

            var autoMapperConfig = new AutoMapperConfig();
            autoMapperConfig.Execute(Assembly.GetExecutingAssembly());
        }
    }
}
