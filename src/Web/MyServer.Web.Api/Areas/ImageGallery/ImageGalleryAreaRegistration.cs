﻿using System.Web.Mvc;

namespace MyServer.Web.Api.Areas.ImageGallery
{
    public class ImageGalleryAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ImageGallery";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {

        }
    }
}