namespace MyServer.Web.Api.Areas.Account.Models
{
    using MyServer.Data.Models;
    using MyServer.Web.Infrastructure.Mappings;

    public class AccountUserInfoBindingModel : IMapFrom<User>
    {
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        // public string LoginProvider { get; set; }

        // public bool HasRegistered { get; set; }
    }
}