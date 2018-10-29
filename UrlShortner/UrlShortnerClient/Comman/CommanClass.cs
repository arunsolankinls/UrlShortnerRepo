using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace UrlShortnerClient.Comman
{
    public class CommanClass
    {
        public enum Plans : int
        {
            Plan1 = 15,
            Plan2 = 39,
            Plan3 = 59
        }

        public enum PlanType : int
        {
            Plan1 = 1,
            Plan2 = 2,
            Plan3 = 3
        }

        public enum PaymentStatus : int
        {
            Pending=0,
            Sucess=1,
            Failed = 2
        }

        public enum PaymentMethod : int
        {
            Stripe = 0,
            Zarinpal = 1
        }
    }

    //return plan amount from dollar to rials.
    public static class PlanCalculate
    {
        public static int CalculatePlanAmount(int planamount)
        {
            int plan1 = Convert.ToInt32(ConfigurationManager.AppSettings["Plan1"].ToString());
            int plan2 = Convert.ToInt32(ConfigurationManager.AppSettings["Plan2"].ToString());
            int plan3 = Convert.ToInt32(ConfigurationManager.AppSettings["Plan3"].ToString());

            int plan1_rial = Convert.ToInt32(ConfigurationManager.AppSettings["Plan1_Rial"].ToString());
            int plan2_rial = Convert.ToInt32(ConfigurationManager.AppSettings["Plan2_Rial"].ToString());
            int plan3_rial = Convert.ToInt32(ConfigurationManager.AppSettings["Plan3_Rial"].ToString());

            if (planamount == plan1)
                return plan1_rial = plan1_rial / 10;

            else if (planamount == plan2)
                return plan2_rial = plan2_rial / 10;

            else if (planamount == plan3)
                return plan3_rial = plan3_rial / 10;

            else
                return 0;
        } 
    }
}