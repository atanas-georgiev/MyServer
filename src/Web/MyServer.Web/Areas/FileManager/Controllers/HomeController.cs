using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyServer.Data;
using MyServer.Data.Models;
using MyServer.Services.Users;
using MyServer.Web.Areas.Shared.Controllers;

namespace MyServer.Web.Areas.FileManager.Controllers
{
    [Area("FileManager")]
    public class HomeController : BaseController
    {
        public HomeController(IUserService userService, UserManager<User> userManager, SignInManager<User> signInManager, MyServerDbContext dbContext) : base(userService, userManager, signInManager, dbContext)
        {
        }

        public IActionResult Index()
        {
            return this.View();
        }
    }
}
