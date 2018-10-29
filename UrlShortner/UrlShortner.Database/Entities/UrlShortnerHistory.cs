using System;
using System.ComponentModel.DataAnnotations;

namespace UrlShortner.Database.Entities
{
    public class UrlShortnerHistory
    {
        [Key]
        public long UrlId { get; set; }
        public string OriginalUrl { get; set; }
        public string ShortUrlKey { get; set; }        
        public DateTime AddedDate { get; set; } = DateTime.Now.Date;
        public DateTime UpdatedDate { get; set; } = DateTime.Now.Date;
        public long UpdatedBy { get; set; }
        public virtual AdManage AdManage { get; set; }
        public virtual Registration Registration { get; set; }
        public virtual UrlShortner UrlShortner { get; set; }
    }
}
