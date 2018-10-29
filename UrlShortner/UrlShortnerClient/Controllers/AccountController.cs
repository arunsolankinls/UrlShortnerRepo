using AutoMapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
//using System.Web;
using System.Web.Mvc;
using UrlShortner.Database;
using UrlShortner.Database.Entities;
using UrlShortner.Database.Repository;
using UrlShortnerClient.Comman;
using UrlShortnerClient.Models;

namespace UrlShortnerClient.Controllers
{
    public class AccountController : BaseController
    {
        public static ShortnerContext _context = new ShortnerContext();

        private readonly IRegistrationRepository _RegistrationRepository;
        // GET: Account
        public AccountController(IRegistrationRepository RegistrationRepository)
        {
            this._RegistrationRepository = RegistrationRepository;
        }
        public ActionResult Index()
        {
            RegistrationViewModel model = new RegistrationViewModel();
            return View(model);
        }
        public ActionResult Logout()
        {
            Session["UserName"] = "";
            Session["UserId"] = "";
            Session["UrlList"] = "";
            return RedirectToAction("Index", "Home");
        }
        public ActionResult ChangeCurrentCulture(int id)
        {
            // Change the current culture for this user.
            //
            CultureHelper.CurrentCulture = id;
            //
            // Cache the new current culture into the user HTTP session. 
            //
            Session["CurrentCulture"] = id;
            //
            // Redirect to the same page from where the request was made! 
            //
            return Redirect(Request.UrlReferrer.ToString());
        }
        [HttpGet]
        public ActionResult Login()
        {
            LoginViewModel model = new LoginViewModel();
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Login(LoginViewModel model)
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
                            TempData["Error"] = "User not active please contact to admin";
                            TempData.Keep();
                            return View(model);
                        }
                        Session["UserName"] = Result.FirstName + " " + Result.LastName;
                        Session["UserId"] = Result.RegistId;
                        Session["MemberType"] = Result.MemberType;
                        Session["MemberShipType"] = Result.MemberShipType;
                        Session["PaymentStatus"] = Result.PaymentStatus;

                        //save Email password in session
                        Session["Email"] = Result.Email;
                        Session["Password"] = Result.Password;

                        if (Result.MemberType == "SuppeAdmin")
                        {
                            return RedirectToAction("Index", "Client");
                        }
                        else
                        {
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

        [HttpGet]
        public ActionResult Signup()
        {
            RegistrationViewModel model = new RegistrationViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Signup(RegistrationViewModel model)
        {

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                bool IsExist = _RegistrationRepository.IsEmailExist(model.Email);

                if (IsExist == true)
                {
                    ViewBag.errorMessage = "Email already exist.";
                    return View(model);
                }

                Registration objRegistration = new Registration();
                objRegistration.Email = model.Email;
                objRegistration.Password = model.Password;
                objRegistration.FirstName = model.FirstName;
                objRegistration.UpdatedDate = DateTime.Now.Date;
                objRegistration.ExpireDate = DateTime.Now.Date;
                objRegistration.ActiveStatus = true;
                objRegistration.MemberType = ConfigurationManager.AppSettings["MemberType"];
                objRegistration.MemberShipType = ConfigurationManager.AppSettings["MemberShipType"];
                objRegistration.PaymentStatus = false;
                _RegistrationRepository.AddClient(objRegistration);
                _RegistrationRepository.SaveAll();
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                ViewBag.errorMessage = ex.Message + "Fail Reigistration";
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult AdminRedirect()
        {
            string email = Session["Email"].ToString();
            string password = Session["Password"].ToString();
            string adminurl = HttpContext.Request.Url.GetLeftPart(UriPartial.Authority);

            //current statis path for redirect to admin login
            string request = string.Format(adminurl + "/Admin/Login/LoginRequest" + "?email={0}&password={1}", email, password);
            //Response.Flush(); 
            Response.Redirect(request);

            return RedirectToAction("Index", "Home");
        }

        #region PasswordForget and reset password
        public ActionResult ForgetPassword()
        {
            RegistrationViewModel model = new RegistrationViewModel();

            return View();
        }

        [HttpPost]
        public ActionResult ForgetPassword(RegistrationViewModel model)
        {

            if (string.IsNullOrEmpty(model.Email))
            {
                ViewBag.errorMessage = "Email is not empty!";
                return View(model);
            }
            try
            {
                var isEmailExist = _RegistrationRepository.GetUserByEmail(model.Email);
                if (isEmailExist == null)
                {
                    ViewBag.errorMessage = "Email is not exist in record.";
                    return View(model);
                }

                string token = Guid.NewGuid().ToString();

                //create link for send user.
                string linkhref = "<a href='" + Url.Action("ResetPassword", "Account", new { email = model.Email, code = token }, "http") + "'> Reset password</a>";
                string subject = "Your changed password";
                string body = "<b>Please find the Password reset link.</b> <br/>" + linkhref;
                string newbody= "<br/>We have sent you this email in response to your request to reset your password on <a href=ijl.li>ijl.li </a>. <br/> To reset your password for <a href=ijl.li>ijl.li</a>, please follow the link below: <br/>"+linkhref;
                   //Send Mail function to user with password change link.
                   SendMail(model.Email, newbody, subject);

                Registration register = _RegistrationRepository.GetUserByEmail(model.Email);

                register.Code = token;
                register.PasswordReset = true;
                _RegistrationRepository.Update(register);

            }
            catch (Exception ex)
            {
                ViewBag.errorMessage = "Something went wrong " + ex;
                return View(model);
            }
            ViewBag.errorMessage = "Password reset link has been sent to " + model.Email;
            return View(model);
        }

        public void SendMail(string tomailId, string body, string subject)
        {
       
                string UserID = ConfigurationManager.AppSettings["UserID"];
                string Password = ConfigurationManager.AppSettings["Password"];
                string SMTPPort = ConfigurationManager.AppSettings["SMTPPort"];
                string Host = ConfigurationManager.AppSettings["Host"];

                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                mail.To.Add(tomailId);
                mail.From = new System.Net.Mail.MailAddress(UserID);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = Host;
                smtp.Port = Convert.ToInt16(SMTPPort);
                smtp.Credentials = new NetworkCredential(UserID, Password);
                smtp.EnableSsl = false;
                smtp.Send(mail);
           
        }

        [HttpGet]
        public ActionResult ResetPassword(string email, string code)
        {
            RegistrationViewModel model = new RegistrationViewModel();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(code))
            {
                ViewBag.errorMessage = "Something went wrong please contact admin";
                return View();
            }
            var RegisteredUser = _RegistrationRepository.GetUserByEmailAndCode(email, code);

            if (RegisteredUser == null)
            {
                ViewBag.errorMessage = "Something went wrong please contact admin";
                return View();
            }

            model.Email = RegisteredUser.Email;

            return View(model);
        }

        [HttpPost]
        public ActionResult ResetPassword(RegistrationViewModel model)
        {
            if (string.IsNullOrEmpty(model.Password))
            {
                ViewBag.errorMessage = "Please enter password.";
                return View(model);
            }
            if (string.IsNullOrEmpty(model.Email))
            {
                ViewBag.errorMessage = "Email is not exist.";
                return View(model);
            }

            Registration RegisteredUser = _RegistrationRepository.GetUserByEmail(model.Email);

            if (RegisteredUser == null)
            {
                return View(model);
            }

            RegisteredUser.Password = model.Password;
            RegisteredUser.PasswordReset = false;

            _RegistrationRepository.Update(RegisteredUser);

            return RedirectToAction("Login", "Account");
        }

        #endregion
    }
}