namespace MyServer.Web.Areas.Account.Controllers
{
    using Kendo.Mvc.Extensions;
    using System.Linq;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using MyServer.Data;
    using MyServer.Data.Models;
    using MyServer.Services.Users;
    using MyServer.Web.Areas.Account.Models;
    using MyServer.Web.Areas.Shared.Controllers;

    [Authorize]
    [Area("Account")]
    public class ManageController : BaseController
    {
        public ManageController(
            IUserService userService,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            MyServerDbContext dbcontext)
            : base(userService, userManager, signInManager, dbcontext)
        {
        }

        public IActionResult Index()
        {
            var model = new AccountManageViewModel()
                            {
                                Email = this.UserProfile.Email,
                                FirstName = this.UserProfile.FirstName,
                                LastName = this.UserProfile.LastName
                            };

            return this.View(model);
        }

        [HttpPost]
        public IActionResult Index(AccountManageViewModel model)
        {
            var user = this.UserService.GetAll().FirstOrDefault(x => x.UserName == model.Email);

            if (user != null && user.UserName != this.UserProfile.UserName)
            {
                this.ModelState.AddModelError("Email", Startup.SharedLocalizer["UsernameExist"]);
            }
            else if (this.ModelState.IsValid)
            {
                this.UserProfile.UserName = model.Email;
                this.UserProfile.Email = model.Email;
                this.UserProfile.FirstName = model.FirstName;
                this.UserProfile.LastName = model.LastName;

                this.UserService.Update(this.UserProfile);
                return this.RedirectToAction("Index", "Home", new { area = string.Empty });
            }

            return this.View(model);
        }
    }
}