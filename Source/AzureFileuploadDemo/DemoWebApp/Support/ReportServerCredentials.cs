using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;

namespace DemoWebApp.Support
{
    /// <summary>
    /// Required for authenticating with Reporting server.
    /// </summary>
    [Serializable]
    public class ReportServerCredentials : Microsoft.Reporting.WebForms.IReportServerCredentials
    {
        public string UserName { get;private set; }
        public string Password { get; private set; }
        public string Domain { get;private set; }

        public ReportServerCredentials()
        {
            var reportsrvCred = ConfigurationManager.AppSettings["reportsrvact"].Split('\\');
            UserName = reportsrvCred[0];
            Password = reportsrvCred[1];
            Domain = reportsrvCred[2];
        }

        public ReportServerCredentials(string userName, string password, string domain)
        {
            UserName = userName;
            Password = password;
            Domain = domain;
        }
        public System.Security.Principal.WindowsIdentity ImpersonationUser
        {
            get
            {
                return null;
            }
        }
        public System.Net.ICredentials NetworkCredentials
        {
            get
            {
                return new System.Net.NetworkCredential(UserName, Password, Domain);
            }
        }
        public bool GetFormsCredentials(out System.Net.Cookie authCoki, out string userName, out string password, out string authority)
        {
            userName = UserName;
            password = Password;
            authority = Domain;
            authCoki = new System.Net.Cookie(".ASPXAUTH", ".ASPXAUTH", "/", "Domain");
            return true;
        }

    }
}