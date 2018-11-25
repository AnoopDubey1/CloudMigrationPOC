using DemoWebApp.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;



namespace DemoWebApp.Controllers
{
    public class ExcelImportController : Controller
    {
        public ActionResult Index()
        {      
            string excelPath = ConfigurationManager.AppSettings["exceppath"];

            IList<SaleOrder> saleorderlist = ExcelImporter.ReadToList<SaleOrder>(excelPath, 2, 1);

            return View();
        }
    }
}