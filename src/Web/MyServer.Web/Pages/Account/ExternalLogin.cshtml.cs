namespace MyServer.Web.Pages.Account
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.Extensions.Logging;
    using MyServer.Data;
    using MyServer.Data.Models;
    using MyServer.Services.Users;
    using MyServer.Web.Data;
    using MyServer.Web.Extensions;
    using MyServer.Web.Pages.Base;

    public class ExternalLoginModel : BasePageModel
    {
        public ExternalLoginModel(IUserService userService, UserManager<User> userManager, SignInManager<User> signInManager, MyServerDbContext dbContext) : base(userService, userManager, signInManager, dbContext)
        {
        }

        [TempData]
        public string ErrorMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public string LoginProvider { get; set; }

        public string ReturnUrl { get; set; }

        public IActionResult OnGetAsync()
        {
            return this.RedirectToPage("./Login");
        }

        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                this.ErrorMessage = $"Error from external provider: {remoteError}";
                return this.RedirectToPage("./Login");
            }

            var info = await this.signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return this.RedirectToPage("./Login");
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await this.signInManager.ExternalLoginSignInAsync(
                             info.LoginProvider,
                             info.ProviderKey,
                             isPersistent: false,
                             bypassTwoFactor: true);
            if (result.Succeeded)
            {
                return this.LocalRedirect(this.Url.GetLocalUrl(returnUrl));
            }

            if (result.IsLockedOut)
            {
                return this.RedirectToPage("./Lockout");
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                this.ReturnUrl = returnUrl;
                this.LoginProvider = info.LoginProvider;
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    this.Input = new InputModel { Email = info.Principal.FindFirstValue(ClaimTypes.Email) };
                }

                return this.Page();
            }
        }

        public IActionResult OnPost(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = this.Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            var properties = this.signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
        {
            if (this.ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await this.signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    throw new ApplicationException("Error loading external login information during confirmation.");
                }

                var user = new User { UserName = this.Input.Email, Email = this.Input.Email };
                var result = await this.userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await this.userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await this.signInManager.SignInAsync(user, isPersistent: false);
                        return this.LocalRedirect(this.Url.GetLocalUrl(returnUrl));
                    }
                }

                foreach (var error in result.Errors)
                {
                    this.ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            this.ReturnUrl = returnUrl;
            return this.Page();
        }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }
    }
}