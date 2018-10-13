using System;
using System.Web;

namespace DemoWebApp
{
    public class TraceInitModule:IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.BeginRequest += Context_BeginRequest; ;
        }

        private void Context_BeginRequest(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
            HttpContext requestContext = app.Context;

            Guid traceidguid;
            if (requestContext.Request.Cookies["traceid"] is HttpCookie traceidcookie)
            {
                traceidguid = Guid.Parse(requestContext.Request.Cookies["traceid"].Value);
            }
            else
            {
                traceidguid = Guid.NewGuid();
                requestContext.Response.Cookies.Add(new HttpCookie("traceid", traceidguid.ToString()){HttpOnly = true});
            }
            
            System.Diagnostics.Trace.CorrelationManager.ActivityId = traceidguid;
        }

        public void Dispose()
        {
        }
    }
}