using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MyServer.Web.Main.Startup))]

namespace MyServer.Web.Main
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            this.ConfigureAuth(app);
        }
    }
}
