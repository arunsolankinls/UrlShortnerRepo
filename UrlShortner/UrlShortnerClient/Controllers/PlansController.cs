using Stripe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UrlShortner.Database;
using UrlShortner.Database.Repository;
//using UrlShortner.Comman;
using UrlShortnerClient.Comman;
using UrlShortnerClient.ServiceReference;

namespace UrlShortnerClient.Controllers
{
    [UserAuthenticationFilter]
    public class PlansController : BaseController
    {
        private readonly IRegistrationRepository _RegistrationRepository;
        public PlansController(IRegistrationRepository RegistrationRepository)
        {
            this._RegistrationRepository = RegistrationRepository;
        }
        // GET: Plans
        public ActionResult Index()
        {
            Session["PaymentStatus"] = false;

            ShortnerContext db = new ShortnerContext();
            PaymentRepository paymentrepo = new PaymentRepository(db);
            int userid = Convert.ToInt32(Session["UserId"]);
            var paymentinfo = paymentrepo.GetPaymentByRegistId(userid);

            var RegisteredUser = _RegistrationRepository.GetClientById(userid);

            if (paymentinfo != null)
            {
                if (paymentinfo.IsSuccess == true|| RegisteredUser.PaymentStatus==true)
                {
                    Session["PaymentStatus"] = true;
                }
            }

            var stripePublishKey = ConfigurationManager.AppSettings["PublishableKey"];
            ViewBag.StripePublishKey = stripePublishKey;

            return View();
        }

        //public ActionResult PaymentSession(string stripeEmail, string stripeToken, FormCollection formvalue)
        public ActionResult PaymentSession(int PaymentType)
        {
            Session["SelectedPlans"] = PaymentType;
            Session["IsPaymentPrecessing"] = "Processing";
            return RedirectToRoute("Payment");
        }
    }
}