namespace MyServer.Web.Areas.Account.Models
{
    using System.ComponentModel.DataAnnotations;

    using MyServer.Web.Helpers;
    using MyServer.Web.Resources;

    public class AccountRegisterViewModel
    {
        [DataType(DataType.Password)]
        [Display(Name = "ConfirmPassword", ResourceType = typeof(Helpers_SharedResource))]
        [Compare("Password", ErrorMessageResourceName = "ErrorPasswordNotMatch", ErrorMessageResourceType = typeof(Helpers_SharedResource))]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessageResourceName = "ErrorRequired", ErrorMessageResourceType = typeof(Helpers_SharedResource))]
        [EmailAddress]
        [Display(Name = "Email", ResourceType = typeof(Helpers_SharedResource))]
        public string Email { get; set; }

        [Required(ErrorMessageResourceName = "ErrorRequired", ErrorMessageResourceType = typeof(Helpers_SharedResource))]
        [StringLength(50, ErrorMessageResourceName = "ErrorLength", ErrorMessageResourceType = typeof(Helpers_SharedResource), MinimumLength = 2)]
        [Display(Name = "FirstName", ResourceType = typeof(Helpers_SharedResource))]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceName = "ErrorRequired", ErrorMessageResourceType = typeof(Helpers_SharedResource))]
        [StringLength(50, ErrorMessageResourceName = "ErrorLength", ErrorMessageResourceType = typeof(Helpers_SharedResource), MinimumLength = 2)]
        [Display(Name = "LastName", ResourceType = typeof(Helpers_SharedResource))]
        public string LastName { get; set; }

        [Required(ErrorMessageResourceName = "ErrorRequired", ErrorMessageResourceType = typeof(Helpers_SharedResource))]
        [StringLength(50, ErrorMessageResourceName = "ErrorMinLength", ErrorMessageResourceType = typeof(Helpers_SharedResource), MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(Helpers_SharedResource))]
        public string Password { get; set; }
    }
}