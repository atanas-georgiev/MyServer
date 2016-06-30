namespace MyServer.Web.Areas.Account.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.AspNetCore.Mvc;

    using MyServer.Common;
    using MyServer.Data;
    using MyServer.Data.Models;
    using MyServer.Services;
    using MyServer.Services.Users;
    using MyServer.Web.Areas.Account.Models;
    using System.Linq;

    [Route("Account/Register")]
    public class RegisterController : BaseController
    {
        public RegisterController(IUserService userService, UserManager<User> userManager, SignInManager<User> signInManager, IEmailSender emailSender, MyServerDbContext dbContext) : base(userService, userManager, signInManager, emailSender, dbContext)
        {
        }

        [AllowAnonymous]
        [Route("Index")]
        public ActionResult Index()
        {
            return this.View();
        }

        [HttpPost]
        [Route("Index")]
        [AllowAnonymous]
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

                var result = await this.userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    var role = this.dbContext.Roles.First(x => x.Name == MyServerRoles.Admin);
                    this.dbContext.UserRoles.Add(new IdentityUserRole<string>() { RoleId = role.Id, UserId = user.Id });
                    this.dbContext.SaveChanges();

                    await this.signInManager.SignInAsync(user, isPersistent: false);
                    return this.RedirectToAction("Index", "Home", new { area = string.Empty });
                }

                this.AddErrors(result);
            }

            return this.View(model);
        }
    }
}