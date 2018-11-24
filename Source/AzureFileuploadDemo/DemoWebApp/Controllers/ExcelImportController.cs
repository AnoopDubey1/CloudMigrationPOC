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
        // GET: ExcelImport
        public ActionResult Index()
        {
            List<SaleOrder> saleorderlist = new List<SaleOrder>();
            string excelPath = ConfigurationManager.AppSettings["exceppath"];
            var obj = new ExcelToEntityConversion();
            saleorderlist = obj.GetClassFromExcel<SaleOrder>(excelPath, 1, 1, 1);
            

            return View();
        }
    }
}