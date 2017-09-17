using System.Linq;
using System.Net.Cache;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyServer.Data;
using MyServer.Data.Models;
using MyServer.Services.Users;

namespace MyServer.Web.Pages.Base
{
    public class BasePageModel : PageModel
    {
        protected readonly MyServerDbContext dbContext;

        protected readonly SignInManager<User> signInManager;

        protected readonly UserManager<User> userManager;

        public BasePageModel(
            IUserService userService,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            MyServerDbContext dbContext,
            IHttpContextAccessor httpContextAccessor)
        {
            this.UserService = userService;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.dbContext = dbContext;

            this.UserProfile =
                this.UserService.GetAll().FirstOrDefault(u => u.UserName == httpContextAccessor.HttpContext.User.Identity.Name);
        }

        protected User UserProfile { get; private set; }

        protected IUserService UserService { get; set; }
    }
}
