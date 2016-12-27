using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyServer.Data;
using MyServer.Data.Models;
using MyServer.Services.Users;
using MyServer.Web.Areas.Shared.Controllers;
using System.IO;
using System.Linq;

using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MyServer.Web.Areas.FileManager.Models;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;

namespace MyServer.Web.Areas.FileManager.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("FileManager")]
    public class JsonController : BaseController
    {
        private readonly IHostingEnvironment env;

        public JsonController(IHostingEnvironment env, IUserService userService, UserManager<User> userManager, SignInManager<User> signInManager, MyServerDbContext dbContext) : base(userService, userManager, signInManager, dbContext)
        {
            this.env = env;
            
        }

        public JsonResult Read(string id)
        {
            IEnumerable<KendoTreeViewViewModel> res = null;

            if (id == null)
            {
                res = DirSearch(this.env.WebRootPath);
            }
            else
            {
                res = DirSearch(id);
            }
            
            return Json(res);        
        }

        private static List<KendoTreeViewViewModel> list = new List<KendoTreeViewViewModel>();

        static List<KendoTreeViewViewModel> DirSearch(string sDir)
        {
            var dirs = new List<KendoTreeViewViewModel>();

            foreach (string directory in Directory.GetDirectories(sDir))
            {
                var subDir = DirSearch(directory);

                dirs.Add(new KendoTreeViewViewModel()
                {
                    id = directory,
                    Name = directory.Split('\\').LastOrDefault(),
                    hasChildren = subDir.Any(),
                    Children = subDir
                });
            }

            list.AddRange(dirs);

            return dirs;
        }
    }
}
