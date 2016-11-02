namespace MyServer.Web.Areas.Account.Models
{
    using System.ComponentModel.DataAnnotations;

    using MyServer.Web.Resources;

    public class AccountLoginViewModel
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