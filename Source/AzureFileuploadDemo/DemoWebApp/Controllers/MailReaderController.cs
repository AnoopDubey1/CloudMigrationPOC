using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Serilog;

namespace DemoWebApp.Controllers
{
   // [RoutePrefix("api/mailreader")]
    public class MailReaderController : ApiController
    {

        public class MailContent
        {
            public string Name { get; set; }
            public string ContentBytes { get; set; }
            public string ContentType { get; set; }
  
        }

        private readonly ILogger _logger;

        public MailReaderController(ILogger logger)
        {
            _logger = logger;
        }


        [HttpPost]
        //[Route("mail")]
        public IHttpActionResult Mail([FromBody]MailContent mailContent)
        {

            var mailid = Request.Headers.Contains("mailid")? Request.Headers.GetValues("mailid").First():"";
            var mailfrom = Request.Headers.Contains("mailfrom") ? Request.Headers.GetValues("mailfrom").First():"";

            _logger.Warning($"File content:{ System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(mailContent.ContentBytes))}");

            return Content(HttpStatusCode.Accepted, $"Thanks for calling me with attachment {mailContent.Name}.");
        }

    }
}
