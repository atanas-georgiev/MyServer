namespace MyServer.Web.Areas.Account.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using MyServer.Data;
    using MyServer.Data.Models;
    using MyServer.Services;
    using MyServer.Services.Users;
    using MyServer.Web.Areas.Account.Models;
    using Shared.Controllers;
    using System.Security.Claims;
    using Common;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

    [Area("Account")]
    public class LoginController : BaseController
    {
        public LoginController(IUserService userService, UserManager<User> userManager, SignInManager<User> signInManager, MyServerDbContext dbContext) 
            : base(userService, userManager, signInManager, dbContext)
        {
        }

        [AllowAnonymous]
        public ActionResult Index(string returnUrl)
        {
            this.ViewData["ReturnUrl"] = returnUrl;
            return this.View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(AccountLoginViewModel model, string returnUrl = null)
        {
            this.ViewData["ReturnUrl"] = returnUrl;

            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var result =
                await
                this.signInManager.PasswordSignInAsync(
                    model.Email,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return this.RedirectToLocal(returnUrl);
            }

            this.ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Exit()
        {
            await this.signInManager.SignOutAsync();
            return this.RedirectToAction("Index", "Home", new { area = string.Empty });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            var redirectUrl = this.Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl });
            var properties = this.signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return this.Challenge(properties, provider);
        }

        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl, string remoteError = null)
        {
            if (remoteError != null)
            {
                this.ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return this.View(nameof(Index));
            }

            var info = await this.signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return this.RedirectToAction(nameof(Index));
            }

            var result = await this.signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);

            if (result.Succeeded)
            {
                return this.RedirectToLocal(returnUrl);
            }
            else
            {
                var email = info.Principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
                var firstName = info.Principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName);
                var lastName = info.Principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname);
                this.ViewBag.ReturnUrl = returnUrl;

                //this.ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                return this.View(
                    "ExternalLoginConfirmation",
                    new AccountExternalLoginConfirmationViewModel
                    {
                        Email = email == null ? "" : email.Value,
                        FirstName = firstName == null ? "" : firstName.Value,
                        LastName = lastName == null ? "" : lastName.Value
                    });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(
            AccountExternalLoginConfirmationViewModel model, 
            string returnUrl = null)
        {
            //if (this.ModelState.IsValid)
            {
                var info = await this.signInManager.GetExternalLoginInfoAsync();
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

                var result = await this.userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await this.userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await this.signInManager.SignInAsync(user, isPersistent: false);

                        var role = this.dbContext.Roles.First(x => x.Name == MyServerRoles.User);
                        this.dbContext.UserRoles.Add(new IdentityUserRole<string>() { RoleId = role.Id, UserId = user.Id });
                        this.dbContext.SaveChanges();

                        return this.RedirectToLocal(returnUrl);
                    }
                }
                this.AddErrors(result);
            }

            this.ViewData["ReturnUrl"] = returnUrl;
            return this.View(model);
        }

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return this.View();
        }
    }
}