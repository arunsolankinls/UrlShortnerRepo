using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace UrlShortner.Database.Entities
{
    [Table("AdManage")]
    public class AdManage
    {
        public AdManage()
        {
            UrlShortner = new HashSet<UrlShortner>();
            UrlShortnerHistory = new HashSet<UrlShortnerHistory>();
        }
        [Key]
        public long AdId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }        
        public int AddedBy { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime Addeddate { get; set; } = DateTime.Now.Date;
        public DateTime UpdateDate { get; set; } = DateTime.Now.Date;
        public virtual ICollection<UrlShortner> UrlShortner { get; set; }
        public virtual ICollection<UrlShortnerHistory> UrlShortnerHistory { get; set; }

        [ForeignKey("Registration")]
        public long RegistAdId { get; set; }
        public virtual Registration Registration { get; set; }
        public string UploadFile { get; set; }



    }
}
