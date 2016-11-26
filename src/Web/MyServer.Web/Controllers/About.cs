using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyServer.Web.Controllers
{
    public class About : Controller
    {
        public IActionResult AboutMe()
        {
            return this.View();
        }

        public IActionResult CV()
        {
            return this.View();
        }

        public IActionResult Contacts()
        {
            return this.View();
        }

        public IActionResult MyProjects()
        {
            return this.View();
        }
    }
}
