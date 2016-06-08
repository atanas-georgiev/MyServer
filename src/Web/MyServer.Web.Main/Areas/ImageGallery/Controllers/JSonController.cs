using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyServer.Web.Main.Areas.ImageGallery.Controllers
{
    public class JsonController : Controller
    {
        // GET: ImageGallery/JSon
        public ActionResult Index()
        {
            return View();
        }
    }
}