using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyServer.Web.Main.Controllers
{
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
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}