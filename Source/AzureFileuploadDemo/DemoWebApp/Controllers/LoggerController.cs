using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.ModelBinding;
using System.Web.Mvc;
using System.Web.Routing;
using Serilog;

namespace DemoWebApp.Controllers
{

    public class TraceInitAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session["traceid"] is Guid traceidguid)
            {
                System.Diagnostics.Trace.CorrelationManager.ActivityId = traceidguid;
            }

            base.OnActionExecuting(filterContext);

        }
    }

    //public class BaseController : Controller
    //{
    //    protected override IAsyncResult BeginExecute(RequestContext requestContext, AsyncCallback callback, object state)
    //    {
    //        if (requestContext.HttpContext.Session["traceid"] is Guid traceidguid)
    //        {
    //            System.Diagnostics.Trace.CorrelationManager.ActivityId = traceidguid;
    //        }
    //        return base.BeginExecute(requestContext, callback, state);
    //    }
    //}

    [TraceInitAttribute]
    public class LoggerController : Controller
    {
        private readonly ILogger _logger;
        public LoggerController(ILogger logger)
        {
            this._logger = logger;
        }

        // GET: Logger
        public ActionResult Index()
        {
            ViewBag.Message = TempData["message"];
            return View();
        }


        [HttpPost]
        public ActionResult Log([Form]int logType)
        {
            var str = "";
            
            switch (logType)
            {
                case 1:
                    str = $"Logging Information @ {DateTime.Now}";
                    System.Diagnostics.Trace.TraceInformation(str);
                    _logger.Information(str);
                    break;
                case 2:
                    str = $"Logging Warning @ {DateTime.Now}";
                    System.Diagnostics.Trace.TraceWarning(str);
                    _logger.Warning(str);
                    break;
                case 3:
                    str = $"Logging Error @ {DateTime.Now}";
                    System.Diagnostics.Trace.TraceError(str);
                    _logger.Error(new InvalidOperationException(str), str);
                    break;
            }

            if (!string.IsNullOrWhiteSpace(str))
            {
                TempData["message"] = str + $" with trace id:{System.Diagnostics.Trace.CorrelationManager.ActivityId } using attribute.";
               
            }
            return RedirectToAction("Index");
        }
    }
}