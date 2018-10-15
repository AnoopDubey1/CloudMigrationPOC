using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DemoWebApp.Support;

namespace DemoWebApp.Controllers
{
    public class ImageHandlerController : Controller
    {
        private readonly ICloudService _cloudService;
        private readonly string _profilepicpath = "profilepics";

        public ImageHandlerController(ICloudService cloudService)
        {
            _cloudService = cloudService;
        }

        [HttpGet]
        public async Task ProfilePic(string name)
        {
            var dir = _cloudService.GetFileShare(_profilepicpath);
            var imagefile= dir.GetFileReference(name);
            Response.ContentType = imagefile.Properties.ContentType;
            await imagefile.DownloadToStreamAsync(Response.OutputStream);
        }

    }
}