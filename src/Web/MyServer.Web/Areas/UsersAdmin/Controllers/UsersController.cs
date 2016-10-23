namespace MyServer.Web.Areas.UsersAdmin.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using MyServer.Data;
    using MyServer.Data.Models;
    using MyServer.Services.Users;
    using MyServer.Web.Areas.Shared.Controllers;

    [Authorize(Roles = "Admin")]
    [Area("UsersAdmin")]
    public class UsersController : BaseController
    {
        public UsersController(
            IUserService userService,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            MyServerDbContext context)
            : base(userService, userManager, signInManager, context)
        {
        }

        public IActionResult Index()
        {
            return this.View();
        }
    }
}