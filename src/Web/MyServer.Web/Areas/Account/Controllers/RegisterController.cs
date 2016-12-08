namespace MyServer.Web.Areas.Account.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.AspNetCore.Mvc;

    using MyServer.Common;
    using MyServer.Data;
    using MyServer.Data.Models;
    using MyServer.Services.Users;
    using MyServer.Web.Areas.Account.Models;
    using MyServer.Web.Areas.Shared.Controllers;

    [Area("Account")]
    public class RegisterController : BaseController
    {
        public RegisterController(
            IUserService userService,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            MyServerDbContext dbContext)
            : base(userService, userManager, signInManager, dbContext)
        {
        }

        public ActionResult Index(string returnUrl)
        {
            return this.View(new AccountRegisterViewModel() { returnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> Index(AccountRegisterViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var result = await this.UserService.Add(model.Email, model.FirstName, model.LastName, model.Password);                

                if (string.IsNullOrEmpty(result))
                {
                    var user = this.UserService.GetAll().FirstOrDefault(x => x.Email == model.Email);

                    if (user != null)
                    {
                        await this.signInManager.SignInAsync(user, isPersistent: false);
                    }
                }
                else
                {
                    this.ModelState.AddModelError("Email", Startup.SharedLocalizer["UsernameExist"]);
                }
                
                if (string.IsNullOrEmpty(model.returnUrl))
                {
                    return this.RedirectToAction("Index", "Home", new { area = string.Empty });
                }

                return RedirectToLocal(model.returnUrl);                
            }

            return this.View(model);
        }
    }
}