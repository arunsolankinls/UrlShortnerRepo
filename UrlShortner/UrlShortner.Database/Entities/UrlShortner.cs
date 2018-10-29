using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortner.Database.Entities
{
    [Table("UrlShortner")]
    public class UrlShortner
    {
        [Key]
        public long UrlId { get; set; }
        public string OriginalUrl { get; set; }
        public string ShortUrlKey { get; set; }
        public DateTime AddedDate { get; set; } = DateTime.Now.Date;
        public DateTime UpdatedDate { get; set; } = DateTime.Now.Date;
        public long UpdatedBy { get; set; }

        [ForeignKey("AdManage")]
        public long AdId { get; set; }
        public virtual AdManage AdManage { get; set; }     

        [ForeignKey("Registration")]
        public long RegisId { get; set; }
        public virtual Registration Registration { get; set; }       
    }
}
