namespace MyServer.Web.Api.Areas.Account.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Cors;
    using System.Web.Http.Description;

    using Microsoft.AspNet.Identity;

    using MyServer.Common;
    using MyServer.Data.Models;
    using MyServer.Services.Users;
    using MyServer.Web.Api.Areas.Account.Models;

    public class RegisterController : BaseController
    {
        [HttpPost]
        [AllowAnonymous]
        [ResponseType(typeof(User))]
        [Route("Account/Register")]
        public async Task<IHttpActionResult> Post(AccountRegisterBindingModel model)
        {
            try
            {
                if (model == null || !this.ModelState.IsValid)
                {
                    return this.BadRequest(this.ModelState);
                }

                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    NotificationMask = model.NotificationMask,
                    CreatedOn = DateTime.UtcNow
                };

                var result = await this.UserManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    return this.GetErrorResult(result);
                }
                else
                {
                    this.UserManager.AddToRole(user.Id, MyServerRoles.User);
                }

                return this.Ok();
            }
            catch (Exception ex)
            {
                return this.InternalServerError(ex);
            }
        }
    }
}