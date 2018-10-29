using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortner.Database.Entities
{
    [Table("UrlVisitorLog")]
    public class UrlVisitorLog
    {
        [Key]
        public long UrlVisitorLogId { get; set; }

        //[ForeignKey("Registration")]
        public long RegistId { get; set; }
        //[ForeignKey("UrlShortner")]
        public long UrlId { get; set; }
        public DateTime VisitedDate { get; set; } = DateTime.Now.Date;
        public string ShortUrl { get; set; }
        public string OriginalUrl { get; set; }
        
        //public string urlcount { get; set; }
        //public virtual Registration Registration { get; set; }       
        //public virtual UrlShortner UrlShortner { get; set; }
    }
}
