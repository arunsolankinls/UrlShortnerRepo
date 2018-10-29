using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.SessionState;

namespace UrlShortnerClient.Comman
{
    public class CultureHelper
    {
        protected HttpSessionState session;

        public CultureHelper(HttpSessionState httpsessionstate)
        {
            session = httpsessionstate;
        }

        public static int CurrentCulture
        {
            get
            {
                if (Thread.CurrentThread.CurrentUICulture.Name == "en")
                {
                    return 0;
                }
                else if (Thread.CurrentThread.CurrentUICulture.Name == "fa")
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                if (value == 0)
                {
                    //Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-GB");
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
                }
                else if (value == 1)
                {
                    //Thread.CurrentThread.CurrentUICulture = new CultureInfo("fa-SA");
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("fa");
                }
                else
                {
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
                }
                Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture;
            }
        }
    }
}