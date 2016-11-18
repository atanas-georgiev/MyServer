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

        public JsonResult Read(string path)
        {
            list.Clear();
            DirSearch(this.env.WebRootPath);
            
            return Json(list);            
        }

        private static List<KendoTreeViewViewModel> list = new List<KendoTreeViewViewModel>();

        static void DirSearch(string sDir)
        {
            foreach (string d in Directory.GetDirectories(sDir))
            {
                var files = new List<KendoTreeViewViewModel>();
                foreach (string f in Directory.GetFiles(d))
                {                    
                    files.Add(new KendoTreeViewViewModel()
                    {
                        Id = f,
                        Name = f,
                        HasChildren = false
                    });
                }

                list.Add(new KendoTreeViewViewModel()
                {
                    Id = d,
                    Name = d,
                    HasChildren = true,
                    Children = files
                });
                list.AddRange(files);

                DirSearch(d);
            }
        }
    }
}
