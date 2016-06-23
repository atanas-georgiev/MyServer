using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyServer.Web.Api.Areas.Account.Controllers
{
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Cors;
    using System.Web.Http.Description;

    using Microsoft.AspNet.Identity;

    using MyServer.Data.Models;
    using MyServer.Services.Users;
    using MyServer.Web.Api.Areas.Account.Models;
    using MyServer.Web.Api.Controllers;
    using MyServer.Web.Api.Models;
    using MyServer.Web.Infrastructure.Mappings;

    public class UserInfoController : BaseController
    {
        public UserInfoController(IUserService userService)
            : base(userService)
        {
        }

        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [ResponseType(typeof(AccountUserInfoBindingModel))]
        [Route("Account/UserInfo")]
        public async Task<IHttpActionResult> Get()
        {
            try
            {
                var userId = this.User.Identity.GetUserId();
                var user =
                    this.UserService.GetAll()
                        .Where(x => x.Id == userId)
                        .To<AccountUserInfoBindingModel>()
                        .FirstOrDefault();

                if (user == null)
                {
                    return this.NotFound();
                }

                return this.Ok(user);
            }
            catch (Exception ex)
            {
                return this.InternalServerError(ex);
            }
        }

        //public AccountUserInfoBindingModel Get()
        //{
        //    //AccounttController.ExternalLoginData externalLogin = AccounttController.ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

        //    //return new UserInfoViewModel
        //    //{
        //    //    Email = User.Identity.GetUserName(),
        //    //    HasRegistered = externalLogin == null,
        //    //    LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
        //    //};

        //}
    }
}