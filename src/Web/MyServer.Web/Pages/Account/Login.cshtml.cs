using MyServer.Web.Pages.Base;
using MyServer.Web.Resources;

namespace MyServer.Web.Pages.Account
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.Extensions.Logging;
    using MyServer.Data;
    using MyServer.Data.Models;
    using MyServer.Services.Users;
    using MyServer.Web.Data;
    using MyServer.Web.Extensions;

    public class LoginModel : BasePageModel
    {
        public LoginModel(IUserService userService, UserManager<User> userManager, SignInManager<User> signInManager, MyServerDbContext dbContext) : base(userService, userManager, signInManager, dbContext)
        {
        }

        [TempData]
        public string ErrorMessage { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(this.ErrorMessage))
            {
                this.ModelState.AddModelError(string.Empty, this.ErrorMessage);
            }

            // Clear the existing external cookie to ensure a clean login process
            await this.HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            this.ExternalLogins = (await this.signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            this.ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            this.ReturnUrl = returnUrl;

            if (!this.ModelState.IsValid)
            {
                return this.Page();
            }

            var result =
                await this.signInManager.PasswordSignInAsync(Input.Email, Input.Password, true, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return this.LocalRedirect(this.Url.GetLocalUrl(returnUrl));
            }

            this.ModelState.AddModelError(string.Empty, Startup.SharedLocalizer["InvalidCredentials"]);
            return this.Page();
        }

        public class InputModel
        {
            [Required(ErrorMessageResourceName = "ErrorRequired", ErrorMessageResourceType = typeof(Helpers_SharedResource))
            ]
            [EmailAddress(ErrorMessageResourceName = "InvalidEmail",
                ErrorMessageResourceType = typeof(Helpers_SharedResource))]
            [Display(Name = "Email", ResourceType = typeof(Helpers_SharedResource))]
            public string Email { get; set; }

            [Required(ErrorMessageResourceName = "ErrorRequired", ErrorMessageResourceType = typeof(Helpers_SharedResource))
            ]
            [StringLength(50, ErrorMessageResourceName = "ErrorMinLength",
                ErrorMessageResourceType = typeof(Helpers_SharedResource), MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password", ResourceType = typeof(Helpers_SharedResource))]
            public string Password { get; set; }

            [Display(Name = "RememberMe", ResourceType = typeof(Helpers_SharedResource))]
            public bool RememberMe { get; set; }
        }
    }
}