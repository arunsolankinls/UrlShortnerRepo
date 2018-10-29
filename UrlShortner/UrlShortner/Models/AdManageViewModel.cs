using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using UrlShortner.LocalResource;

namespace UrlShortner.Models
{
    public class AdManageViewModel
    {
        public long AdId { get; set; }
        [Display(Name = "Title", ResourceType = typeof(Resource))]
        public string Title { get; set; }
        [Display(Name = "Description", ResourceType = typeof(Resource))]
        public string Description { get; set; }
        public int AddedBy { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime Addeddate { get; set; }
        public DateTime UpdateDate { get; set; }
        public virtual ICollection<UrlShortnerViewModel> UrlShortner { get; set; }
        public virtual ICollection<UrlShortnerHistoryViewModel> UrlShortnerHistory { get; set; }
        public long RegistAdId { get; set; }
        public virtual RegistrationViewModel Registration { get; set; }
        //add for upload file
        [Display(Name = "UploadFile", ResourceType = typeof(Resource))]

        public string UploadFile { get; set; }

    }
}