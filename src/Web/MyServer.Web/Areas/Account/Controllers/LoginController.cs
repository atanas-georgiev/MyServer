﻿namespace MyServer.Web.Areas.Account.Controllers
{
    using System;
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

    [Area("Account")]
    [Route("Account/Login")]
    public class LoginController : BaseController
    {
        public LoginController(IUserService userService, UserManager<User> userManager, SignInManager<User> signInManager, MyServerDbContext dbContext) 
            : base(userService, userManager, signInManager, dbContext)
        {
        }

        [AllowAnonymous]
        [Route("Index")]
        public ActionResult Index(string returnUrl)
        {
            this.ViewData["ReturnUrl"] = returnUrl;
            return this.View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Index")]
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
        [Route("Exit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Exit()
        {
            await this.signInManager.SignOutAsync();
            return this.RedirectToAction("Index", "Home", new { area = string.Empty });
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("ExternalLogin")]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            var redirectUrl = this.Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl });
            var properties = this.signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return this.Challenge(properties, provider);
        }

        [AllowAnonymous]
        [Route("ExternalLoginCallback")]
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
            //var firstName = result.ExternalIdentity.Name.Split(' ').FirstOrDefault();
            //var lastName = result.ExternalIdentity.Name.Split(' ').LastOrDefault();
            //var email = result.Email;


            if (result.Succeeded)
            {
                return this.RedirectToLocal(returnUrl);
            }
            else
            {
                this.ViewBag.ReturnUrl = returnUrl;
                //this.ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                return this.View(
                    "ExternalLoginConfirmation",
                    new AccountExternalLoginConfirmationViewModel
                    {
                        //Email = email,
                        //FirstName = firstName,
                        //LastName = lastName
                    });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("ExternalLoginConfirmation")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(
            AccountExternalLoginConfirmationViewModel model, 
            string returnUrl = null)
        {
            if (this.ModelState.IsValid)
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
                        return this.RedirectToLocal(returnUrl);
                    }
                }
                this.AddErrors(result);
            }

            this.ViewData["ReturnUrl"] = returnUrl;
            return this.View(model);
        }

        [AllowAnonymous]
        [Route("ExternalLoginFailure")]
        public ActionResult ExternalLoginFailure()
        {
            return this.View();
        }
    }
}