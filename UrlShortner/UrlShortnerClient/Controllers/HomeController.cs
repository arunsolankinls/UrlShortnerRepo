using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UrlShortner.Database.Repository;
using System.IO;
using UrlShortnerClient.Models;
using System.Configuration;
using AutoMapper;
using System.Text;
using UrlShortner.Database.Entities;
using UrlShortner.Database;
using UrlShortnerClient.Comman;

namespace UrlShortnerClient.Controllers
{
    public class HomeController : BaseController
    {
        private static List<int> numbers = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };
        //private static List<char> characters = new List<char>() { 'i', 'j', 'l', 'I' };
        //private static List<char> characters = new List<char>() { 'i', 'j', 'l', 'I','i'};
        private static List<char> characters = new List<char>();

        public static int count = 0;
        public static ShortnerContext _context = new ShortnerContext();

        private readonly IRegistrationRepository registrationRepository;
        private readonly IUrlShortnerRepository urlShortnerRepository;
        private readonly IAdManageRepository adManageRepository;
        private readonly IUrlVisitorLogRepository _urlVisitorLogRepository;
        public HomeController(IRegistrationRepository RegistrationRepository, IUrlShortnerRepository UrlShortnerRepository, IAdManageRepository AdManageRepository,
            IUrlVisitorLogRepository UrlVisitorLogRepository)
        {
            this.registrationRepository = RegistrationRepository;
            this.urlShortnerRepository = UrlShortnerRepository;
            this.adManageRepository = AdManageRepository;
            this._urlVisitorLogRepository = UrlVisitorLogRepository;
            count = urlShortnerRepository.CountAllShortenUrl();
            characters = GetCharactersList();
        }
        string hosturl = System.Configuration.ConfigurationManager.AppSettings["HostUrl2"].ToString();
        string shorturl = ConfigurationManager.AppSettings["ShortUrl"];
        string persiandomain = System.Configuration.ConfigurationManager.AppSettings["HostUrl2"].ToString();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AdView()
        {
            AdManageViewModel objAdView = new AdManageViewModel();
            return View(objAdView);
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UrlShortnerAction(string OriginalUrl)
        {
            var result = new JsonResult();

            if (string.IsNullOrEmpty(OriginalUrl))
            {
                result.Data = "Index";
                return result;
            }

            try
            {
                List<UrlShortnerViewModel> urlList = new List<UrlShortnerViewModel>();
                UrlShortnerViewModel Row = new UrlShortnerViewModel();

                List<UrlShortnerListViewModel> objeUrlList = new List<UrlShortnerListViewModel>();
                UrlShortnerListViewModel Row1 = new UrlShortnerListViewModel();

                if (string.IsNullOrEmpty(Session["UserName"].ToString()))
                {
                    result.Data = "Login";
                    return result;
                }
                else
                {
                    if ((bool)Session["PaymentStatus"] == false)
                    {
                        result.Data = "Plans";
                        return result;
                    }

                    if (OriginalUrl.StartsWith("http://") || OriginalUrl.StartsWith("https://")){

                    }
                    else {
                        OriginalUrl = "http://" + OriginalUrl;
                    }

                    //Select domain for persian domain
                    //string currenturl = HttpContext.Request.Url.AbsoluteUri;

                    //string currenturl = HttpContext.Request.Url.AbsoluteUri;

                    //if (currenturl.Contains(persiandomain))
                    //{
                    //    hosturl = persiandomain;
                    //}

                    //Create Unique short key and check if exist then recreate
                    string urlKey = "";
                    dynamic shorturlList;
                    do
                    {
                        urlKey = GetURL();
                        shorturlList = urlShortnerRepository.GetAllShortUrl(hosturl);
                    }
                    while (shorturlList.Contains(urlKey));

                    #region Inster Logic 
                    Row.OriginalUrl = OriginalUrl;
                    Row.RegisId = Convert.ToInt64(Session["UserId"]); 
                    Row.AdId = adManageRepository.MemberAdId(Convert.ToInt64(Session["UserId"]));
                    Row.AddedDate = DateTime.Now.Date;
                    Row.UpdatedDate = DateTime.Now.Date;
                    Row.ShortUrlKey = hosturl + urlKey;
                    urlList.Add(Row);
                    urlShortnerRepository.AddUrl(Mapper.Map<UrlShortnerViewModel, UrlShortner.Database.Entities.UrlShortner>(Row));
                    urlShortnerRepository.SaveAll();
                    #endregion

                    #region Create HTML
                    var strReturnHTMLGenerate = RenderViewToString("AdViewClone", "", ControllerContext);
                    string HTMLFileUrl = hosturl + "/AdHTML/" + adManageRepository.GetFileName(Row.AdId);
                    string HTMLAdViewUrl = hosturl + "/Ad/" + urlKey + ".html";
                    //script replace

                    //strReturnHTMLGenerate = strReturnHTMLGenerate.Replace("http://43.250.164.92:1150/AdHTML/ad.html", HTMLFileUrl);
                    strReturnHTMLGenerate = strReturnHTMLGenerate.Replace(hosturl+"AdHTML/ad.html", HTMLFileUrl);
                    strReturnHTMLGenerate = strReturnHTMLGenerate.Replace("http://www.example.com", OriginalUrl);
                    strReturnHTMLGenerate = strReturnHTMLGenerate.Replace("https://www.google.co.in", OriginalUrl);

                    using (FileStream fs = new FileStream(Server.MapPath("~\\Ad\\" + urlKey + ".html"), FileMode.Create))
                    {
                        using (StreamWriter w = new StreamWriter(fs, Encoding.UTF8))
                        {
                            w.Write(strReturnHTMLGenerate);
                        }
                    }
                    #endregion

                    //Row1.OriginalUrl = HTMLAdViewUrl;
                    Row1.OriginalUrl = urlKey;
                    //Row1.ShortUrlKey = ConfigurationManager.AppSettings["HostUrl"] + urlKey;
                    Row1.ShortUrlKey = shorturl + urlKey;

                    if (string.IsNullOrEmpty(Session["UrlList"].ToString()))
                    {
                        objeUrlList.Add(Row1);
                        Session["UrlList"] = objeUrlList;
                    }
                    else
                    {
                        objeUrlList = (List<UrlShortnerListViewModel>)Session["UrlList"];
                        objeUrlList.Add(Row1);
                    }
                    var strReturn = RenderViewToString("_ListofUrl", objeUrlList, ControllerContext);
                    result.Data = strReturn;
                    result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Data = ex.Message.ToString();
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return result;
            }

        }

        // Request for share URL functionality like Aiim.ly/ijil
        public ActionResult ShortURL(string shortlink)
        {
            if (string.IsNullOrEmpty(shortlink))
                return View("Index");

            string currenturl = Request.Url.GetLeftPart(UriPartial.Authority);
            if (currenturl.Contains(persiandomain))
            {
                hosturl = persiandomain;
            }

            //var shorturlList = urlShortnerRepository.GetAllShortUrl(hosturl);
            var shorturlList = urlShortnerRepository.GetUrlByShortUrlKey(hosturl, shortlink);

            if (shorturlList == null)
                return View("Index");

            //if (!shorturlList.Contains(shortlink))
            //    return View("Index");

            var path=Server.MapPath("~\\Ad\\" + shortlink + ".html");

            string htmlContent = System.IO.File.ReadAllText(path);

            ShortURLModel model = new ShortURLModel
            {
                HtmlPage = htmlContent
            };

            //Insert into UrlVisitorLog table for count visitors
            UrlVisitorLog visitorlog = new UrlVisitorLog()
            {
                UrlId = shorturlList.UrlId,
                RegistId = shorturlList.RegisId,
                VisitedDate = DateTime.Now.Date,
                ShortUrl= shorturlList.ShortUrlKey,
                OriginalUrl=shorturlList.OriginalUrl
            };
            _urlVisitorLogRepository.AddUrl(visitorlog);
            _urlVisitorLogRepository.SaveAll();

            return View(model);
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

        //Random generate short url string
        public static string GetURL()
        {
            string URL = "";
            Random rand = new Random();

            //posibility of characters count
            int posibilitycount = characters.Count;
            for (int i = 0; i < characters.Count-1; i++)
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

            // run the loop till I get a string of characters
            for (int i = 0; i < characters.Count; i++)
            {
                int random = rand.Next(0, characters.Count);
                URL += characters[random].ToString();
            }
            return URL;
        }

        //public static string GetURL()
        //{
        //    string URL = "";
        //    Random rand = new Random();
        //    for (int i = 0; i < characters.Count; i++)
        //    {
        //        int random = rand.Next(0, characters.Count);
        //        URL += characters[random].ToString();
        //    }
        //    return URL;
        //}

        public void AddNewShortCharcter()
        {
            ShortCharacters shortcharcters = new ShortCharacters();
            ShortnerContext _context = new ShortnerContext();

            var result = _context.ShortCharacters.Add(shortcharcters);
            int Return = _context.SaveChanges();

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