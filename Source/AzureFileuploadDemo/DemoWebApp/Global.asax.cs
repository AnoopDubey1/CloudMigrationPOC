using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace DemoWebApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            var certs = new[] {"73db78163c244cf21a3d33b725570878f7b37654", "701dc38e61d9b338aefe2a09fb943d291f66c122"};
            //Todo: Can remove this if reporting server is using valid certificate
            ServicePointManager.ServerCertificateValidationCallback += delegate(
                object s,
                X509Certificate certificate,
                X509Chain chain,
                SslPolicyErrors sslPolicyErrors
            )
            {
                if (certs.Any(a=>a.Equals(certificate.GetCertHashString(),StringComparison.InvariantCultureIgnoreCase)))
                {
                    return true;
                }
                return sslPolicyErrors == SslPolicyErrors.None;
            };
        }

        protected void Session_Start(object sender, EventArgs args)
        {
            //In session start or after login store traceid into session
            Session["traceid"] = Guid.NewGuid();
        }
    }
}
