using AutoMapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using UrlShortner.Database.Entities;
using UrlShortner.Database.Repository;
using UrlShortnerClient.Models;
using UrlShortnerClient.Comman;
using System.Configuration;
using Stripe;
using UrlShortner.Database;
using UrlShortnerClient.ServiceReference;
using zarinpal;

namespace UrlShortnerClient.Controllers
{
    [UserAuthenticationFilter]
    public class PaymentController : BaseController
    {
        ShortnerContext db = new ShortnerContext();
        public string ZarinpalPaymentURL = ConfigurationManager.AppSettings["ZarinpalPaymentGatewayURL"].ToString();
        public string ZarinpalCallbackURL = ConfigurationManager.AppSettings["CallbackURL"].ToString();
        public string MerchantID = ConfigurationManager.AppSettings["MerchantID"].ToString();
        public event pay.PArgs OnPaymentAction;
        public long Amount = 0;
        private readonly IAdManageRepository adManageRepository;
        private readonly IRegistrationRepository registrationRepository;
        private readonly IPaymentRepository _paymentRepository;

        public PaymentController(IAdManageRepository AdManageRepository, IRegistrationRepository RegistrationRepository, IPaymentRepository PaymentRepository)
        {
            this.adManageRepository = AdManageRepository;
            this.registrationRepository = RegistrationRepository;
            this._paymentRepository = PaymentRepository;
        }
        public string[] ValidFileTypes = { "html", "htm" };
        // GET: Payment

        [HttpGet]
        public ActionResult Purchase(AdManageViewModel objAdModel)
        {
            PaymentRepository paymentrepo = new PaymentRepository(db);
            var paymentinfo = paymentrepo.GetPaymentByRegistId(Convert.ToInt32(Session["UserId"]));

            objAdModel.Paymentstatus = CommanClass.PaymentStatus.Pending;
            Session["PaymentStatus"] = false;

            if (paymentinfo != null)
            {
                if (paymentinfo.IsSuccess)
                {
                    objAdModel.Paymentstatus = CommanClass.PaymentStatus.Sucess;
                    Session["PaymentStatus"] = true;
                }
                else
                {
                    objAdModel.Paymentstatus = CommanClass.PaymentStatus.Failed;
                }
            }
            else {
                Session["IsPaymentPrecessing"] = "Processing";
            }

            if (Convert.ToInt32(Session["SelectedPlans"]) == Convert.ToInt32(Comman.CommanClass.PlanType.Plan1))
            {
                TempData["PlanAmount"] = Convert.ToInt32(Comman.CommanClass.Plans.Plan1);
            }
            else if (Convert.ToInt32(Session["SelectedPlans"]) == Convert.ToInt32(Comman.CommanClass.PlanType.Plan2))
            {
                TempData["PlanAmount"] = Convert.ToInt32(Comman.CommanClass.Plans.Plan2);
            }
            else { TempData["PlanAmount"] = Convert.ToInt32(Comman.CommanClass.Plans.Plan3); }

            TempData.Keep("PlanAmount");

            //Stripe credentials
            var stripePublishKey = ConfigurationManager.AppSettings["PublishableKey"];
            ViewBag.StripePublishKey = stripePublishKey;

            return View(objAdModel);
        }

        #region Payment with Stripe and store file and data in DB 
        [HttpPost]
        public ActionResult Purchase(string stripeEmail, string stripeToken, AdManageViewModel model)
        {
            AdManage objAdManage = new AdManage();
            var result = new JsonResult();
            
            int selectedplan = Convert.ToInt32(TempData["PlanAmount"]);
            string planname = Enum.GetName(typeof(CommanClass.Plans), selectedplan);
            int planvalue = (int)Enum.Parse(typeof(CommanClass.PlanType), planname.ToString());
            int planamount = selectedplan * 100;
            string FileName = string.Empty;
            int userid = Convert.ToInt32(Session["UserId"]);
            var customers = new StripeCustomerService();
            var charges = new StripeChargeService();

            try
            {
                var RegisteredUser = registrationRepository.GetClientById(userid);

                if (!string.IsNullOrEmpty(stripeEmail))
                {
                    var customer = customers.Create(new StripeCustomerCreateOptions
                    {
                        Email = stripeEmail,
                        SourceToken = stripeToken
                    });

                    var charge = charges.Create(new StripeChargeCreateOptions
                    {
                        Amount = planamount,
                        Description = selectedplan.ToString(),
                        //Currency = "usd",
                        Currency = "eur",
                        CustomerId = customer.Id
                    });

                    //Save Response in PaymentTransaction Table
                    PaymentTransaction payment = new PaymentTransaction()
                    {
                        RegistId = userid,
                        Customer_stripeEmail = stripeEmail,
                        Customer_Id = customer.Id,
                        Customer_stripeToken = stripeToken,
                        Customer_Created = customer.Created,
                        Customer_DefaultSourceId = customer.DefaultSourceId,
                        Customer_InvoicePrefix = customer.InvoicePrefix,
                        Customer_StripeResponse_RequestId = customer.StripeResponse.RequestId,
                        Customer_StripeResponse_ResponseJson = customer.StripeResponse.ResponseJson,

                        Charge_Id = charge.Id,
                        Charge_Amount = charge.Amount,
                        Charge_BalanceTransactionId = charge.BalanceTransactionId,
                        Charge_Created = charge.Created,
                        Charge_CustomerId = charge.CustomerId,
                        Charge_Description = charge.Description,
                        Charge_FailureCode = charge.FailureCode,
                        Charge_FailureMessage = charge.FailureMessage,
                        Charge_Outcome_SellerMessage = charge.Outcome.SellerMessage,
                        Charge_Paid = charge.Paid,
                        Charge_Source_Id = charge.Source.Id,
                        Charge_Status = charge.Status,
                        Charge_StripeResponse_RequestId = charge.StripeResponse.RequestId,
                        Charge_StripeResponse_ResponseJson = charge.StripeResponse.ResponseJson,
                        IsSuccess = true,
                        Response = "Complete",
                        PaymentMethod = (int)CommanClass.PaymentMethod.Stripe
                    };
                    _paymentRepository.AddPayment(payment);
                    //_paymentRepository.SaveAll();

                    //Insert stripe response information in PaymentTransaction table
                    //db.PaymentTransaction.Add(payment);
                    //db.SaveChanges();

                    RegisteredUser.PaymentStatus = true;
                    registrationRepository.Edit(RegisteredUser);

                    if (!string.IsNullOrEmpty(charge.FailureCode))
                    {
                        model.ResponseMessage = "Your subscription failed!!";
                        model.Paymentstatus = CommanClass.PaymentStatus.Failed;
                        return View(model);
                    }
                }
                //for zarinpal payment plugin
                else
                {
                    //Payment with Zarinpal Service reference.
                    string authority = string.Empty;
                    int payment_request_response = 0;
                    PaymentTransaction payment = new PaymentTransaction();
                    try
                    {
                        PaymentGatewayImplementationServicePortTypeClient request = new PaymentGatewayImplementationServicePortTypeClient();
                        Amount = PlanCalculate.CalculatePlanAmount(selectedplan);
                        payment_request_response = request.PaymentRequest(MerchantID, 120, "Test", "", "", ZarinpalCallbackURL, out authority);
                        if (payment_request_response > 0)
                        {
                            if (Request.Files[0].ContentLength > 0)
                            {
                                Guid FileNameGuid = Guid.NewGuid();
                                FileName = FileNameGuid.ToString() + ".html";
                                string _path = Path.Combine(Server.MapPath("~/AdHTML"), FileName);
                                Request.Files[0].SaveAs(_path);
                            }

                            objAdManage = Mapper.Map<AdManageViewModel, AdManage>(model);
                            objAdManage.Addeddate = DateTime.Now.Date;
                            objAdManage.RegistAdId = userid;
                            objAdManage.UpdateDate = DateTime.Now.Date;
                            objAdManage.UpdatedBy = userid;
                            objAdManage.UploadFile = FileName;
                            objAdManage.Description = model.Description;

                            adManageRepository.AddAd(objAdManage);
                            adManageRepository.SaveAll();

                            payment.IsSuccess = false;
                            payment.RegistId = userid;
                            payment.Authority = authority;
                            payment.Payment_RequestResponse = payment_request_response;
                            payment.Charge_Amount = Convert.ToInt32(Amount);
                            //Insert in Payment Table
                            Session["IsPaymentPrecessing"] = false;
                            _paymentRepository.InsertPaymentTransactionData(payment);
                            //Redirect to zarinpal gateway site for payment.
                            return Redirect(ZarinpalPaymentURL + authority);
                        }
                    }
                    catch (Exception ex)
                    {
                        payment.Charge_Amount = Convert.ToInt32(Amount);
                        payment.IsSuccess = false;
                        payment.RegistId = userid;
                        payment.Authority = authority;
                        payment.Payment_RequestResponse = payment_request_response;
                        payment.PaymentRequest_ErrorResponse = ex.ToString();
                        _paymentRepository.InsertPaymentTransactionData(payment);
                        Session["PaymentStatus"] = false;
                        Session["IsPaymentPrecessing"] = false;
                    }
                }

                //Insert in DB
                if (Request.Files[0].ContentLength > 0)
                {
                    //FileName = Convert.ToString(Session["UserId"]) + ".html";
                    Guid FileNameGuid = Guid.NewGuid();
                    FileName = FileNameGuid.ToString() + ".html";
                    string _path = Path.Combine(Server.MapPath("~/AdHTML"), FileName);
                    Request.Files[0].SaveAs(_path);
                }

                // AdManage objAdManage = new AdManage();
                objAdManage = Mapper.Map<AdManageViewModel, AdManage>(model);
                objAdManage.Addeddate = DateTime.Now;
                objAdManage.RegistAdId = Convert.ToInt64(Session["UserId"]);
                objAdManage.UpdateDate = DateTime.Now;
                objAdManage.UpdatedBy = Convert.ToInt32(Session["UserId"]);
                objAdManage.UploadFile = FileName;
                objAdManage.Description = model.Description;

                adManageRepository.AddAd(objAdManage);
                if (adManageRepository.SaveAll())
                {
                    Registration objRegisterUser = registrationRepository.GetClientById(userid);
                    objRegisterUser.PaymentStatus = true;
                    registrationRepository.Edit(objRegisterUser);
                    Session["PaymentStatus"] = true;
                    model.ResponseMessage = "Your subscription succesfully!!";
                    model.Paymentstatus = CommanClass.PaymentStatus.Sucess;
                    return View(model);
                }
                else
                {
                    model.ResponseMessage = "Your subscription failed!!";
                    model.Paymentstatus = CommanClass.PaymentStatus.Failed;
                    Session["PaymentStatus"] = false;
                }
            }
            catch (Exception ex)
            {
                //result.Data = "Your subscription failed!! " + ex.Message.ToString();
                model.ResponseMessage = "Your subscription failed!! " + ex.Message.ToString();
                model.Paymentstatus = CommanClass.PaymentStatus.Failed;
                Session["PaymentStatus"] = false;
            }

            return View(model);
        }

        #endregion

        #region Zarinpal payment
        public ActionResult ZarinpalCallback(string Authority = "", string Status = "")
        {
            ShortnerContext db = new ShortnerContext();
            AdManageViewModel model = new AdManageViewModel();
            PaymentGatewayImplementationServicePortTypeClient request = new PaymentGatewayImplementationServicePortTypeClient();
            long userid = (long)Session["UserId"];
            PaymentRepository paymentrepo = new PaymentRepository(db);
            PaymentTransaction paymenttransaction = new PaymentTransaction();

            long RefID = 0;
            int response = 0;

            try
            {
                var RegisteredUser = registrationRepository.GetClientById(userid);
                paymenttransaction = _paymentRepository.GetPaymentByRegistId(userid);

                if (Status == "OK")
                {
                    Amount = PlanCalculate.CalculatePlanAmount(Convert.ToInt32(TempData["PlanAmount"]));

                    //PAYMENT VERIFICATION
                    response = request.PaymentVerification(MerchantID, Authority, 120, out RefID);

                    if (RefID > 0)
                    {
                        //CheckPaymentStatus(Authority);
                        Registration objRegisterUser = registrationRepository.GetClientById(userid);
                        objRegisterUser.PaymentStatus = true;
                        registrationRepository.Edit(objRegisterUser);
                        Session["PaymentStatus"] = true;
                        model.ResponseMessage = "Your subscription succesfully!!";
                        model.Paymentstatus = CommanClass.PaymentStatus.Sucess;
                        paymenttransaction.RegistId = userid;
                        paymenttransaction.Charge_Paid = true;
                        paymenttransaction.IsSuccess = true;
                        paymenttransaction.PaymentMethod = (int)CommanClass.PaymentMethod.Zarinpal;
                        paymenttransaction.Response = "Complete";
                        paymenttransaction.Status = Status;
                        paymenttransaction.ReferenceID =RefID;
                        paymenttransaction.PaymentVerificationResponse = response;
                        paymenttransaction.Charge_Amount = Convert.ToInt32(Amount);
                        _paymentRepository.UpdatePaymentTransactionData(paymenttransaction);

                        RegisteredUser.PaymentStatus = true;
                        registrationRepository.Edit(RegisteredUser);
                        Session["IsPaymentPrecessing"] = true;
                        return RedirectToAction("Purchase", "Payment", model);
                    }
                }
                if (Status == "NOK")
                {
                    paymenttransaction.RegistId = userid;
                    paymenttransaction.Charge_Amount = Convert.ToInt32(Amount);
                    paymenttransaction.Charge_Paid = false;
                    paymenttransaction.ExpireDate = DateTime.Now.AddDays(30);
                    paymenttransaction.IsSuccess = false;
                    paymenttransaction.PaymentMethod = (int)CommanClass.PaymentMethod.Zarinpal;
                    paymenttransaction.Response = "Failed";
                    paymenttransaction.Status = Status;
                    paymenttransaction.ReferenceID = RefID;
                    paymenttransaction.Authority = Authority;
                    paymenttransaction.PaymentVerificationResponse = response;
                    _paymentRepository.UpdatePaymentTransactionData(paymenttransaction);
                    Session["IsPaymentPrecessing"] = false;
                   
                    RegisteredUser.PaymentStatus = false;
                    registrationRepository.Edit(RegisteredUser);

                    model.ResponseMessage = "Your subscription failed!!";
                    model.Paymentstatus = CommanClass.PaymentStatus.Failed;
                    Session["PaymentStatus"] = false;
                }
            }
            catch (Exception ex)
            {
                paymenttransaction.RegistId = userid;
                paymenttransaction.Charge_Amount = Convert.ToInt32(Amount);
                paymenttransaction.Charge_Paid = false;
                paymenttransaction.IsSuccess = false;
                paymenttransaction.PaymentMethod = (int)CommanClass.PaymentMethod.Zarinpal;
                paymenttransaction.Response = "Failed" + ex.ToString();
                paymenttransaction.Status = Status;
                paymenttransaction.ReferenceID = RefID;
                paymenttransaction.Authority = Authority;
                paymenttransaction.PaymentVerificationResponse = Convert.ToInt32(response);
                _paymentRepository.UpdatePaymentTransactionData(paymenttransaction);

                model.ResponseMessage = "Your subscription failed!" + ex;
                model.Paymentstatus = CommanClass.PaymentStatus.Failed;
                Session["PaymentStatus"] = false;
                Session["IsPaymentPrecessing"] = false;
            }
            return RedirectToAction("Purchase", "Payment", model);
        }

        //Check payment status from authority
        private void CheckPaymentStatus(string autohority)
        {
            PaymentGatewayImplementationServicePortTypeClient request = new PaymentGatewayImplementationServicePortTypeClient();
            long refID = -1;
            bool stopit = false;
            long curtick = DateTime.Now.Ticks / TimeSpan.TicksPerSecond;
            while (true)
            {
                if (stopit)
                    break;
                int verf = -21;
                try
                {
                    verf = request.PaymentVerification(MerchantID, autohority, 120, out refID);
                }
                catch (Exception ex)
                {

                }
                if (verf > 0)
                {
                    stopit = true;
                    if (OnPaymentAction != null)
                    {
                        OnPaymentAction(this, new pay.PayArgs(verf, autohority, refID));
                    }
                }
                else
                {
                    if (!stopit && verf != -21)
                    {
                        stopit = true;
                        if (OnPaymentAction != null)
                        {
                            OnPaymentAction(this, new pay.PayArgs(verf, autohority, refID));
                        }
                    }
                }
                long curtime = DateTime.Now.Ticks / TimeSpan.TicksPerSecond;
                if ((curtime - curtick) > 1850) // 30 * 60 +- 50
                {
                    if (!stopit)
                    {
                        OnPaymentAction(this, new pay.PayArgs(-22, autohority, refID));
                        stopit = true;
                    }
                }
            }
        }

        #endregion

        #region Old Payment Transaction with ActionResult
        ////public JsonResult Purchase(AdManageViewModel model)
        //public ActionResult Purchase(string stripeEmail, string stripeToken, AdManageViewModel model)
        //{
        //    var result = new JsonResult();

        //    //HTML File upload
        //    //string filename = string.Empty;
        //    //Stream fs = model.File.InputStream;
        //    //BinaryReader br = new BinaryReader(fs);
        //    //byte[] bytes = br.ReadBytes((Int32)fs.Length);

        //    int selectedplan = Convert.ToInt32(TempData["PlanAmount"]);
        //    string planname = Enum.GetName(typeof(CommanClass.Plans), selectedplan);
        //    int planvalue = (int)Enum.Parse(typeof(CommanClass.PlanType), planname.ToString());
        //    int planamount = selectedplan * 100;

        //    //Session["SelectedPlans"] = planvalue;

        //    var customers = new StripeCustomerService();
        //    var charges = new StripeChargeService();

        //    var customer = customers.Create(new StripeCustomerCreateOptions
        //    {
        //        Email = stripeEmail,
        //        SourceToken = stripeToken
        //    });

        //    var charge = charges.Create(new StripeChargeCreateOptions
        //    {
        //        Amount = planamount,
        //        Description = selectedplan.ToString(),
        //        Currency = "usd",
        //        CustomerId = customer.Id
        //    });

        //    if (!string.IsNullOrEmpty(charge.FailureCode))
        //    {
        //        result.Data = "Your subscription failed!!";
        //        result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        //        //return result;
        //        model.message= "Your subscription failed!!";
        //    }

        //    //Insert into Db Operation
        //    string FileName = string.Empty;
        //    try
        //    {
        //        if (Request.Files[0].ContentLength > 0)
        //        {
        //            //FileName = Convert.ToString(Session["UserId"]) + ".html";
        //            Guid FileNameGuid = Guid.NewGuid();
        //            FileName = FileNameGuid.ToString() + ".html";
        //            string _path = Path.Combine(Server.MapPath("~/AdHTML"), FileName);
        //            Request.Files[0].SaveAs(_path);
        //        }

        //        AdManage objAdManage = new AdManage();
        //        objAdManage = Mapper.Map<AdManageViewModel, AdManage>(model);
        //        objAdManage.Addeddate = DateTime.Now.Date;
        //        objAdManage.RegistAdId = Convert.ToInt64(Session["UserId"]);
        //        objAdManage.UpdateDate = DateTime.Now.Date;
        //        objAdManage.UpdatedBy = Convert.ToInt32(Session["UserId"]);
        //        objAdManage.UploadFile = FileName;
        //        objAdManage.Description = model.Description;

        //        adManageRepository.AddAd(objAdManage);
        //        if (adManageRepository.SaveAll())
        //        {
        //            long UrlId = (long)Session["UserId"];
        //            Registration objRegisterUser = registrationRepository.GetClientById(UrlId);
        //            objRegisterUser.PaymentStatus = true;
        //            registrationRepository.Edit(objRegisterUser);
        //            Session["PaymentStatus"] = true;
        //            //result.Data = "Your subscription succesfully!!";
        //            model.message = "Your subscription succesfully!!";
        //        }
        //        else
        //        {
        //            //result.Data = "Your subscription failed!!";
        //            model.message = "Your subscription failed!!";
        //            Session["PaymentStatus"] = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //result.Data = "Your subscription failed!! " + ex.Message.ToString();
        //        model.message = "Your subscription failed!! " + ex.Message.ToString();
        //        Session["PaymentStatus"] = false;
        //    }
        //    //result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        //    //return result;
        //    return RedirectToAction("Index","Home");
        //}
        #endregion
    }
}