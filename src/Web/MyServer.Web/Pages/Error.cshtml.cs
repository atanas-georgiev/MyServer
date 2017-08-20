using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyServer.Web.Pages
{
    public class ErrorModel : PageModel
    {
        public new int? StatusCode = 0;

        public void OnGet(int? statusCode)
        {
            this.StatusCode = statusCode;
        }
    }
}