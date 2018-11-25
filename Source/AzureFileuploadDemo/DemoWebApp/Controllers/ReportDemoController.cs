using System;
using System.Collections.Generic;
using System.Configuration;
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
            reportViewer.ServerReport.ReportServerCredentials=new ReportServerCredentials();
            var reportParams = new ReportParameter[1];
            reportParams[0] = new ReportParameter("ConnectionString", ConfigurationManager.AppSettings["reportconstring"],false);
            reportViewer.ServerReport.ReportPath = "/SSRSDemo/Sales Orders";
            reportViewer.ServerReport.ReportServerUrl = new Uri("https://suryadbvm.centralindia.cloudapp.azure.com/ReportServer/");
            reportViewer.ServerReport.SetParameters(reportParams);

            ViewBag.ReportViewer = reportViewer;

            return View();
        }
    }
}