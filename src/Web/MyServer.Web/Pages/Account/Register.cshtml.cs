using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using MyServer.Common;
using MyServer.Web.Pages.Base;
using MyServer.Web.Resources;

namespace MyServer.Web.Pages.Account
{
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using MyServer.Data;
    using MyServer.Data.Models;
    using MyServer.Services.Users;
    using MyServer.Web.Data;
    using MyServer.Web.Extensions;
    using MyServer.Web.Services;

    public class RegisterModel : BasePageModel
    {
        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public void OnGet(string returnUrl = null)
        {
            this.ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            this.ReturnUrl = returnUrl;

            if (this.ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    CreatedOn = DateTime.UtcNow
                };

                var result = await this.userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    var role = this.dbContext.Roles.First(x => x.Name == MyServerRoles.User.ToString());
                    this.dbContext.UserRoles.Add(new IdentityUserRole<string>() { RoleId = role.Id, UserId = user.Id });
                    this.dbContext.SaveChanges();

                    await this.signInManager.SignInAsync(user, isPersistent: false);
                    return this.LocalRedirect(this.Url.GetLocalUrl(returnUrl));
                }
            }

            this.ModelState.AddModelError("Input.Email", Startup.SharedLocalizer["UsernameExist"]);
            return this.Page();
        }

        public class InputModel
        {
            [DataType(DataType.Password)]
            [Display(Name = "ConfirmPassword", ResourceType = typeof(Helpers_SharedResource))]
            [Compare("Password", ErrorMessageResourceName = "ErrorPasswordNotMatch",
                         ErrorMessageResourceType = typeof(Helpers_SharedResource))]
            public string ConfirmPassword { get; set; }

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

            [Required(ErrorMessageResourceName = "ErrorRequired", ErrorMessageResourceType = typeof(Helpers_SharedResource))
            ]
            [StringLength(50, ErrorMessageResourceName = "ErrorMinLength",
                 ErrorMessageResourceType = typeof(Helpers_SharedResource), MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password", ResourceType = typeof(Helpers_SharedResource))]
            public string Password { get; set; }

        }

        public RegisterModel(IUserService userService, UserManager<User> userManager, SignInManager<User> signInManager, MyServerDbContext dbContext, IHttpContextAccessor httpContextAccessor) : base(userService, userManager, signInManager, dbContext, httpContextAccessor)
        {
        }
    }
}