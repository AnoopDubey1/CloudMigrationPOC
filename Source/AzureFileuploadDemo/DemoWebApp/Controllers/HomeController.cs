using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DemoWebApp.Support;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;

namespace DemoWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICloudService _cloudService;

        public HomeController(ICloudService cloudService)
        {
            _cloudService = cloudService;
        }


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = $"Your application description page from Environment:{ConfigurationManager.AppSettings["appMode"]}";
            ViewBag.Files = GetFiles();
            return View();
        }

        public IEnumerable<dynamic> GetFiles()
        {
            var uploadDir = _cloudService.GetFileShare("fileupload\\uploads");
            var counter=1;
            foreach (var file in uploadDir.ListFilesAndDirectories().OfType<CloudFile>())
            {
                file.FetchAttributes();
                dynamic eo =new  System.Dynamic.ExpandoObject();
                eo.Sl = counter++;
                eo.Name = file.Metadata["filename"];
                eo.Size = file.Properties.Length;
                eo.id = file.Name;
                yield return eo;
            }
        }

        public async Task File(string id)
        {
            var uploadDir = _cloudService.GetFileShare("fileupload\\uploads"); 
           var file= uploadDir.GetFileReference(id);
            //file.BeginDownloadToStream()
            file.FetchAttributes();

            Response.ContentType = file.Properties.ContentType;
            Response.AddHeader("Content-Disposition", "Attachment;filename=" + file.Metadata["filename"]);
            Response.AddHeader("Content-Length", file.Properties.Length.ToString());
           await file.DownloadToStreamAsync(Response.OutputStream);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = $"Your contact page. Date:{DateTime.Now}";

            return View();
        }

        public ActionResult Cookie()
        {
            Response.Cookies.Add(new HttpCookie("test","test val"));

            return RedirectToAction("Contact");
        }
      


        [HttpPost]
        public async Task<ActionResult> UploadFile(HttpPostedFileBase fileupd)
        {
            if (fileupd?.ContentLength > 0)
            {
                if (fileupd.ContentLength > 1024)
                {
                    ViewBag.OperationResponseMsg = "File shouldn't be more than 1KB";
                    return RedirectToAction("About");
                }
                
                var uploadDir = _cloudService.GetFileShare("fileupload\\uploads",true);

                // Get a reference to the file we created previously.
                CloudFile file = uploadDir.GetFileReference(Guid.NewGuid().ToString("N"));
                file.Metadata["filename"] = Path.GetFileName(fileupd.FileName);
                await file.UploadFromStreamAsync(fileupd.InputStream);
                ViewBag.OperationResponseMsg = "File uploaded successfully.";


            }

            return RedirectToAction("About");
        }
    }
}