namespace MyServer.Web.Api.Areas.ImageGallery.Controllers
{
    using System.Net.Http;
    using System.Web.Http;

    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin.Security;

    using MyServer.Data.Models;
    using MyServer.Services.Users;

    public class BaseController : ApiController
    {
        public BaseController()
        {
        }

        public BaseController(IUserService userService)
        {
            this.UserService = userService;
        }

        public ApplicationUserManager UserManager
            => this.Request.GetOwinContext().GetUserManager<ApplicationUserManager>();

        protected IAuthenticationManager AuthenticationManager { get; private set; }

        protected User UserProfile { get; private set; }

        protected IUserService UserService { get; set; }

        protected void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError(string.Empty, error);
            }
        }

        protected IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return this.InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        this.ModelState.AddModelError(string.Empty, error);
                    }
                }

                if (this.ModelState.IsValid)
                {
                    return this.BadRequest();
                }

                return this.BadRequest(this.ModelState);
            }

            return null;
        }
    }
}