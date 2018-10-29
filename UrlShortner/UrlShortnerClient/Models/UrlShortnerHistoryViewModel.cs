using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UrlShortnerClient.Models
{
    public class UrlShortnerHistoryViewModel
    {
        public long UrlId { get; set; }
        public string OriginalUrl { get; set; }
        public string ShortUrlKey { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public long UpdatedBy { get; set; }
        public virtual AdManageViewModel AdManageViewModel { get; set; }
        public virtual RegistrationViewModel RegistrationViewModel { get; set; }
        public virtual UrlShortnerViewModel UrlShortnerViewModel { get; set; }
    }
}