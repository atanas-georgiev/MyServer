namespace MyServer.Web.Main.Areas.Account.Controllers
{
    using System;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin.Security;

    using MyServer.Data.Models;
    using MyServer.Services.Users;

    public class BaseController : Controller
    {
        public BaseController(IUserService userService)
        {
            this.UserService = userService;
        }

        protected IAuthenticationManager AuthenticationManager { get; private set; }

        protected ApplicationSignInManager SignInManager { get; private set; }

        protected ApplicationUserManager UserManager { get; private set; }

        protected User UserProfile { get; private set; }

        protected IUserService UserService { get; set; }

        protected void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError(string.Empty, error);
            }
        }

        protected override IAsyncResult BeginExecute(
            RequestContext requestContext, 
            AsyncCallback callback, 
            object state)
        {
            this.UserProfile =
                this.UserService.GetAll()
                    .FirstOrDefault(u => u.UserName == requestContext.HttpContext.User.Identity.Name);

            this.SignInManager = requestContext.HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            this.UserManager = requestContext.HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            this.AuthenticationManager = requestContext.HttpContext.GetOwinContext().Authentication;

            return base.BeginExecute(requestContext, callback, state);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.UserManager != null)
                {
                    this.UserManager.Dispose();
                    this.UserManager = null;
                }

                if (this.SignInManager != null)
                {
                    this.SignInManager.Dispose();
                    this.SignInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        protected ActionResult RedirectToLocal(string returnUrl)
        {
            if (this.Url.IsLocalUrl(returnUrl))
            {
                return this.Redirect(returnUrl);
            }

            return this.RedirectToAction("Index", "Home", new { area = string.Empty });
        }
    }
}