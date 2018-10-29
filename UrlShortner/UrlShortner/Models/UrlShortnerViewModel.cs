using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using UrlShortner.LocalResource;

namespace UrlShortner.Models
{
    public class UrlShortnerViewModel
    {
        public long UrlId { get; set; }
        [Display(Name = "OriginalUrl", ResourceType = typeof(Resource))]
        public string OriginalUrl { get; set; }
        [Display(Name = "ShortUrl", ResourceType = typeof(Resource))]
        public string ShortUrlKey { get; set; }
        
        public DateTime AddedDate { get; set; }
        
        public DateTime UpdatedDate { get; set; }
        
        public long UpdatedBy { get; set; }
        public long AdId { get; set; }
        public virtual AdManageViewModel AdManage { get; set; }
        public long RegisId { get; set; }
        public virtual RegistrationViewModel Registration { get; set; }
        
    }
}