using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UrlShortner.Comman;

namespace UrlShortner.Controllers
{
    public class BaseController : Controller
    {
        protected override void ExecuteCore()
        {
            //var domain = Request.Url.AbsoluteUri;
            var domain = string.Format("{0}://{1}{2}",
            HttpContext.Request.Url.Scheme,
            HttpContext.Request.Url.Authority, Url.Content("~"));

            string domain_persian = ConfigurationManager.AppSettings["HostUrl2"].ToString();
            bool IsPersian = false;

            if (domain.Contains(domain_persian))
                IsPersian = true;

            if (Session["CurrentCulture"] == null)
            {
                if (IsPersian == true)
                    Session["CurrentCulture"] = 1;
                else
                    Session["CurrentCulture"] = 0;
            }

            int culture = 0;
            if (this.Session == null || this.Session["CurrentCulture"] == null)
            {
                int.TryParse(System.Configuration.ConfigurationManager.AppSettings["Culture"], out culture);
                this.Session["CurrentCulture"] = culture;
            }
            else
            {
                culture = (int)this.Session["CurrentCulture"];
            }
            // calling CultureHelper class properties for setting
            CultureHelper.CurrentCulture = culture;

            base.ExecuteCore();
        }

        protected override bool DisableAsyncSupport
        {
            get { return true; }
        }
    }
}