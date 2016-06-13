namespace MyServer.Web.Main.Controllers
{
    using System.Web.Mvc;

    using MyServer.Services.ImageGallery;

    public class HomeController : Controller
    {
        private ILocationService locationService;

        public HomeController(ILocationService locationService)
        {
            this.locationService = locationService;
        }

        public ActionResult Index()
        {
            // System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
            // message.To.Add("atanasgeorgiev83@gmail.com");
            // message.Subject = "This is the Subject line";
            ////message.From = new System.Net.Mail.MailAddress("From@online.microsoft.com");
            // message.Body = "This is the message body";
            // System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
            // smtp.Send(message);
            return this.View();
        }
    }
}