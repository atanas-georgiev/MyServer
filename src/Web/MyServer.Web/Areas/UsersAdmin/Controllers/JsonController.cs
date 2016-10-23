namespace MyServer.Web.Areas.UsersAdmin.Controllers
{
    using Kendo.Mvc.Extensions;
    using Kendo.Mvc.UI;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using MyServer.Data;
    using MyServer.Data.Models;
    using MyServer.Services.Mappings;
    using MyServer.Services.Users;
    using MyServer.Web.Areas.Shared.Controllers;
    using MyServer.Web.Areas.UsersAdmin.Models;

    [Authorize(Roles = "Admin")]
    [Area("UsersAdmin")]
    public class JsonController : BaseController
    {
        public JsonController(
            IUserService userService,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            MyServerDbContext context)
            : base(userService, userManager, signInManager, context)
        {
        }

        [HttpPost]
        public ActionResult UsersCreate([DataSourceRequest] DataSourceRequest request, UsersViewModel user)
        {
            if (user != null && this.ModelState.IsValid)
            {
                var dbuser = new User() { FirstName = user.FirstName, LastName = user.LastName, Email = user.Email };
                this.UserService.Add(dbuser, user.Role.ToString());
            }

            return this.Json(new[] { user }.ToDataSourceResult(request, this.ModelState));
        }

        [HttpPost]
        public ActionResult UsersDestroy([DataSourceRequest] DataSourceRequest request, UsersViewModel user)
        {
            if (user != null)
            {
                this.UserService.Delete(user.Id);
            }

            return this.Json(new[] { user }.ToDataSourceResult(request, this.ModelState));
        }

        public ActionResult UsersRead([DataSourceRequest] DataSourceRequest request)
        {
            return this.Json(this.UserService.GetAll().To<UsersViewModel>().ToDataSourceResult(request));
        }

        [HttpPost]
        public ActionResult UsersUpdate([DataSourceRequest] DataSourceRequest request, UsersViewModel user)
        {
            if (user != null && this.ModelState.IsValid)
            {
                var dbuser = this.UserService.GetById(user.Id);
                if (dbuser != null)
                {
                    dbuser.FirstName = user.FirstName;
                    dbuser.LastName = user.LastName;
                    dbuser.Email = user.Email;

                    this.UserService.Update(dbuser, user.Role.ToString());
                }
            }

            return this.Json(new[] { user }.ToDataSourceResult(request, this.ModelState));
        }
    }
}