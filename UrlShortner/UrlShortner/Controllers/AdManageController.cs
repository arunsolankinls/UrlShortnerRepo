using AutoMapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UrlShortner.Comman;
using UrlShortner.Database.Entities;
using UrlShortner.Database.Repository;
using UrlShortner.Models;

namespace UrlShortner.Controllers
{
    [UserAuthenticationFilter]
    public class AdManageController : BaseController
    {
        private readonly IAdManageRepository adManageRepository;
        private readonly IRegistrationRepository registrationRepository;
        public string domain_aiim = ConfigurationManager.AppSettings["HostUrl1"].ToString();
        public string domain_ijli = ConfigurationManager.AppSettings["HostUrl2"].ToString();
        public string directorypath= ConfigurationManager.AppSettings["DirectoryPath"].ToString();

        // GET: AdManage
        public AdManageController(IAdManageRepository AdManageRepository, IRegistrationRepository RegistrationRepository)
        {
            this.adManageRepository = AdManageRepository;
            this.registrationRepository = RegistrationRepository;
        }
        public ActionResult Index()
        {
            var objAdManage = adManageRepository.GetAllAdsByClient(Convert.ToInt64(Session["UserId"]));

            return View(Mapper.Map<IEnumerable<AdManage>, IEnumerable<AdManageViewModel>>(objAdManage));
        }
        public ActionResult Create()
        {
            AdManageViewModel objAdManage = new AdManageViewModel();

            return View(objAdManage);
        }

        [HttpPost]
        public ActionResult Create(AdManageViewModel model, HttpPostedFileBase file)
        {
            string filename = string.Empty;
            Guid filenameGuid = Guid.NewGuid();
            //string requesturl = HttpContext.Request.Url.AbsoluteUri.ToString();

            var requesturl = string.Format("{0}://{1}{2}",
              HttpContext.Request.Url.Scheme,
              HttpContext.Request.Url.Authority, Url.Content("~"));

            if (!ModelState.IsValid)
                return View(model);
            else
            {
                try
                {
                    if (Request.Files[0].ContentLength > 0)
                    {
                        string _path = "";
                        //if (requesturl.Contains(domain_ijli) || requesturl.Contains("https://ijl.li/"))
                        if (requesturl.Contains(domain_ijli))
                        {
                            _path = string.Format(@"{0}AdHTML\\{1}.html", directorypath, filenameGuid.ToString());
                            Request.Files[0].SaveAs(_path);
                        }
                        else
                        {
                            //_path = @"C:\Inetpub\vhosts\aiim.li\httpdocs\UrlShotner\AdHTML\" + filenameGuid.ToString() + ".html";
                            _path = string.Format(@"{0}AdHTML\\{1}.html", directorypath, filenameGuid.ToString());
                            Request.Files[0].SaveAs(_path);
                        }
                    }

                    AdManage objAdManage = new AdManage();
                    objAdManage = Mapper.Map<AdManageViewModel, AdManage>(model);
                    objAdManage.Addeddate = DateTime.Now;
                    objAdManage.RegistAdId = Convert.ToInt64(Session["UserId"]);
                    objAdManage.UpdateDate = DateTime.Now;
                    objAdManage.UpdatedBy = Convert.ToInt32(Session["UserId"]);
                    //objAdManage.UploadFile = Session["UserId"].ToString();
                    objAdManage.UploadFile = filenameGuid.ToString() + ".html";

                    adManageRepository.AddAd(objAdManage);

                    if (adManageRepository.SaveAll())
                    {
                        return RedirectToAction("Index", "AdManage");
                    }
                    else
                    {
                        return View(model);
                    }
                }
                catch (Exception ex)
                {
                    //throw ex;
                    return View(model);
                }
            }
        }

        public ActionResult Edit(int Id)
        {
            try
            {
                AdManageViewModel objAdManage = new AdManageViewModel();
                objAdManage = Mapper.Map<AdManage, AdManageViewModel>(adManageRepository.GetAdById(Id));
                TempData["AdId"] = objAdManage.AdId;
                TempData.Keep();
                objAdManage.Description = System.Web.HttpUtility.HtmlDecode(objAdManage.Description);
                return View(objAdManage);
            }
            catch (Exception ex)
            {

            }
            return View();
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult EditPost(AdManageViewModel model)
        {
            try
            {
                if (!ModelState.IsValid) return View(model);

                else
                {
                    long UrlId = (long)TempData["AdId"];
                    AdManage objAdManage = adManageRepository.GetAdById(UrlId);
                    objAdManage.UpdateDate = DateTime.Now.Date;
                    objAdManage.UpdatedBy = Convert.ToInt32(Session["UserId"]);
                    objAdManage.Title = model.Title;
                    objAdManage.Description = model.Description;
                    objAdManage.RegistAdId = Convert.ToInt64(Session["UserId"]);
                    adManageRepository.Edit(objAdManage);
                    return RedirectToAction("Index", "AdManage");
                }
            }
            catch (Exception ex)
            {
            }
            return View();
        }

        public ActionResult Delete(int Id)
        {
            try
            {
                if (adManageRepository.Delete(Id))
                {
                    return RedirectToAction("Index", "AdManage");
                }
            }
            catch (Exception)
            {

            }
            return View();
        }
        public bool IsAdmin(long Id)
        {
            Registration register = registrationRepository.GetClientById(Id);

            if (register != null && register.MemberType == ConfigurationManager.AppSettings["AdminType"].ToString())
                return true;

            return false;
        }
        //public void MoveFileFromFTP(string filename, byte[] fileBytes)
        //{
        //    try
        //    {
        //        string ftp = "ftp://192.168.1.200/";
        //        string ftpFolder = "AdHTML/";
        //        //byte[] fileBytes = null;

        //        //Create FTP Request.
        //        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftp + ftpFolder + filename + ".html");
        //        request.Method = WebRequestMethods.Ftp.UploadFile;

        //        //Enter FTP Server credentials.
        //        request.Credentials = new NetworkCredential("atul", "nls43@@321");
        //        request.ContentLength = fileBytes.Length;
        //        request.UsePassive = true;
        //        request.UseBinary = true;
        //        request.ServicePoint.ConnectionLimit = fileBytes.Length;
        //        request.EnableSsl = false;

        //        using (Stream requestStream = request.GetRequestStream())
        //        {
        //            requestStream.Write(fileBytes, 0, fileBytes.Length);
        //            requestStream.Close();
        //        }
        //        FtpWebResponse response = (FtpWebResponse)request.GetResponse();
        //        response.Close();
        //    }
        //    catch (WebException ex)
        //    {
        //        throw new Exception((ex.Response as FtpWebResponse).StatusDescription);
        //    }
        //}
    }
}