using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UrlShortnerClient.Models
{
    public class UrlShortnerViewModel
    {
        public long UrlId { get; set; }
        public string OriginalUrl { get; set; }
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