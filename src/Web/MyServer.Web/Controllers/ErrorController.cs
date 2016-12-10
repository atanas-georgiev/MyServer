using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace MyServer.Web.Controllers
{
    public class ErrorController : Controller
    {
        [HttpGet("/Error/StatusCode/{statusCode}")]
        public IActionResult Code(int statusCode)
        {
            var reExecute = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            //_logger.LogInformation($"Unexpected Status Code: {statusCode}, OriginalPath: {reExecute.OriginalPath}");
            return View("Index", statusCode);
        }
    }
}
