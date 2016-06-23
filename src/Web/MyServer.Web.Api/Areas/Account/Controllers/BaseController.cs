namespace MyServer.Web.Api.Areas.Account.Controllers
{
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Cors;

    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin.Security;

    using MyServer.Data.Models;
    using MyServer.Services.Users;

//    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class BaseController : ApiController
    {
        public BaseController()
        {
        }

        public BaseController(IUserService userService)
        {
            this.UserService = userService;
        }

        // protected ApplicationSignInManager SignInManager { get; private set; }
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
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return this.BadRequest();
                }

                return this.BadRequest(this.ModelState);
            }

            return null;
        }

        // .FirstOrDefault(u => u.UserName == requestContext.HttpContext.User.Identity.Name);
        // this.UserService.GetAll()
        // this.UserProfile =
        // {
        // object state)
        // AsyncCallback callback, 
        // RequestContext requestContext, 

        // protected override IAsyncResult BeginExecute(

        // this.SignInManager = requestContext.HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
        // this.UserManager = requestContext.HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
        // this.AuthenticationManager = requestContext.HttpContext.GetOwinContext().Authentication;

        // return base.BeginExecute(requestContext, callback, state);
        // }

        // protected override void Dispose(bool disposing)
        // {
        // if (disposing)
        // {
        // if (this.UserManager != null)
        // {
        // this.UserManager.Dispose();
        // this.UserManager = null;
        // }

        // if (this.SignInManager != null)
        // {
        // this.SignInManager.Dispose();
        // this.SignInManager = null;
        // }
        // }

        // base.Dispose(disposing);
        // }

        // protected ActionResult RedirectToLocal(string returnUrl)
        // {
        // if (this.Url.IsLocalUrl(returnUrl))
        // {
        // return this.Redirect(returnUrl);
        // }

        // return this.RedirectToAction("Index", "Home", new { area = string.Empty });
        // }
    }
}