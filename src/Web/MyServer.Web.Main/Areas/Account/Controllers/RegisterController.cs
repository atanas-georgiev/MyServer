namespace MyServer.Web.Main.Areas.Account.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using global::ImageGallery.Data.Models;

    using Microsoft.AspNet.Identity;

    using MyServer.Common;
    using MyServer.Services.Users;
    using MyServer.Web.Main.Areas.Account.Models;
    using MyServer.Web.Main.Controllers;

    public class RegisterController : BaseController
    {
        public RegisterController(IUserService userService)
            : base(userService)
        {
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            return this.View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(AccountRegisterViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = new User
                               {
                                    UserName = model.Email,
                                    Email = model.Email,
                                    FirstName = model.FirstName,
                                    LastName = model.LastName,
                                    CreatedOn = DateTime.UtcNow
                               };

                var result = await this.UserManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    this.UserManager.AddToRole(user.Id, MyServerRoles.User);
                    await this.SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    return this.RedirectToAction("Index", "Home", new { area = string.Empty });
                }

                this.AddErrors(result);
            }

            return this.View(model);
        }
    }
}