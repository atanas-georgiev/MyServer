using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace MyServer.Web.Controllers
{
    public class ErrorController : Controller
    {
        //private static Logger Logger = LogManager.GetCurrentClassLogger();

        [HttpGet("/Error/StatusCode/{statusCode}")]
        public IActionResult Code(int statusCode)
        {
            //var user = string.IsNullOrEmpty(this.User.Identity.Name) ? "Annonymous" : this.User.Identity.Name;
            //Logger.Error("Error code " + statusCode + " user -> " + user);
            var reExecute = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            return View("Index", statusCode);
        }
    }
}
