namespace MyServer.Web.Api.Areas.Account.Models
{
    using System.Web;

    using DelegateDecompiler;

    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;

    using MyServer.Common.ImageGallery;
    using MyServer.Data.Models;
    using MyServer.Web.Infrastructure.Mappings;

    using Newtonsoft.Json;

    public class AccountUserInfoBindingModel : IMapFrom<User>
    {
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Computed]
        public string Role
        {
            get
            {
                var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var userRoles = userManager.GetRoles(HttpContext.Current.User.Identity.GetUserId());
                return userRoles[0];
            }
        }

        // public string LoginProvider { get; set; }

        // public bool HasRegistered { get; set; }
    }
}