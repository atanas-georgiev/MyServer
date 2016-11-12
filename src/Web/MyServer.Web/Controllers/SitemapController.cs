using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MyServer.Services.ImageGallery;
using SimpleMvcSitemap;
using SimpleMvcSitemap.Images;
using SimpleMvcSitemap.Translations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyServer.Web.Controllers
{
    public class SitemapController : Controller
    {
        private readonly IAlbumService albumService;

        private readonly IHostingEnvironment env;

        public SitemapController(IAlbumService albumService, IHostingEnvironment env)
        {
            this.albumService = albumService;
            this.env = env;
        }

        public ActionResult Index()
        {
            List<SitemapNode> nodes = new List<SitemapNode>
            {
                new SitemapNode(Url.Action("Index", "Home"))
                {
                    ChangeFrequency = ChangeFrequency.Daily,
                    LastModificationDate = DateTime.UtcNow,
                    Priority = 1M,
                    Translations = new List<SitemapPageTranslation>
                    {
                        new SitemapPageTranslation(Url.Action("Index", "Home") + "?culture=bg-BG", "bg"),
                        new SitemapPageTranslation(Url.Action("Index", "Home") + "?culture=en-US", "en")
                    }
                },
                new SitemapNode("/ImageGallery/Album")
                {
                    ChangeFrequency = ChangeFrequency.Daily,
                    LastModificationDate = DateTime.UtcNow,
                    Priority = 0.9M,
                    Translations = new List<SitemapPageTranslation>
                    {
                        new SitemapPageTranslation("/ImageGallery/Album?culture=bg-BG", "bg"),
                        new SitemapPageTranslation("/ImageGallery/Album?culture=en-US", "en")
                    }
                }
            };

            foreach (var album in albumService.GetAllReqursive())
            {
                var albumNode = new SitemapNode("/ImageGallery/Album/Details/" + album.Id)
                {
                    ChangeFrequency = ChangeFrequency.Weekly,
                    LastModificationDate = album.CreatedOn,
                    Priority = 0.8M,
                    Translations = new List<SitemapPageTranslation>
                    {
                        new SitemapPageTranslation("/ImageGallery/Album/Details/" + album.Id + "?culture=bg-BG", "bg"),
                        new SitemapPageTranslation("/ImageGallery/Album/Details/" + album.Id + "?culture=en-US", "en"),
                    }
                };

                var albumImages = new List<SitemapImage>();

                foreach (var image in album.Images)
                {
                    albumImages.Add(new SitemapImage("/Content/Images/" + album.Id + "/Original/" + image.FileName) {
                        Location = image.ImageGpsData?.LocationName
                    });
                }

                albumNode.Images = albumImages;

                nodes.Add(albumNode);
            }

            var sitemap = new SitemapModel(nodes);
            return new SitemapProvider().CreateSitemap(sitemap);
        }
    }
}
