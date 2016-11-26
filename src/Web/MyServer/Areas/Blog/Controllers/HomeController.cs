using Microsoft.AspNetCore.Mvc;
using MyServer.Web.Areas.Shared.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MyServer.Data;
using MyServer.Data.Models;
using MyServer.Services.Users;

namespace MyServer.Web.Areas.Blog.Controllers
{
    [Area("Blog")]
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
