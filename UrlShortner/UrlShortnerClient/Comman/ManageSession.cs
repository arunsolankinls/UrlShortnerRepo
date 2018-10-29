using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UrlShortnerClient.Comman
{
    public static class ManageSession
    {
        public static string SessionUserName
        {
            get
            {
                if (HttpContext.Current.Session["UserName"] != null && HttpContext.Current.Session["UserName"].ToString().Length > 0)
                {

                }
                return "";
            }
            set
            {
                HttpContext.Current.Session["UserName"] = value;
            }

        }
        public static long SessionUserId
        {
            get
            {
                if (HttpContext.Current.Session["UserId"] != null && HttpContext.Current.Session["UserId"].ToString().Length > 0)
                {

                }
                return 0;
            }
            set
            {
                HttpContext.Current.Session["UserId"] = value;
            }
        }
        public static string SessionUrlList
        {

            get
            {
                if (HttpContext.Current.Session["UrlList"] != null && HttpContext.Current.Session["UrlList"].ToString().Length > 0)
                {

                }
                return "";
            }
            set
            {
                HttpContext.Current.Session["UrlList"] = value;
            }
        }

        public static bool SessionPaymentStatus
        {

            get
            {
                bool PaymentStatus;
                if (bool.TryParse(HttpContext.Current.Session["PaymentStatus"].ToString(), out PaymentStatus) == false)
                {
                    return false;
                }
                return PaymentStatus;
            }            
        }

    }
}