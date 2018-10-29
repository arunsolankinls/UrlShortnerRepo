using Stripe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using UrlShortner.Database;

namespace UrlShortnerClient
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
            StripeConfiguration.SetApiKey(ConfigurationManager.AppSettings["SecretKey"]);
            Database.SetInitializer<ShortnerContext>(null);
            //Database.SetInitializer<ShortnerContext>(new DropCreateDatabaseIfModelChanges<ShortnerContext>());
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            Session["UserName"] = "";
            Session["UserId"] = 0;            
            Session["UrlList"] = "";
            Session["PaymentStatus"] = false;
            Session["SelectedPlans"] = 0;
        }
    }
}
