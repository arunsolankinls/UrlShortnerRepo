using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using UrlShortner.Database;

namespace UrlShortner
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            UnityConfig.RegisterComponents();
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            System.Data.Entity.Database.SetInitializer<ShortnerContext>(null);
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            Session["UserName"] = "";
            Session["UserId"] = "";
            Session["MemberType"] = "";
            Session["MemberShipType"] = "";
        }
    }
}
