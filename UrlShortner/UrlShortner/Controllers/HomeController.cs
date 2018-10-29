using AutoMapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using UrlShortner.Comman;
using UrlShortner.Database;
using UrlShortner.Database.Entities;
using UrlShortner.Database.Repository;
using UrlShortner.Models;

namespace UrlShortner.Controllers
{
    [UserAuthenticationFilter]
    public class HomeController : BaseController
    {

        private static List<int> numbers = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };
        //private static List<char> characters = new List<char>() { 'i', 'j', 'l', 'I', 'i' };
        string hosturl = ConfigurationManager.AppSettings["HostUrl2"];

        private static List<char> characters = new List<char>();
        public static int count = 0;
        public static ShortnerContext _context = new ShortnerContext();

        private readonly IUrlShortnerRepository urlShortnerRepository;
        private readonly IAdManageRepository adManageRepository;
        private readonly IRegistrationRepository registrationRepository;
        private readonly IUrlVisitorLogRepository _urlVisitorLogRepository;

        public string domain_aiim = ConfigurationManager.AppSettings["HostUrl1"].ToString();
        public string domain_ijli = ConfigurationManager.AppSettings["HostUrl2"].ToString();

        public HomeController(IUrlShortnerRepository UrlShortnerRepository, IAdManageRepository AdManageRepository, IRegistrationRepository RegistrationRepository, IUrlVisitorLogRepository UrlVisitorLogRepository)
        {
            this.urlShortnerRepository = UrlShortnerRepository;
            this.adManageRepository = AdManageRepository;
            this.registrationRepository = RegistrationRepository;
            this._urlVisitorLogRepository = UrlVisitorLogRepository;
            count = urlShortnerRepository.CountAllShortenUrl();
            characters = GetCharactersList();
        }
        public ActionResult Index()
        {
            var objRegistraion = urlShortnerRepository.GetAllUrlsByClient(Convert.ToInt64(Session["UserId"]));
            return View(Mapper.Map<IEnumerable<Database.Entities.UrlShortner>, IEnumerable<UrlShortnerViewModel>>(objRegistraion));
        }


        public ActionResult Create()
        {
            UrlShortnerViewModel objUrlShortner = new UrlShortnerViewModel();

            dynamic admin_ad_list = null;

            if (IsAdmin(Int64.Parse(Session["UserId"].ToString())))
            {
                admin_ad_list = adManageRepository.GetAllAdsByClient(Int64.Parse(Session["UserId"].ToString())).Select(i => new { i.AdId, i.Title });
                ViewBag.VBAdList = new SelectList(admin_ad_list, "AdId", "Title");
            }
            return View(objUrlShortner);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(UrlShortnerViewModel model, FormCollection form)
        {
            string SelectedAdId = form["SelectedAd"];
            long J;
            //string requesturl = HttpContext.Request.Url.AbsoluteUri.ToString();
            var requesturl = string.Format("{0}://{1}{2}", HttpContext.Request.Url.Scheme,
                                                        HttpContext.Request.Url.Authority,
                                                        Url.Content("~"));
            if (!ModelState.IsValid)
                return View(model);
            else
            {
                //FTP Folder name. Leave blank if you want to upload to root folder.
                string ftpFolder = "Ad/";

                string urlKey = "";
                dynamic shorturlList;

                if (model.OriginalUrl.StartsWith("http://") || model.OriginalUrl.StartsWith("https://"))
                {

                }
                else
                {
                    model.OriginalUrl = "http://" + model.OriginalUrl;
                }

                //create short url key
                do
                {
                    urlKey = GetURL();
                    shorturlList = urlShortnerRepository.GetAllShortUrl(hosturl);
                }
                //check if already exist then create again short key
                while (shorturlList.Contains(urlKey));

                var strReturnHTMLGenerate = RenderViewToString("AdViewClone", "", ControllerContext);

                string HTMLFileUrl = "";

                if (!string.IsNullOrEmpty(SelectedAdId))
                    HTMLFileUrl = hosturl + "/AdHTML/" + adManageRepository.GetFileName(Int64.Parse(SelectedAdId.ToString()));
                else
                    HTMLFileUrl = hosturl + "/AdHTML/" + adManageRepository.GetFileName(adManageRepository.MemberAdId(Int64.Parse(Session["UserId"].ToString())));

                string HTMLAdViewUrl = hosturl + "/Ad/" + urlKey + ".html";

                strReturnHTMLGenerate = strReturnHTMLGenerate.Replace(hosturl + "AdHTML/ad.html", HTMLFileUrl);
                strReturnHTMLGenerate = strReturnHTMLGenerate.Replace("http://www.example.com", model.OriginalUrl);
                strReturnHTMLGenerate = strReturnHTMLGenerate.Replace("https://www.google.co.in", model.OriginalUrl);

                //Read the FileName and convert it to Byte array.

                try
                {
                    string directorypath = "";
                    if (requesturl.Contains(domain_ijli))
                    {
                        directorypath = @"C:\Inetpub\vhosts\ijl.li\httpdocs\UrlShotner\" + ftpFolder + urlKey + ".html";
                    }
                    else
                    {
                        directorypath = @"C:\Inetpub\vhosts\aiim.li\httpdocs\UrlShotner\" + ftpFolder + urlKey + ".html";
                    }
                    //using (FileStream fs = new FileStream(Server.MapPath("~\\Ad\\" + urlKey + ".html"), FileMode.Create))
                    using (FileStream fs = new FileStream(directorypath, FileMode.Create))
                    {
                        using (StreamWriter w = new StreamWriter(fs, Encoding.UTF8))
                        {
                            w.Write(strReturnHTMLGenerate);
                        }
                    }
                }
                catch (Exception ex)
                {
                    return View(model);
                    //throw new Exception((ex.Response as FtpWebResponse).StatusDescription);
                }

                Database.Entities.UrlShortner objUrlShortner = new Database.Entities.UrlShortner();
                objUrlShortner = Mapper.Map<UrlShortnerViewModel, Database.Entities.UrlShortner>(model);
                objUrlShortner.AddedDate = DateTime.Now.Date;
                objUrlShortner.RegisId = long.TryParse(Session["UserId"].ToString(), out J) == true ? Int64.Parse(Session["UserId"].ToString()) : 0;
                objUrlShortner.UpdatedDate = DateTime.Now.Date;
                //objUrlShortner.ShortUrlKey = ConfigurationManager.AppSettings["HostUrl"] + GetURL();
                objUrlShortner.ShortUrlKey = hosturl + urlKey;
                objUrlShortner.UpdatedBy = Int64.Parse(Session["UserId"].ToString());

                //single user or admin 
                if ((string)Session["MemberShipType"] == "Single" || IsAdmin(Int64.Parse(Session["UserId"].ToString())))
                {
                    objUrlShortner.AdId = adManageRepository.MemberAdId(Int64.Parse(Session["UserId"].ToString()));
                }

                urlShortnerRepository.AddUrl(objUrlShortner);
                bool result = urlShortnerRepository.SaveAll();

                if (result == true)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return View(model);
                }
            }
        }

        public string RenderViewToString(string viewPath, object model, ControllerContext context)
        {
            var viewEngineResult = ViewEngines.Engines.FindPartialView(context, viewPath);
            var view = viewEngineResult.View;
            context.Controller.ViewData.Model = model;
            string result;
            using (var sw = new StringWriter())
            {
                var ctx = new ViewContext(context, view,
                                          context.Controller.ViewData,
                                          context.Controller.TempData,
                                          sw);
                view.Render(ctx, sw);
                result = sw.ToString();
            }
            return result;
        }


        public static string GetURL()
        {
            string URL = "";
            Random rand = new Random();
            // run the loop till I get a string of 10 characters

            //posibility of characters count
            int posibilitycount = characters.Count;
            for (int i = 0; i < characters.Count - 1; i++)
            {
                posibilitycount = posibilitycount * characters.Count;
            }
            //Add new character in list
            int newcharposition = 0;
            if ((posibilitycount - 5) < count)
            {
                newcharposition = characters.Count - 4;
                char newchar = characters[newcharposition];
                characters.Add(newchar);

                //Add new character
                ShortCharacters shortcharacter = new ShortCharacters();
                shortcharacter.Id = 1;
                var shortcharentity = _context.ShortCharacters.Find(shortcharacter.Id);
                shortcharentity.Characters = shortcharentity.Characters + "," + newchar;
                _context.Entry(shortcharentity).CurrentValues.SetValues(shortcharentity);
                _context.SaveChanges();
            }


            for (int i = 0; i < characters.Count; i++)
            {
                // Get random numbers, to get either a character or a number...
                //int random = rand.Next(0, 3);
                int random = rand.Next(0, characters.Count);
                URL += characters[random].ToString();
            }
            return URL;
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult EditPost(UrlShortnerViewModel model)
        {
            try
            {
                if (!ModelState.IsValid) return View(model);

                else
                {
                    long UrlId = (long)TempData["UrlId"];
                    Database.Entities.UrlShortner objUrlShortner = urlShortnerRepository.GetUrlById1(UrlId);
                    objUrlShortner.UpdatedDate = DateTime.Now.Date;
                    objUrlShortner.UpdatedBy = Convert.ToInt64(Session["UserId"]);
                    objUrlShortner.OriginalUrl = model.OriginalUrl;
                    urlShortnerRepository.Edit(objUrlShortner);
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
            }
            return View();

        }

        public ActionResult Edit(int Id)
        {
            try
            {
                UrlShortnerViewModel objUrlShortner = new UrlShortnerViewModel();
                objUrlShortner = Mapper.Map<Database.Entities.UrlShortner, UrlShortnerViewModel>(urlShortnerRepository.GetUrlById(Id));
                TempData["UrlId"] = objUrlShortner.UrlId;
                TempData.Keep();
                return View(objUrlShortner);
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
                if (urlShortnerRepository.Delete(Id))
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception)
            {

            }
            return View();
        }

        public ActionResult VisitorsLog()
        {
            List<UrlVisitorLogViewModel> viewmodel = new List<UrlVisitorLogViewModel>();

            var urlvisitorlistbyRegistId = _urlVisitorLogRepository.GetAllUrlVisitorLog(Convert.ToInt64(Session["UserId"]));

            var urlvisitorlist = urlvisitorlistbyRegistId.GroupBy(x => new { x.ShortUrl, x.VisitedDate, x.OriginalUrl })
                                               .Select(x => new { GroupData = x.Key, urlcount = x.Count() });

            if (urlvisitorlist == null)
                return View(viewmodel);

            foreach (var model in urlvisitorlist)
            {
                viewmodel.Add(new UrlVisitorLogViewModel()
                {
                    ShortUrl = model.GroupData.ShortUrl,
                    VisitedDate = model.GroupData.VisitedDate,
                    OriginalUrl = model.GroupData.OriginalUrl,
                    count = model.urlcount,
                });
            }

            return View(viewmodel);
        }
        public IList<UrlShortnerViewModel> UrlShortnerListModel(long UrlId)
        {
            var urllistbyid = urlShortnerRepository.GetUrlById(UrlId);

            IList<UrlShortnerViewModel> list = new List<UrlShortnerViewModel>();

            list.Add(new UrlShortnerViewModel()
            {
                ShortUrlKey = urllistbyid.ShortUrlKey,
            });
            return list;
        }
        //return character list from db.
        public List<char> GetCharactersList()
        {
            ShortCharacters shortcharacter = new ShortCharacters();
            shortcharacter.Id = 1;
            var shortcharentity = _context.ShortCharacters.Find(shortcharacter.Id);

            string charstring = shortcharentity.Characters.Replace(",", "");
            List<char> charlist = new List<char>(charstring);

            return charlist;
        }

        //check user is admin.
        public bool IsAdmin(long Id)
        {
            Registration register = registrationRepository.GetClientById(Id);

            if (register != null && register.MemberType == ConfigurationManager.AppSettings["AdminType"].ToString())
                return true;

            return false;
        }

        //Use  this methode for changing language(English,Persian).
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


    }
}