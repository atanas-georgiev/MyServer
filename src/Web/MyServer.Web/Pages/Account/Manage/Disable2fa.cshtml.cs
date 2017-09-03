namespace MyServer.Web.Pages.Account.Manage
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.Extensions.Logging;

    using MyServer.Web.Data;

    public class Disable2faModel : PageModel
    {
        private readonly ILogger<Disable2faModel> _logger;

        private readonly UserManager<ApplicationUser> _userManager;

        public Disable2faModel(UserManager<ApplicationUser> userManager, ILogger<Disable2faModel> logger)
        {
            this._userManager = userManager;
            this._logger = logger;
        }

        public async Task<IActionResult> OnGet()
        {
            var user = await this._userManager.GetUserAsync(this.User);
            if (user == null)
            {
                throw new ApplicationException(
                    $"Unable to load user with ID '{this._userManager.GetUserId(this.User)}'.");
            }

            if (!await this._userManager.GetTwoFactorEnabledAsync(user))
            {
                throw new ApplicationException(
                    $"Cannot disable 2FA for user with ID '{this._userManager.GetUserId(this.User)}' as it's not currently enabled.");
            }

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await this._userManager.GetUserAsync(this.User);
            if (user == null)
            {
                throw new ApplicationException(
                    $"Unable to load user with ID '{this._userManager.GetUserId(this.User)}'.");
            }

            var disable2faResult = await this._userManager.SetTwoFactorEnabledAsync(user, false);
            if (!disable2faResult.Succeeded)
            {
                throw new ApplicationException(
                    $"Unexpected error occurred disabling 2FA for user with ID '{this._userManager.GetUserId(this.User)}'.");
            }

            this._logger.LogInformation(
                "User with ID '{UserId}' has disabled 2fa.",
                this._userManager.GetUserId(this.User));

            return this.RedirectToPage("./TwoFactorAuthentication");
        }
    }
}