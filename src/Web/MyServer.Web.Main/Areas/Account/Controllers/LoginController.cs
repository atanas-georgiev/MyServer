namespace MyServer.Web.Main.Areas.Account.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using global::ImageGallery.Data.Models;

    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin.Security;

    using MyServer.Common;
    using MyServer.Services.Users;
    using MyServer.Web.Main.Areas.Account.Models;

    public class LoginController : BaseController
    {
        public LoginController(IUserService userService)
            : base(userService)
        {
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Exit()
        {
            this.AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return this.RedirectToAction("Index", "Home", new { area = string.Empty });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ChallengeResult(
                provider, 
                this.Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await this.AuthenticationManager.GetExternalLoginInfoAsync();
            var firstName = loginInfo.ExternalIdentity.Name.Split(' ').FirstOrDefault();
            var lastName = loginInfo.ExternalIdentity.Name.Split(' ').LastOrDefault();
            var email = loginInfo.Email;

            if (loginInfo == null)
            {
                return this.RedirectToAction("Index");
            }

            var result = await this.SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return this.RedirectToLocal(returnUrl);
                default:
                    this.ViewBag.ReturnUrl = returnUrl;
                    this.ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return this.View(
                        "ExternalLoginConfirmation", 
                        new AccountExternalLoginConfirmationViewModel
                            {
                                Email = email, 
                                FirstName = firstName, 
                                LastName = lastName
                            });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(
            AccountExternalLoginConfirmationViewModel model, 
            string returnUrl)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                return this.RedirectToAction("Index", "Manage");
            }

            if (this.ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await this.AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return this.View("ExternalLoginFailure");
                }

                var user = new User
                               {
                                   UserName = model.Email, 
                                   Email = model.Email, 
                                   CreatedOn = DateTime.Now, 
                                   FirstName = model.FirstName, 
                                   LastName = model.LastName
                               };
                var result = await this.UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await this.UserManager.AddLoginAsync(user.Id, info.Login);

                    if (result.Succeeded)
                    {
                        this.UserManager.AddToRole(user.Id, MyServerRoles.User);
                        await this.SignInManager.SignInAsync(user, isPersistent: true, rememberBrowser: false);
                        return this.RedirectToLocal(returnUrl);
                    }
                }

                this.AddErrors(result);
            }

            this.ViewBag.ReturnUrl = returnUrl;
            return this.View(model);
        }

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return this.View();
        }

        [AllowAnonymous]
        public ActionResult Index(string returnUrl)
        {
            this.ViewBag.ReturnUrl = returnUrl;
            return this.View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(AccountLoginViewModel model, string returnUrl)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var result =
                await
                this.SignInManager.PasswordSignInAsync(
                    model.Email, 
                    model.Password, 
                    model.RememberMe, 
                    shouldLockout: false);

            if (result == SignInStatus.Success)
            {
                return this.RedirectToLocal(returnUrl);
            }

            this.ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return this.View(model);
        }
    }
}