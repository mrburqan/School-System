using System;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;

namespace SchoolSystem.Web
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            // ONLY routes & bundles

            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}