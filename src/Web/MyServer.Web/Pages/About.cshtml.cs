namespace MyServer.Web.Pages
{
    using Microsoft.AspNetCore.Mvc.RazorPages;

    public class AboutModel : PageModel
    {
        public string Message { get; set; }

        public void OnGet()
        {
            this.Message = "Your application description page.";
        }
    }
}