using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyServer.Web.Main.Areas.ImageGallery.Controllers
{
    public class BaseController : Controller
    {
        // GET: ImageGallery/Base
        public ActionResult Index()
        {
            return View();
        }
    }
}