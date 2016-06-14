namespace MyServer.Web.Main.Controllers
{
    using System;
    using System.Web.Mvc;
    using System.Web.Routing;

    using MyServer.Services.ImageGallery;

    public class HomeController : Controller
    {
        private readonly IImageService imageService;

        private readonly ILocationService locationService;

        public HomeController(ILocationService locationService, IImageService imageService)
        {
            this.locationService = locationService;
            this.imageService = imageService;
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

        protected override IAsyncResult BeginExecute(RequestContext requestContext, AsyncCallback callback, object state)
        {
            var routes = requestContext.RouteData.Values;

            if (routes.ContainsKey("file"))
            {
                this.imageService.PrepareFileForDownload(Guid.Parse(routes["id"].ToString()), requestContext.HttpContext.Server);
            }

            return base.BeginExecute(requestContext, callback, state);
        }
    }
}