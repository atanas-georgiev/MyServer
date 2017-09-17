using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Http;
using MyServer.Common;
using MyServer.Web.Resources;

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

            var result =
                await
                    this.signInManager.ExternalLoginSignInAsync(
                        info.LoginProvider,
                        info.ProviderKey,
                        isPersistent: false,
                        bypassTwoFactor: true);

            if (result.Succeeded)
            {
                return this.LocalRedirect(this.Url.GetLocalUrl(returnUrl));
            }
            else
            {
                var email = info.Principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
                var firstName = info.Principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName);
                var lastName = info.Principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname);
                this.ReturnUrl = returnUrl;
                this.LoginProvider = info.LoginProvider;

                this.Input = new InputModel()
                    {
                        Email = email == null ? string.Empty : email.Value,
                        FirstName = firstName == null ? string.Empty : firstName.Value,
                        LastName = lastName == null ? string.Empty : lastName.Value
                    };
            }

            return this.Page();
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
            ExternalLoginInfo info = null;

            if (this.ModelState.IsValid)
            {
                info = await this.signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return this.RedirectToPage("./ExternalLoginFailed");
                }
            }

            var user = new User
            {
                UserName = this.Input.Email,
                Email = this.Input.Email,
                CreatedOn = DateTime.Now,
                FirstName = this.Input.FirstName,
                LastName = this.Input.LastName
            };

            var result = await this.userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                result = await this.userManager.AddLoginAsync(user, info);
                if (result.Succeeded)
                {
                    await this.signInManager.SignInAsync(user, isPersistent: false);

                    var role = this.dbContext.Roles.First(x => x.Name == MyServerRoles.User.ToString());
                    this.dbContext.UserRoles.Add(
                        new IdentityUserRole<string>() { RoleId = role.Id, UserId = user.Id });
                    this.dbContext.SaveChanges();

                    return this.LocalRedirect(this.Url.GetLocalUrl(returnUrl));
                }
            }

            this.ModelState.AddModelError("Input.Email", Startup.SharedLocalizer["UsernameExist"]);
            this.ReturnUrl = returnUrl;
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
            [StringLength(50, ErrorMessageResourceName = "ErrorLength",
                ErrorMessageResourceType = typeof(Helpers_SharedResource), MinimumLength = 2)]
            [Display(Name = "FirstName", ResourceType = typeof(Helpers_SharedResource))]
            public string FirstName { get; set; }

            [Required(ErrorMessageResourceName = "ErrorRequired", ErrorMessageResourceType = typeof(Helpers_SharedResource))
            ]
            [StringLength(50, ErrorMessageResourceName = "ErrorLength",
                ErrorMessageResourceType = typeof(Helpers_SharedResource), MinimumLength = 2)]
            [Display(Name = "LastName", ResourceType = typeof(Helpers_SharedResource))]
            public string LastName { get; set; }
        }

        public ExternalLoginModel(IUserService userService, UserManager<User> userManager, SignInManager<User> signInManager, MyServerDbContext dbContext, IHttpContextAccessor httpContextAccessor) : base(userService, userManager, signInManager, dbContext, httpContextAccessor)
        {
        }
    }
}