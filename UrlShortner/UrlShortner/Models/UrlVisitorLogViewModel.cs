using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using UrlShortner.LocalResource;

namespace UrlShortner.Models
{
    public class UrlVisitorLogViewModel
    {
        public UrlVisitorLogViewModel()
        {
            urlListModel = new List<UrlShortnerViewModel>();
        }

        public long UrlVisitorLogId { get; set; }
        public long RegistId { get; set; }
        public long UrlId { get; set; }
        [Display(Name = "VisitedDate", ResourceType = typeof(Resource))]
        public DateTime VisitedDate { get; set; }
        //public virtual UrlShortnerViewModel UrlShortner { get; set; }
        [Display(Name = "ShortUrl", ResourceType = typeof(Resource))]
        public string ShortUrl { get; set; }
        [Display(Name = "OriginalUrl", ResourceType = typeof(Resource))]
        public string OriginalUrl { get; set; }
        [Display(Name = "TotalVIsit", ResourceType = typeof(Resource))]
        public int count { get; set; }
        public virtual RegistrationViewModel Registration { get; set; }
        public virtual IList<UrlShortnerViewModel> urlListModel { get; set; }
    }
}