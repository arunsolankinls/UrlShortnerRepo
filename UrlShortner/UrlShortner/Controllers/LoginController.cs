using System.Configuration;
using System.Web.Mvc;
using UrlShortner.Database.Repository;
using UrlShortner.Models;

namespace UrlShortner.Controllers
{
    public class LoginController : BaseController
    {
        // GET: Login
        private readonly IRegistrationRepository _RegistrationRepository;

        public LoginController(IRegistrationRepository RegistrationRepository)
        {
            this._RegistrationRepository = RegistrationRepository;
        }
        public ActionResult Index()
        {
            RegistrationViewModel model = new RegistrationViewModel();
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(RegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var Result = _RegistrationRepository.ClientAuthentication(model.Email, model.Password);
                if (Result != null)
                {
                    if (Result.Email.ToLower() == model.Email.ToLower() && Result.Password == model.Password)
                    {
                        if (!Result.ActiveStatus)
                        {
                            ViewBag.error = "User not active please contact to admin";
                            return View(model);
                        }
                        Session["UserName"] = Result.FirstName + " " + Result.LastName;
                        Session["UserId"] = Result.RegistId;
                        Session["MemberType"] = Result.MemberType;
                        Session["PaymentStatus"] = Result.PaymentStatus;
                        Session["MemberShipType"] = Result.MemberShipType;

                        if (Result.MemberType == (string)ConfigurationManager.AppSettings["AdminType"])
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        else {
                            return RedirectToAction("Index", "Home");
                        }
                        
                    }
                    else
                    {
                        ViewBag.error = "User name password wrong please try again.";
                        return View(model);
                    }
                }
                else
                {
                    ViewBag.error = "User name password wrong please try again.";
                    return View(model);
                }
            }
            else
            {
                return View(model);
            }

        }
        //[AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LoginRequest(string email,string password)
        {
            var Result = _RegistrationRepository.ClientAuthentication(email, password);

            if (Result.Email.ToLower() == email.ToLower() && Result.Password == password)
            {
                if (!Result.ActiveStatus)
                {
                    ViewBag.error = "User not active please contact to admin";
                    return Content(ViewBag.error);
                }
                Session["UserName"] = Result.FirstName + " " + Result.LastName;
                Session["UserId"] = Result.RegistId;
                Session["MemberType"] = Result.MemberType;
                Session["PaymentStatus"] = Result.PaymentStatus;
                Session["MemberShipType"] = Result.MemberShipType;

                if (Result.MemberType == (string)ConfigurationManager.AppSettings["AdminType"])
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }

            }
            else
            {
                ViewBag.error = "User name password wrong please try again.";
                return Content("Not valid credentials.");
            }
        }

        public ActionResult Logout()
        {
            Session["UserName"] = null;
            Session["UserId"] = null;
            return RedirectToAction("Index", "Login");
        }
    }
}