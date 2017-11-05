namespace MyServer.Web.Pages.Account.Manage
{
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using MyServer.Data;
    using MyServer.Data.Models;
    using MyServer.Services.Users;
    using MyServer.Web.Pages.Base;
    using MyServer.Web.Resources;

    public partial class IndexModel : BasePageModel
    { 
        [BindProperty]
        public InputModel Input { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public string Username { get; set; }

        public IActionResult OnGet()
        {
            if (this.UserProfile != null)
            {
                this.Input = new InputModel()
                                 {
                                     Email = this.UserProfile.Email,
                                     FirstName = this.UserProfile.FirstName,
                                     LastName = this.UserProfile.LastName
                                 };
            }
            else
            {
                this.Input = new InputModel();
            }


            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!this.ModelState.IsValid)
            {
                return this.Page();
            }

            var user = this.UserService.GetAll().FirstOrDefault(x => x.UserName == this.Input.Email);

            if (user != null && user.UserName != this.UserProfile.UserName)
            {
                this.ModelState.AddModelError("Input.Email", Startup.SharedLocalizer["UsernameExist"]);
            }
            else if (this.ModelState.IsValid)
            {
                this.UserProfile.UserName = this.Input.Email;
                this.UserProfile.Email = this.Input.Email;
                this.UserProfile.FirstName = this.Input.FirstName;
                this.UserProfile.LastName = this.Input.LastName;

                this.UserService.Update(this.UserProfile);
            }

            return this.RedirectToPage();
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


        public IndexModel(IUserService userService, UserManager<User> userManager, SignInManager<User> signInManager, MyServerDbContext dbContext, IHttpContextAccessor httpContextAccessor) : base(userService, userManager, signInManager, dbContext, httpContextAccessor)
        {
        }
    }
}