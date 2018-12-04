using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using DemoWebApp.Support;
using Microsoft.Reporting.WebForms;

namespace DemoWebApp.Controllers
{
    public class ReportDemoController : Controller
    {
        // GET: ReportDemo
        public ActionResult Index()
        {
            var reportViewer = new ReportViewer()
            {
                ProcessingMode = ProcessingMode.Remote,
                SizeToReportContent = true,
                Width = Unit.Percentage(100),
                Height = Unit.Percentage(100),
            };

            //reportViewer.
            reportViewer.ServerReport.ReportServerCredentials = new ReportServerCredentials();
            var reportParams = new ReportParameter[1];
            //DatabaseConnectionString
            reportParams[0] = new ReportParameter("DatabaseConnectionString", ConfigurationManager.AppSettings["reportconstring"], false);
            // reportViewer.ServerReport.ReportPath = "/SSRSDemo2/Sales Orders";
            reportViewer.ServerReport.ReportPath = "/SSRSDemo2/ProductCategorylistReport";

            reportViewer.ServerReport.ReportServerUrl = new Uri("https://suryadbpoc.centralindia.cloudapp.azure.com/ReportServer/");
            reportViewer.ServerReport.SetParameters(reportParams);

            ViewBag.ReportViewer = reportViewer;

            return View();
        }

        private DataTable GetData()
        {
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.AppSettings["reportconstring"].Replace("Homefurnishing","AdventureWorks2016")))
            {
                sqlcon.Open();
                var sqladap = new SqlDataAdapter(@"SELECT   
   soh.OrderDate AS [Date],   
   soh.SalesOrderNumber AS [Order],   
   pps.Name AS Subcat, pp.Name as Product,    
   SUM(sd.OrderQty) AS Qty,  
   SUM(sd.LineTotal) AS LineTotal  
FROM Sales.SalesPerson sp   
   INNER JOIN Sales.SalesOrderHeader AS soh   
      ON sp.BusinessEntityID = soh.SalesPersonID  
   INNER JOIN Sales.SalesOrderDetail AS sd   
      ON sd.SalesOrderID = soh.SalesOrderID  
   INNER JOIN Production.Product AS pp   
      ON sd.ProductID = pp.ProductID  
   INNER JOIN Production.ProductSubcategory AS pps   
      ON pp.ProductSubcategoryID = pps.ProductSubcategoryID  
   INNER JOIN Production.ProductCategory AS ppc   
      ON ppc.ProductCategoryID = pps.ProductCategoryID  
GROUP BY ppc.Name, soh.OrderDate, soh.SalesOrderNumber, pps.Name, pp.Name,   
   soh.SalesPersonID  
HAVING ppc.Name = 'Clothing'", sqlcon);
                var data=new DataTable();
                sqladap.Fill(data);

                return data;

            }
        }


        public ActionResult AsPdf()
        {
            ReportViewer reportViewer = new ReportViewer();

            var reportParams = new ReportParameter[1];
            //DatabaseConnectionString
            reportParams[0] = new ReportParameter("ConnectionString", ConfigurationManager.AppSettings["reportconstring"], false);

            reportViewer.LocalReport.ReportPath = Server.MapPath("~/Reports/Sales Orders.rdl");
            reportViewer.LocalReport.SetParameters(reportParams);
            var ReportFormatType = "PDF";
            string deviceinfo =
                "<DeviceInfo>" +
                "   <OutputFormat>" + ReportFormatType + "</OutputFormat>" +
                "</DeviceInfo>";

            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("AdventureWorksDataset", GetData()));
            reportViewer.ProcessingMode = ProcessingMode.Local;
            // var reportdatasource=new ReportDataSource("AdventureWorks2016");
            // reportViewer.LocalReport.DataSources.Add(reportdatasource);

            string mimeType;
            string encoding;
            string fileNameExtension;
            string[] streams;
            Warning[] warnings;
            var pdfBytes = reportViewer.LocalReport.Render(
                ReportFormatType,
                deviceinfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);

            return File(pdfBytes, "application/pdf");
        }
    }
}