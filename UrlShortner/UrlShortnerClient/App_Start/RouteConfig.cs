using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace UrlShortnerClient
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "ShortLink",
                url: "{shortlink}",
                defaults: new { controller = "Home", action = "ShortURL" },
                namespaces: new string[] { "UrlShortnerClient.Controllers" }
             );

            routes.MapRoute(
               name: "Default",
               url: "{controller}/{action}",
               defaults: new { controller = "Home", action = "Index" },
               namespaces: new string[] { "UrlShortnerClient.Controllers" }
           );

            routes.MapRoute(
                name: "Plans",
                url: "plans/index",
                defaults: new { controller = "Plans", action = "Index" },
                namespaces: new string[] { "UrlShortnerClient.Controllers" });

            //routes.MapRoute(
            //    name: "Payment",
            //    url: "payment/index/",
            //    defaults: new { controller = "Payment", action = "Index" },
            //    namespaces: new string[] { "UrlShortnerClient.Controllers" });

            routes.MapRoute(
               name: "Payment",
               url: "payment/purchase",
               defaults: new { controller = "Payment", action = "Purchase" },
               namespaces: new string[] { "UrlShortnerClient.Controllers" });

            routes.MapRoute(
             name: "RedirectToAdmin",
             url: "Account/AdminRedirect",
             defaults: new { controller = "Account", action = "AdminRedirect" },
             namespaces: new string[] { "UrlShortnerClient.Controllers" });

            routes.MapRoute(
                name: "Logout",
                url: "account/logout",
                defaults: new { controller = "Account", action = "Logout" },
                namespaces: new string[] { "UrlShortnerClient.Controllers" }
            );
            routes.MapRoute(
           name: "PaymentSession",
           url: "Plans/PaymentSession/{PaymentType}",
           defaults: new { controller = "Plans", action = "PaymentSession" },
           namespaces: new string[] { "UrlShortnerClient.Controllers" }
       );
          routes.MapRoute(
          name: "PlansZarinpal",
          url: "Plans/zarinpalpay",
          defaults: new { controller = "Plans", action = "zarinpalpay" },
          namespaces: new string[] { "UrlShortnerClient.Controllers" }
      );


            //routes.MapRoute(
            //"ShortUrl",
            //"{shortlink}",
            //new { shortlink = UrlParameter.Optional, action = "ShortURL" }  );            

        }
    }
}
