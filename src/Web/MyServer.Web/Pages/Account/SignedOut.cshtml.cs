namespace MyServer.Web.Pages.Account
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;

    public class SignedOutModel : PageModel
    {
        public IActionResult OnGet()
        {
            if (this.User.Identity.IsAuthenticated)
            {
                // Redirect to home page if the user is authenticated.
                return this.RedirectToPage("/Index");
            }

            return this.Page();
        }
    }
}