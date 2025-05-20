using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Http;
using System.Web.Optimization;
using WebHome.Helper.Jobs;
using Utility;
using System.Net;
using WebHome.Properties;

namespace WebHome
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            JobLauncher.StartUp();
            ServicePointManager.ServerCertificateValidationCallback = (s, cert, chain, policy) => { return true; };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            if (AppSettings.Default.UseDKCMSMessageDispatcher)
                JobLauncher.DKCMSMessageDispatcher.DelayNotify(10);
        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs
            var ex = Server.GetLastError();
            if (ex != null)
                Logger.Error(ex);
        }
    }
}