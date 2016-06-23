using System.Web.Mvc;

namespace MyServer.Web.Api.Areas.Account
{
    using System.Web.Http;

    public class AccountAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Account";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.Routes.MapHttpRoute(
                name: "AccountApi",
                routeTemplate: "Account/{controller}/{id}",
                defaults: new { area = "Account", id = RouteParameter.Optional });
        }
    }
}