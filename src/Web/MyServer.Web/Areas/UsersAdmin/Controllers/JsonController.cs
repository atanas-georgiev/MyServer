namespace MyServer.Web.Areas.UsersAdmin.Controllers
{
    using Kendo.Mvc.Extensions;
    using Kendo.Mvc.UI;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using MyServer.Data;
    using MyServer.Data.Models;
    using MyServer.Services.Users;
    using MyServer.Web.Areas.Shared.Controllers;
    using MyServer.Web.Areas.UsersAdmin.Models;
    using Services.Mappings;

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
                // productService.Create(product);
            }

            return this.Json(new[] { user }.ToDataSourceResult(request, this.ModelState));
        }

        [HttpPost]
        public ActionResult UsersDestroy([DataSourceRequest] DataSourceRequest request, UsersViewModel user)
        {
            if (user != null)
            {
                // productService.Destroy(product);
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
                // productService.Update(product);
            }

            return this.Json(new[] { user }.ToDataSourceResult(request, this.ModelState));
        }
    }
}