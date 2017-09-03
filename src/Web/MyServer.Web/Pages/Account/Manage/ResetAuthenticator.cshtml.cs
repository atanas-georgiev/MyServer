namespace MyServer.Web.Pages.Account.Manage
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.Extensions.Logging;

    using MyServer.Web.Data;

    public class ResetAuthenticatorModel : PageModel
    {
        ILogger<ResetAuthenticatorModel> _logger;

        UserManager<ApplicationUser> _userManager;

        public ResetAuthenticatorModel(
            UserManager<ApplicationUser> userManager,
            ILogger<ResetAuthenticatorModel> logger)
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

            await this._userManager.SetTwoFactorEnabledAsync(user, false);
            await this._userManager.ResetAuthenticatorKeyAsync(user);
            this._logger.LogInformation("User with ID '{UserId}' has reset their authentication app key.", user.Id);

            return this.RedirectToPage("./EnableAuthenticator");
        }
    }
}